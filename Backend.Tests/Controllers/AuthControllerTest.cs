using Xunit;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Backend.Controllers;
using Backend.DAL;
using Backend.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Backend.Tests;

public class AuthControllerTests
{
  private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
  private readonly Mock<SignInManager<ApplicationUser>> _mockSignInManager;
  private readonly Mock<IApplicationUserRepository> _mockUserRepository;
  private readonly Mock<ILogger<AuthController>> _mockLogger;
  private readonly Mock<IConfiguration> _mockConfiguration;
  private readonly AuthController _controller;


  public AuthControllerTests()
  {
    _mockUserManager = new Mock<UserManager<ApplicationUser>>(
      Mock.Of<IUserStore<ApplicationUser>>(),
      It.IsAny<IOptions<IdentityOptions>>(),
      It.IsAny<IPasswordHasher<ApplicationUser>>(),
      It.IsAny<IEnumerable<IUserValidator<ApplicationUser>>>(),
      It.IsAny<IEnumerable<IPasswordValidator<ApplicationUser>>>(),
      It.IsAny<ILookupNormalizer>(),
      It.IsAny<IdentityErrorDescriber>(),
      It.IsAny<IServiceProvider>(),
      It.IsAny<ILogger<UserManager<ApplicationUser>>>());
    _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
        _mockUserManager.Object,
        Mock.Of<IHttpContextAccessor>(),
        Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
        It.IsAny<IOptions<IdentityOptions>>(),
        It.IsAny<ILogger<SignInManager<ApplicationUser>>>(),
        It.IsAny<IAuthenticationSchemeProvider>(),
        It.IsAny<IUserConfirmation<ApplicationUser>>());
    _mockUserRepository = new Mock<IApplicationUserRepository>();
    _mockLogger = new Mock<ILogger<AuthController>>();
    _mockConfiguration = new Mock<IConfiguration>();

    _controller = new AuthController(
        _mockSignInManager.Object,
        _mockUserManager.Object,
        _mockUserRepository.Object,
        _mockConfiguration.Object,
        _mockLogger.Object);
  }

  [Fact]
  public void GetUserIdentity_ReturnsOk_WhenUserIsAuthenticated()
  {
    // Arrange
    var mockClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
    {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Role, "User")
    }, "mock"));

    var controller = new AuthController(
          _mockSignInManager.Object,
          _mockUserManager.Object,
          _mockUserRepository.Object,
          _mockConfiguration.Object,
          _mockLogger.Object)
    {
      ControllerContext = new ControllerContext
      {
        HttpContext = new DefaultHttpContext { User = mockClaimsPrincipal }
      }
    };

    // Act
    var result = controller.GetUserIdentity();

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.NotNull(okResult.Value);

    // Access the anonymous type using dynamic
    var response = okResult.Value as dynamic;
    Assert.NotNull(response);
  }

  [Fact]
  public void GetUserIdentity_ReturnsUnauthorized_WhenUserIsNotAuthenticated()
  {
    // Arrange
    var mockClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity()); // No identity or claims

    var controller = new AuthController(
        _mockSignInManager.Object,
        _mockUserManager.Object,
        _mockUserRepository.Object,
        _mockConfiguration.Object,
        _mockLogger.Object)
    {
      ControllerContext = new ControllerContext
      {
        HttpContext = new DefaultHttpContext { User = mockClaimsPrincipal }
      }
    };

    // Act
    var result = controller.GetUserIdentity();

    // Assert
    var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
    Assert.NotNull(unauthorizedResult.Value);
  }

  [Fact]
  public async Task AddFavoriteDrink_ReturnsOkResult_WhenSuccessful()
  {
    // Arrange
    var userId = "test-user-id";
    var drink = new Drink { DrinkId = 1, Name = "Test Drink" };
    _mockUserRepository.Setup(repo => repo.AddFavoriteDrinkAsync(userId, drink)).ReturnsAsync(true);
    var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
    {
                new Claim(ClaimTypes.NameIdentifier, userId)
    }, "mock"));
    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext { User = user }
    };

    // Act
    var result = await _controller.AddFavoriteDrink(drink);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.NotNull(okResult.Value);
    dynamic response = okResult.Value; // Use dynamic to access the anonymous type
    Assert.NotNull(response);
    Assert.Equal("Favorite drink added successfully", response?.Message); // Use the correct property name
  }

  [Fact]
  public async Task AddFavoriteDrink_ReturnsUnauthorized_WhenUserNotAuthenticated()
  {
    // Arrange
    var drink = new Drink { DrinkId = 1, Name = "Test Drink" };
    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
    };

    // Act
    var result = await _controller.AddFavoriteDrink(drink);

    // Assert
    var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
    Assert.NotNull(unauthorizedResult.Value);

    // Access the anonymous type using dynamic
    var response = unauthorizedResult.Value as dynamic;
    Assert.NotNull(response);
  }

  [Fact]
  public async Task AddCreatedDrink_ReturnsOkResult_WhenSuccessful()
  {
    // Arrange
    var userId = "test-user-id";
    var drink = new Drink { DrinkId = 1, Name = "Test Drink" };
    _mockUserRepository.Setup(repo => repo.AddCreatedDrinkAsync(userId, drink)).ReturnsAsync(true);
    var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
    {
                new Claim(ClaimTypes.NameIdentifier, userId)
    }, "mock"));
    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext { User = user }
    };

    // Act
    var result = await _controller.AddCreatedDrink(drink);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.NotNull(okResult.Value);
    dynamic response = okResult.Value; // Use dynamic to access the anonymous type
    Assert.NotNull(response);
    Assert.Equal("Created drink added successfully", response?.Message); // Use the correct property name
  }

  [Fact]
  public async Task AddCreatedDrink_ReturnsUnauthorized_WhenUserNotAuthenticated()
  {
    // Arrange
    var drink = new Drink { DrinkId = 1, Name = "Test Drink" };
    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
    };

    // Act
    var result = await _controller.AddCreatedDrink(drink);

    // Assert
    var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
    Assert.NotNull(unauthorizedResult.Value);
    dynamic response = unauthorizedResult.Value; // Use dynamic to access the anonymous type
    Assert.NotNull(response);
    Assert.Equal("User is not authenticated.", response?.Message); // Use the correct property name
  }

  [Fact]
  public async Task Register_ReturnsOk_WhenRegistrationIsSuccessful()
  {
    // Arrange
    var registerRequest = new RegisterRequest
    {
      Username = "testuser",
      Password = "Test@1234"
    };

    _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), registerRequest.Password))
        .ReturnsAsync(IdentityResult.Success);

    var controller = new AuthController(
        _mockSignInManager.Object,
        _mockUserManager.Object,
        _mockUserRepository.Object,
        _mockConfiguration.Object,
        _mockLogger.Object);

    // Act
    var result = await controller.Register(registerRequest);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.NotNull(okResult.Value);

    var response = Assert.IsType<ApiResponse>(okResult.Value);
    Assert.NotNull(response);
    Assert.Equal("Registration successful", response.Message);
  }

  [Fact]
  public async Task Register_ReturnsBadRequest_WhenModelStateIsInvalid()
  {
    // Arrange
    var registerRequest = new RegisterRequest
    {
      Username = "", // Invalid username
      Password = "short" // Invalid password
    };

    var controller = new AuthController(
        _mockSignInManager.Object,
        _mockUserManager.Object,
        _mockUserRepository.Object,
        _mockConfiguration.Object,
        _mockLogger.Object);

    controller.ModelState.AddModelError("Username", "The Username field is required.");
    controller.ModelState.AddModelError("Password", "The Password must be at least 6 characters long.");

    // Act
    var result = await controller.Register(registerRequest);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    Assert.NotNull(badRequestResult.Value);

    var response = Assert.IsType<ApiResponse>(badRequestResult.Value);
    Assert.NotNull(response);
    Assert.Equal("Invalid registration data", response.Message);
  }

  [Fact]
  public async Task Register_ReturnsBadRequest_WhenRegistrationFails()
  {
    // Arrange
    var registerRequest = new RegisterRequest
    {
      Username = "testuser",
      Password = "Test@1234"
    };

    var identityErrors = new List<IdentityError>
    {
        new IdentityError { Description = "Password is too weak." },
        new IdentityError { Description = "Username is already taken." }
    };

    _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), registerRequest.Password))
        .ReturnsAsync(IdentityResult.Failed(identityErrors.ToArray()));

    var controller = new AuthController(
        _mockSignInManager.Object,
        _mockUserManager.Object,
        _mockUserRepository.Object,
        _mockConfiguration.Object,
        _mockLogger.Object);

    // Act
    var result = await controller.Register(registerRequest);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    Assert.NotNull(badRequestResult.Value);

    var response = Assert.IsType<ApiResponse>(badRequestResult.Value);
    Assert.NotNull(response);
    Assert.Equal("Registration failed", response.Message);
    Assert.Contains("Password is too weak.", response.Errors);
    Assert.Contains("Username is already taken.", response.Errors);
  }

  [Fact]
  public void GetUserRole_ReturnsAdminRole_WhenUserIsAdmin()
  {
    // Arrange
    var mockClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
    {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Role, "Admin")
    }, "mock"));

    var controller = new AuthController(
        _mockSignInManager.Object,
        _mockUserManager.Object,
        _mockUserRepository.Object,
        _mockConfiguration.Object,
        _mockLogger.Object)
    {
      ControllerContext = new ControllerContext
      {
        HttpContext = new DefaultHttpContext { User = mockClaimsPrincipal }
      }
    };

    // Act
    var result = controller.GetUserRole();

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.NotNull(okResult.Value);

    var response = okResult.Value as dynamic;
    Assert.NotNull(response);
  }

  [Fact]
  public async Task GetUserDetails_ReturnsUnauthorized_WhenUserIsAuthenticatedButNotFound()
  {
    // Arrange
    var mockClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
    {
        new Claim(ClaimTypes.Name, "testuser"),
        new Claim(ClaimTypes.Role, "User")
    }, "mock"));

    _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
        .ReturnsAsync((ApplicationUser)null!); // Simuler at brukeren ikke finnes

    var controller = new AuthController(
        _mockSignInManager.Object,
        _mockUserManager.Object,
        _mockUserRepository.Object,
        _mockConfiguration.Object,
        _mockLogger.Object)
    {
      ControllerContext = new ControllerContext
      {
        HttpContext = new DefaultHttpContext { User = mockClaimsPrincipal }
      }
    };

    // Act
    var result = await controller.GetUserDetails();

    // Assert
    var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
    Assert.NotNull(unauthorizedResult.Value);

    var response = Assert.IsType<ApiResponse>(unauthorizedResult.Value);
    Assert.Equal("User is not authenticated.", response.Message);
  }

  [Fact]
  public void GetUserRole_ReturnsUserRole_WhenUserIsNotAdmin()
  {
    // Arrange
    var mockClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
    {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Role, "User")
    }, "mock"));

    var controller = new AuthController(
        _mockSignInManager.Object,
        _mockUserManager.Object,
        _mockUserRepository.Object,
        _mockConfiguration.Object,
        _mockLogger.Object)
    {
      ControllerContext = new ControllerContext
      {
        HttpContext = new DefaultHttpContext { User = mockClaimsPrincipal }
      }
    };

    // Act
    var result = controller.GetUserRole();

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.NotNull(okResult.Value);

    var response = okResult.Value as dynamic;
    Assert.NotNull(response);
  }
  [Fact]
  public void GetUserRole_ReturnsUnauthorized_WhenUserIsNotAuthenticated()
  {
    // Arrange
    var mockClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity()); // No identity or claims

    var controller = new AuthController(
        _mockSignInManager.Object,
        _mockUserManager.Object,
        _mockUserRepository.Object,
        _mockConfiguration.Object,
        _mockLogger.Object)
    {
      ControllerContext = new ControllerContext
      {
        HttpContext = new DefaultHttpContext { User = mockClaimsPrincipal }
      }
    };

    // Act
    var result = controller.GetUserRole();

    // Assert
    var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
    Assert.NotNull(unauthorizedResult.Value);

    var response = Assert.IsType<ApiResponse>(unauthorizedResult.Value);
    Assert.Equal("User is not authenticated.", response.Message);
  }

  [Fact]
  public async Task Login_ReturnsOk_WhenLoginIsSuccessful()
  {
    // Arrange
    var loginRequest = new LoginRequest
    {
      Username = "testuser",
      Password = "Test@1234"
    };

    var user = new ApplicationUser
    {
      Id = "1",
      UserName = "testuser"
    };

    var roles = new List<string> { "User" };

    _mockUserManager.Setup(um => um.FindByNameAsync(loginRequest.Username))
        .ReturnsAsync(user);

    _mockUserManager.Setup(um => um.CheckPasswordAsync(user, loginRequest.Password))
        .ReturnsAsync(true);

    _mockUserManager.Setup(um => um.GetRolesAsync(user))
        .ReturnsAsync(roles);

    _mockConfiguration.Setup(c => c["Jwt:Key"]).Returns("ThisIsASecretKeyWithAtLeast32Characters");
    _mockConfiguration.Setup(c => c["Jwt:ExpireMinutes"]).Returns("60");
    _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
    _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

    var controller = new AuthController(
        _mockSignInManager.Object,
        _mockUserManager.Object,
        _mockUserRepository.Object,
        _mockConfiguration.Object,
        _mockLogger.Object);

    // Act
    var result = await controller.Login(loginRequest);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.NotNull(okResult.Value);

    var newResponse = Assert.IsType<ApiResponse>(okResult.Value);
    Assert.NotNull(newResponse);
    Assert.NotNull(newResponse.Message);
  }

  [Fact]
  public async Task Login_ReturnsBadRequest_WhenModelStateIsInvalid()
  {
    // Arrange
    var loginRequest = new LoginRequest
    {
      Username = "", // Invalid username
      Password = "short" // Invalid password
    };

    var controller = new AuthController(
        _mockSignInManager.Object,
        _mockUserManager.Object,
        _mockUserRepository.Object,
        _mockConfiguration.Object,
        _mockLogger.Object);

    controller.ModelState.AddModelError("Username", "The Username field is required.");
    controller.ModelState.AddModelError("Password", "The Password must be at least 6 characters long.");

    // Act
    var result = await controller.Login(loginRequest);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    Assert.NotNull(badRequestResult.Value);

    var response = Assert.IsType<ApiResponse>(badRequestResult.Value);
    Assert.Equal("Invalid login data", response.Message);
    Assert.Contains("The Username field is required.", response.Errors);
    Assert.Contains("The Password must be at least 6 characters long.", response.Errors);
  }

  [Fact]
  public async Task Login_ReturnsUnauthorized_WhenUserNotFoundOrPasswordIncorrect()
  {
    // Arrange
    var loginRequest = new LoginRequest
    {
      Username = "testuser",
      Password = "WrongPassword"
    };

    _mockUserManager.Setup(um => um.FindByNameAsync(loginRequest.Username))
        .ReturnsAsync((ApplicationUser)null!);

    var controller = new AuthController(
        _mockSignInManager.Object,
        _mockUserManager.Object,
        _mockUserRepository.Object,
        _mockConfiguration.Object,
        _mockLogger.Object);

    // Act
    var result = await controller.Login(loginRequest);

    // Assert
    var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
    Assert.NotNull(unauthorizedResult.Value);

    var response = Assert.IsType<ApiResponse>(unauthorizedResult.Value);
    Assert.Equal("Invalid username or password", response.Message);
  }

  [Fact]
  public async Task Login_ReturnsInternalServerError_WhenJwtKeyIsMissing()
  {
    // Arrange
    var loginRequest = new LoginRequest
    {
      Username = "testuser",
      Password = "Test@1234"
    };

    var user = new ApplicationUser
    {
      Id = "1",
      UserName = "testuser"
    };

    var roles = new List<string> { "User" };

    _mockUserManager.Setup(um => um.FindByNameAsync(loginRequest.Username))
        .ReturnsAsync(user);

    _mockUserManager.Setup(um => um.CheckPasswordAsync(user, loginRequest.Password))
        .ReturnsAsync(true);

    _mockUserManager.Setup(um => um.GetRolesAsync(user))
        .ReturnsAsync(roles);

    _mockConfiguration.Setup(c => c["Jwt:Key"]).Returns((string)null!); // Missing key

    var controller = new AuthController(
        _mockSignInManager.Object,
        _mockUserManager.Object,
        _mockUserRepository.Object,
        _mockConfiguration.Object,
        _mockLogger.Object);

    // Act
    var result = await controller.Login(loginRequest);

    // Assert
    var statusCodeResult = Assert.IsType<ObjectResult>(result);
    Assert.Equal(500, statusCodeResult.StatusCode);

    var response = Assert.IsType<ApiResponse>(statusCodeResult.Value);
    Assert.NotNull(response);
    Assert.Equal("Internal server error: JWT key is not configured or is too short.", response.Message);
  }

  [Fact]
  public async Task Login_ReturnsInternalServerError_WhenRoleIsMissing()
  {
    // Arrange
    var loginRequest = new LoginRequest
    {
      Username = "testuser",
      Password = "Test@1234"
    };

    var user = new ApplicationUser
    {
      Id = "1",
      UserName = "testuser"
    };


    _mockUserManager.Setup(um => um.FindByNameAsync(loginRequest.Username))
        .ReturnsAsync(user);

    _mockUserManager.Setup(um => um.CheckPasswordAsync(user, loginRequest.Password))
        .ReturnsAsync(true);


    var controller = new AuthController(
        _mockSignInManager.Object,
        _mockUserManager.Object,
        _mockUserRepository.Object,
        _mockConfiguration.Object,
        _mockLogger.Object);

    // Act
    var result = await controller.Login(loginRequest);

    // Assert
    var statusCodeResult = Assert.IsType<ObjectResult>(result);
    Assert.Equal(500, statusCodeResult.StatusCode);

    var response = Assert.IsType<ApiResponse>(statusCodeResult.Value);
    Assert.NotNull(response);
    Assert.Equal("Internal server error", response.Message);
  }

  [Fact]
  public async Task Logout_ReturnsOk_WhenLogoutIsSuccessful()
  {
    // Arrange
    var controller = new AuthController(
        _mockSignInManager.Object,
        _mockUserManager.Object,
        _mockUserRepository.Object,
        _mockConfiguration.Object,
        _mockLogger.Object);

    // Act
    var result = await controller.Logout();

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.NotNull(okResult.Value);

    var response = Assert.IsType<ApiResponse>(okResult.Value);
    Assert.Equal("Logout successful", response.Message);
  }

  [Fact]
  public async Task GetUserDetails_ReturnsOk_WhenUserIsAuthenticated()
  {
    // Arrange
    var mockClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
     {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Role, "Admin")
    }, "mock"));

    var user = new ApplicationUser
    {
      UserName = "TestUser",
      Email = "testuser@exampåle.com",
    };

    _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
        .ReturnsAsync(user);

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext { User = mockClaimsPrincipal }
    };

    // Act
    var result = await _controller.GetUserDetails();

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.NotNull(okResult.Value);

    var response = okResult.Value as dynamic;
    Assert.NotNull(response);
  }

  [Fact]
  public async Task GetUserDetails_ReturnsUnauthorized_WhenUserIsNotAuthenticated()
  {
    // Arrange
    var mockClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity()); // No identity or claims

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext { User = mockClaimsPrincipal }
    };

    // Act
    var result = await _controller.GetUserDetails();

    // Assert
    var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
    Assert.NotNull(unauthorizedResult.Value);

    var response = Assert.IsType<ApiResponse>(unauthorizedResult.Value);
    Assert.Equal("User is not authenticated.", response.Message);
  }

  [Fact]
  public async Task ChangePassword_RetrunsOK_WhenPasswordChangeIsSuccessful()
  {
    // Arrange
    var mockClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
    {
      new Claim(ClaimTypes.Name, "TestUser")
    }, "mock"));

    var user = new ApplicationUser
    {
      UserName = "TestUser"
    };

    var request = new ChangePasswordRequest
    {
      CurrentPassword = "OldPassword123",
      NewPassword = "NewPassword123"
    };

    _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
      .ReturnsAsync(user);

    _mockUserManager.Setup(um => um.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword))
      .ReturnsAsync(IdentityResult.Success);

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext { User = mockClaimsPrincipal }
    };

    // Act
    var result = await _controller.ChangePassword(request);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.NotNull(okResult.Value);

    var response = Assert.IsType<ApiResponse>(okResult.Value);
    Assert.Equal("Password changed successfully", response.Message);
  }

  [Fact]
  public async Task ChangePassword_ReturnsBadRequest_WhenModelStateIsInvalid()
  {
    // Arrange
    var request = new ChangePasswordRequest
    {
      CurrentPassword = "", // Invalid current password
      NewPassword = "short" // Invalid new password
    };

    _controller.ModelState.AddModelError("CurrentPassword", "The CurrentPassword field is required.");
    _controller.ModelState.AddModelError("NewPassword", "The NewPassword must be at least 6 characters long.");

    // Act
    var result = await _controller.ChangePassword(request);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    Assert.NotNull(badRequestResult.Value);

    var response = Assert.IsType<ApiResponse>(badRequestResult.Value);
    Assert.Equal("Invalid password change request", response.Message);
    Assert.Contains("The CurrentPassword field is required.", response.Errors);
    Assert.Contains("The NewPassword must be at least 6 characters long.", response.Errors);
  }

  [Fact]
  public async Task ChangePassword_ReturnsUnauthorized_WhenUserNotFound()
  {
    // Arrange
    var mockClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
    {
        new Claim(ClaimTypes.Name, "TestUser")
    }, "mock"));

    var request = new ChangePasswordRequest
    {
      CurrentPassword = "OldPassword123",
      NewPassword = "NewPassword123"
    };

    _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
        .ReturnsAsync((ApplicationUser)null!); // Simuler at brukeren ikke finnes

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext { User = mockClaimsPrincipal }
    };

    // Act
    var result = await _controller.ChangePassword(request);

    // Assert
    var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
    Assert.NotNull(unauthorizedResult.Value);

    var response = Assert.IsType<ApiResponse>(unauthorizedResult.Value);
    Assert.Equal("User not found", response.Message);
  }

  [Fact]
  public async Task ChangePassword_ReturnsBadRequest_WhenPasswordChangeFails()
  {
    // Arrange
    var mockClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
    {
        new Claim(ClaimTypes.Name, "TestUser")
    }, "mock"));

    var user = new ApplicationUser
    {
      UserName = "TestUser"
    };

    var request = new ChangePasswordRequest
    {
      CurrentPassword = "OldPassword123",
      NewPassword = "NewPassword123"
    };

    var identityErrors = new List<IdentityError>
    {
        new IdentityError { Description = "Current password is incorrect." },
        new IdentityError { Description = "New password does not meet complexity requirements." }
    };

    _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
        .ReturnsAsync(user);

    _mockUserManager.Setup(um => um.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword))
        .ReturnsAsync(IdentityResult.Failed(identityErrors.ToArray()));

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext { User = mockClaimsPrincipal }
    };

    // Act
    var result = await _controller.ChangePassword(request);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    Assert.NotNull(badRequestResult.Value);

    var response = Assert.IsType<ApiResponse>(badRequestResult.Value);
    Assert.Equal("Password change failed", response.Message);
    Assert.Contains("Current password is incorrect.", response.Errors);
    Assert.Contains("New password does not meet complexity requirements.", response.Errors);
  }

  [Fact]
  public async Task DeleteAccount_ReturnsOk_WhenAccountDeletionIsSuccessful()
  {
    // Arrange
    var mockClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
    {
        new Claim(ClaimTypes.Name, "TestUser")
    }, "mock"));

    var user = new ApplicationUser
    {
      UserName = "TestUser"
    };

    _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
        .ReturnsAsync(user);

    _mockUserManager.Setup(um => um.DeleteAsync(user))
        .ReturnsAsync(IdentityResult.Success);

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext { User = mockClaimsPrincipal }
    };

    // Act
    var result = await _controller.DeleteAccount();

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.NotNull(okResult.Value);

    var response = Assert.IsType<ApiResponse>(okResult.Value);
    Assert.Equal("Account deleted successfully", response.Message);
  }

  [Fact]
  public async Task DeleteAccount_ReturnsUnauthorized_WhenUserNotFound()
  {
    // Arrange
    var mockClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
    {
        new Claim(ClaimTypes.Name, "TestUser")
    }, "mock"));

    _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
        .ReturnsAsync((ApplicationUser)null!); // Simuler at brukeren ikke finnes

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext { User = mockClaimsPrincipal }
    };

    // Act
    var result = await _controller.DeleteAccount();

    // Assert
    var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
    Assert.NotNull(unauthorizedResult.Value);

    var response = Assert.IsType<ApiResponse>(unauthorizedResult.Value);
    Assert.Equal("User not found", response.Message);
  }

  [Fact]
  public async Task DeleteAccount_ReturnsBadRequest_WhenAccountDeletionFails()
  {
    // Arrange
    var mockClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
    {
        new Claim(ClaimTypes.Name, "TestUser")
    }, "mock"));

    var user = new ApplicationUser
    {
      UserName = "TestUser"
    };

    var identityErrors = new List<IdentityError>
    {
        new IdentityError { Description = "An error occurred while deleting the account." }
    };

    _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
        .ReturnsAsync(user);

    _mockUserManager.Setup(um => um.DeleteAsync(user))
        .ReturnsAsync(IdentityResult.Failed(identityErrors.ToArray()));

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext { User = mockClaimsPrincipal }
    };

    // Act
    var result = await _controller.DeleteAccount();

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    Assert.NotNull(badRequestResult.Value);

    var response = Assert.IsType<ApiResponse>(badRequestResult.Value);
    Assert.Equal("Account deletion failed", response.Message);
    Assert.Contains("An error occurred while deleting the account.", response.Errors);
  }
}
