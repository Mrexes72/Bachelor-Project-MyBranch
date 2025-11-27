using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.DTOs;
using Backend.DAL;
using Backend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IApplicationUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;

        public AuthController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IApplicationUserRepository userRepository,
            IConfiguration configuration,
            ILogger<AuthController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userRepository = userRepository;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger;
        }

        // GET: api/auth/user
        [HttpGet("user")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // ✅ Use JWT authentication
        public IActionResult GetUserIdentity()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var isAdmin = User.IsInRole("Admin");
                return Ok(new { name = User.Identity.Name });
            }
            return Unauthorized(new ApiResponse { Message = "User is not authenticated." });
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _logger.LogWarning("Registration failed due to validation errors: {Errors}", string.Join(", ", errors));

                return BadRequest(new ApiResponse
                {
                    Message = "Invalid registration data",
                    Errors = errors.ToList()
                });
            }

            var user = new ApplicationUser { UserName = registerRequest.Username, Email = registerRequest.Username };
            var result = await _userManager.CreateAsync(user, registerRequest.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User registered successfully.");
                return Ok(new ApiResponse { Message = "Registration successful" });
            }

            _logger.LogWarning("Registration failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            return BadRequest(new ApiResponse
            {
                Message = "Registration failed",
                Errors = result.Errors.Select(e => e.Description).ToList()
            });
        }

        // GET: api/auth/user/role
        [HttpGet("user/role")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // ✅ Use JWT authentication
        public IActionResult GetUserRole()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var role = User.IsInRole("Admin") ? "Admin" : "User"; // Adjust roles as needed

                return Ok(new { role });
            }

            return Unauthorized(new ApiResponse { Message = "User is not authenticated." });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new ApiResponse
                {
                    Message = "Invalid login data",
                    Errors = errors.ToList()
                });
            }

            try
            {
                var user = await _userManager.FindByNameAsync(loginRequest.Username);
                if (user == null || !await _userManager.CheckPasswordAsync(user, loginRequest.Password))
                {
                    _logger.LogWarning($"Login failed: User '{loginRequest.Username}' not found or incorrect password.");
                    return Unauthorized(new ApiResponse { Message = "Invalid username or password" });
                }

                // 1) Fetch all roles for this user
                var roles = await _userManager.GetRolesAsync(user);

                // 2) Build a list of claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id ?? string.Empty),
                    new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                };

                // 3) Add a ClaimTypes.Role claim for each role in the DB
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                // 4) Construct the JWT as before
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = _configuration["Jwt:Key"];
                if (string.IsNullOrEmpty(key) || key.Length < 32)
                {
                    _logger.LogError("JWT key is not configured or is too short.");
                    return StatusCode(500, new ApiResponse { Message = "Internal server error: JWT key is not configured or is too short." });
                }

                var keyBytes = Encoding.UTF8.GetBytes(key);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["Jwt:ExpireMinutes"])),
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"],
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(keyBytes),
                        SecurityAlgorithms.HmacSha256Signature
                    )
                };

                // 5) Create the JWT token
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                _logger.LogInformation($"User '{user.UserName}' logged in successfully.");

                return Ok(new ApiResponse { Message = tokenString });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                return StatusCode(500, new ApiResponse { Message = "Internal server error" });
            }
        }

        // POST: api/auth/logout
        [HttpPost("logout")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // ✅ Use JWT authentication
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return Ok(new ApiResponse { Message = "Logout successful" });
        }

        // GET: api/auth/user/details
        [HttpGet("user/details")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // ✅ Use JWT authentication
        public async Task<IActionResult> GetUserDetails()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    return Ok(new
                    {
                        username = user.UserName,
                        email = user.Email,
                        role = User.IsInRole("Admin") ? "Admin" : "User"
                    });
                }
            }
            return Unauthorized(new ApiResponse { Message = "User is not authenticated." });
        }

        // POST: api/auth/change-password
        [HttpPost("change-password")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // ✅ Use JWT authentication
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse
                {
                    Message = "Invalid password change request",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(new ApiResponse { Message = "User not found" });
            }

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (result.Succeeded)
            {
                _logger.LogInformation($"User '{user.UserName}' changed password successfully.");
                return Ok(new ApiResponse { Message = "Password changed successfully" });
            }

            return BadRequest(new ApiResponse
            {
                Message = "Password change failed",
                Errors = result.Errors.Select(e => e.Description).ToList()
            });
        }

        // DELETE: api/auth/delete-account
        [HttpDelete("delete-account")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // ✅ Use JWT authentication
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(new ApiResponse { Message = "User not found" });
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation($"User '{user.UserName}' deleted their account.");
                return Ok(new ApiResponse { Message = "Account deleted successfully" });
            }

            return BadRequest(new ApiResponse
            {
                Message = "Account deletion failed",
                Errors = result.Errors.Select(e => e.Description).ToList()
            });
        }

        [HttpPost("add-favorite-drink")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AddFavoriteDrink([FromBody] Drink drink)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ApiResponse { Message = "User is not authenticated." });
            }

            var result = await _userRepository.AddFavoriteDrinkAsync(userId, drink);

            if (result)
            {
                _logger.LogInformation($"User '{userId}' added a favorite drink.");
                return Ok(new ApiResponse { Message = "Favorite drink added successfully" });
            }

            return BadRequest(new ApiResponse { Message = "Failed to add favorite drink" });
        }

        [HttpPost("add-created-drink")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AddCreatedDrink([FromBody] Drink drink)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ApiResponse { Message = "User is not authenticated." });
            }

            var result = await _userRepository.AddCreatedDrinkAsync(userId, drink);

            if (result)
            {
                _logger.LogInformation($"User '{userId}' added a created drink.");
                return Ok(new ApiResponse { Message = "Created drink added successfully" });
            }

            return BadRequest(new ApiResponse { Message = "Failed to add created drink" });
        }
    }

    public class LoginRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        [Required]
        [MinLength(4, ErrorMessage = "Username must be at least 4 characters long.")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; } = string.Empty;
    }

    public class ChangePasswordRequest
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ApiResponse
    {
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new List<string>();
    }
}