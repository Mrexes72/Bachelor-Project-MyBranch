using Microsoft.EntityFrameworkCore;
using Backend.DAL;
using Backend.Models;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using Serilog.Events;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog.Sinks.ApplicationInsights.TelemetryConverters;
using System.IO;
var builder = WebApplication.CreateBuilder(args);

// Configure services

var configuration = builder.Configuration;

// Load JWT configuration
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = jwtSettings["Key"];

try
{
    if (string.IsNullOrEmpty(key))
    {
        // For normal operation, we require a JWT key
        throw new InvalidOperationException("JWT key is not configured.");
    }

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // JWT Bearer configuration
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT issuer is not configured."),
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT audience is not configured."),
            ValidateLifetime = true
        };
    });
}
catch (Exception ex)
{
    // When running migrations or design-time operations, JWT key might not be available
    // Just log the error instead of crashing the application
    Console.WriteLine($"JWT key is not configured: {ex.Message}. Continuing without authentication services.");
}

// ✅ Ensure Identity does NOT override JWT-based authentication
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/api/auth/login"; // Prevents redirect to /Account/Login
    options.AccessDeniedPath = "/api/auth/accessdenied";
});

builder.Services.AddAuthorization();

// Configure Identity with roles
builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (builder.Environment.IsProduction())
    {
        // Use SQL Server for production
        var connectionString = builder.Configuration.GetConnectionString("DbContextConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            Console.WriteLine("Azure SQL connection string not found - using local fallback");
            connectionString = "Server=(localdb)\\mssqllocaldb;Database=temp_drommekopp;Trusted_Connection=True";
        }

        options.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5, // Retry up to 5 times
                maxRetryDelay: TimeSpan.FromSeconds(10), // Wait up to 10 seconds between retries
                errorNumbersToAdd: null // Retry on all transient errors
            );
        });
    }
    else
    {
        // Use SQLite for development
        var connectionString = builder.Configuration.GetConnectionString("DbContextConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            Console.WriteLine("SQLite connection string not found - using local fallback");
            connectionString = "Data Source=drommekopp.db";
        }

        options.UseSqlite(connectionString);
    }
});

// Add Identity for ApplicationUser and IdentityRole
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

builder.Services.AddControllers();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder
            .WithOrigins(
                "http://localhost:3000", // Local development
                "https://victorious-stone-0a0d4a303.6.azurestaticapps.net"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddScoped<ITestRepository, TestRepository>();
builder.Services.AddScoped<IIngredientRepository, IngredientRepository>();
builder.Services.AddScoped<IDrinkRepository, DrinkRepository>();
builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
builder.Services.AddScoped<IMenuItemRepository, MenuItemRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();

builder.Services.AddApplicationInsightsTelemetry();

/* Logger config... */
var loggerConfiguration = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console() // This requires Serilog.Sinks.Console
    .Enrich.FromLogContext();

// Configure Application Insights with modern approach
var appInsightsKey = builder.Configuration["ApplicationInsights:InstrumentationKey"];
var appInsightsConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];

if (!string.IsNullOrEmpty(appInsightsConnectionString))
{
    // Prefer connection string (modern approach)
    var telemetryConfig = new TelemetryConfiguration { ConnectionString = appInsightsConnectionString };
    loggerConfiguration.WriteTo.ApplicationInsights(telemetryConfig, TelemetryConverter.Traces);
}
else if (!string.IsNullOrEmpty(appInsightsKey))
{
    // Fall back to instrumentation key (legacy approach)
    var telemetryConfig = new TelemetryConfiguration();
#pragma warning disable CS0618 // Suppress obsolete warning
    telemetryConfig.ConnectionString = $"InstrumentationKey={appInsightsKey}";
#pragma warning restore CS0618
    loggerConfiguration.WriteTo.ApplicationInsights(telemetryConfig, TelemetryConverter.Traces);
}

// Filter out noisy database commands
loggerConfiguration.Filter.ByExcluding(e =>
    e.Properties.TryGetValue("SourceContext", out var value) &&
    e.Level == LogEventLevel.Information &&
    e.MessageTemplate.Text.Contains("Executed DbCommand"));

// Try to write to Azure's log directory only if it exists
string logDirectory = "D:\\home\\LogFiles\\Application";
if (Directory.Exists(logDirectory))
{
    loggerConfiguration.WriteTo.File(
        Path.Combine(logDirectory, "app_.log"),
        rollingInterval: RollingInterval.Day);
}
else
{
    // Fallback to local logs in development
    Directory.CreateDirectory("APILogs");
    loggerConfiguration.WriteTo.File(
        Path.Combine("APILogs", "app_.log"),
        rollingInterval: RollingInterval.Day);
}

var logger = loggerConfiguration.CreateLogger();
builder.Logging.AddSerilog(logger);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    DBInit.Seed(app);
}

if (app.Environment.IsProduction())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
    }
    DBInit.Seed(app);
}

/* app.UseHttpsRedirection(); */
app.UseStaticFiles();
app.UseRouting();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(name: "api", pattern: "{controller}/{action=Index}/{id?}");

// Seeding database with roles and users
await SeedDatabaseAsync(app.Services);

app.Run();

// Database seeding method for roles
static async Task SeedDatabaseAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

    // 1) Create roles
    string[] roleNames = { "Admin", "User" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (!roleResult.Succeeded)
            {
                Console.WriteLine($"Failed to create role '{roleName}': {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
            }
        }
    }

    // 2) Create admin user if not exists
    var adminUsername = config["AdminCredentials:Username"] ?? "Admin";
    var adminEmail = config["AdminCredentials:Email"] ?? "admin@site.com";
    var adminPassword = config["AdminCredentials:Password"];

    // In production, require proper configuration of admin password
    if (string.IsNullOrEmpty(adminPassword))
    {
        if (env.IsProduction())
        {
            throw new InvalidOperationException("Admin password is not configured in the production environment!");
        }

        // Log a warning for development environments
        Console.WriteLine("WARNING: Admin password is not configured. Skipping admin user creation.");
        return; // Skip admin user creation if no password is provided
    }

    var existingAdmin = await userManager.FindByNameAsync(adminUsername);

    if (existingAdmin == null)
    {
        var adminUser = new ApplicationUser
        {
            UserName = adminUsername,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var createResult = await userManager.CreateAsync(adminUser, adminPassword);
        if (createResult.Succeeded)
        {
            var roleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
            if (!roleResult.Succeeded)
            {
                Console.WriteLine($"Failed to assign 'Admin' role to '{adminUsername}': {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
            }
            else
            {
                Console.WriteLine($"Seeded Admin user '{adminUsername}'");
            }
        }
        else
        {
            Console.WriteLine($"Failed to create user '{adminUsername}': {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
        }
    }
    else
    {
        Console.WriteLine($"Admin user '{adminUsername}' already exists. Skipping creation.");
    }
}