using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Backend.DAL;

// This class is only used at design time (migrations)
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        try
        {
            // Debug output to troubleshoot arguments
            if (args != null && args.Length > 0)
            {
                Console.WriteLine($"AppDbContextFactory args: {string.Join(", ", args)}");
            }
            else
            {
                Console.WriteLine("No arguments provided to AppDbContextFactory");
            }

            // Create DbContext options without depending on configuration
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Always use SQL Server for migrations since that's our target
            // This bypasses all the configuration checks
            Console.WriteLine("Creating SQL Server migration context");

            // Use a hardcoded connection string that will work for creating migrations
            // No actual data access will happen during migration creation
            optionsBuilder.UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=temp_drommekopp;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
            );

            // Enable lazy loading to match AppDbContext configuration
            optionsBuilder.UseLazyLoadingProxies();

            return new AppDbContext(optionsBuilder.Options);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in AppDbContextFactory: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }
}