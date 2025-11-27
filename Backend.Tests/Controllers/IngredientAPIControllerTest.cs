using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Backend.Controllers;
using Backend.DAL;
using Backend.Models;
using Backend.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Backend.Tests;

public class IngredientAPIControllerTest
{
  private readonly Mock<IIngredientRepository> _mockIngredientRepository;
  private readonly Mock<ILogger<IngredientAPIController>> _mockLogger;
  private readonly IngredientAPIController _controller;

  public IngredientAPIControllerTest()
  {
    _mockIngredientRepository = new Mock<IIngredientRepository>();
    _mockLogger = new Mock<ILogger<IngredientAPIController>>();

    _controller = new IngredientAPIController(
        _mockIngredientRepository.Object,
        _mockLogger.Object);
  }

  [Fact]
  public async Task GetIngredients_ReturnsOkResult_WhenSuccessful()
  {
    // Arrange
    var ingredients = new List<Ingredient>
    {
        new Ingredient { IngredientId = 1, Name = "Test Ingredient 1", Description = "Description 1", FillLevel = 50, IsAvailable = true, UnitPrice = 10, CategoryId = 1, Category = new Category { Name = "Category 1" } },
        new Ingredient { IngredientId = 2, Name = "Test Ingredient 2", Description = "Description 2", FillLevel = 75, IsAvailable = true, UnitPrice = 20, CategoryId = 2, Category = new Category { Name = "Category 2" } }
    };
    _mockIngredientRepository.Setup(repo => repo.GetIngredients()).ReturnsAsync(ingredients);

    var expectedDtos = ingredients.Select(ingredient => new IngredientDTO
    {
      IngredientId = ingredient.IngredientId,
      Name = ingredient.Name,
      Description = ingredient.Description,
      FillLevel = ingredient.FillLevel.HasValue ? ingredient.FillLevel.Value : 0,
      IsAvailable = ingredient.IsAvailable,
      UnitPrice = ingredient.UnitPrice,
      CategoryId = ingredient.CategoryId ?? 0,
      CategoryName = ingredient.Category?.Name
    }).ToList();

    // Act
    var result = await _controller.GetIngredients();

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var model = Assert.IsAssignableFrom<IEnumerable<IngredientDTO>>(okResult.Value);
    Assert.Equal(ingredients.Count, model.Count());
  }

  [Fact]
  public async Task GetIngredients_ReturnsNotFound_WhenNoData()
  {
    // Arrange
    _mockIngredientRepository.Setup(repo => repo.GetIngredients()).ReturnsAsync((List<Ingredient>?)null!);

    // Act
    var result = await _controller.GetIngredients();

    // Assert
    Assert.IsType<NotFoundObjectResult>(result);
  }

  [Fact]
  public async Task GetIngredient_ReturnsOkResult_WhenSuccessful()
  {
    // Arrange
    var ingredient = new Ingredient { IngredientId = 1, Name = "Test Ingredient 1", Description = "Description 1", FillLevel = 50, IsAvailable = true, UnitPrice = 10, CategoryId = 1, Category = new Category { Name = "Category 1" } };
    _mockIngredientRepository.Setup(repo => repo.GetIngredientById(1)).ReturnsAsync(ingredient);

    var expectedDto = new IngredientDTO
    {
      IngredientId = ingredient.IngredientId,
      Name = ingredient.Name,
      Description = ingredient.Description,
      FillLevel = ingredient.FillLevel.HasValue ? ingredient.FillLevel.Value : 0,
      IsAvailable = ingredient.IsAvailable,
      UnitPrice = ingredient.UnitPrice,
      CategoryId = ingredient.CategoryId ?? 0,
      CategoryName = ingredient.Category?.Name
    };

    // Act
    var result = await _controller.GetIngredient(1);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var model = Assert.IsType<IngredientDTO>(okResult.Value);
    Assert.Equal(expectedDto.IngredientId, model.IngredientId);
    Assert.Equal(expectedDto.Name, model.Name);
    Assert.Equal(expectedDto.Description, model.Description);
    Assert.Equal(expectedDto.FillLevel, model.FillLevel);
    Assert.Equal(expectedDto.IsAvailable, model.IsAvailable);
    Assert.Equal(expectedDto.UnitPrice, model.UnitPrice);
    Assert.Equal(expectedDto.CategoryId, model.CategoryId);
    Assert.Equal(expectedDto.CategoryName, model.CategoryName);
  }

  [Fact]
  public async Task GetIngredient_ReturnsNotFound_WhenNoData()
  {
    // Arrange
    _mockIngredientRepository.Setup(repo => repo.GetIngredientById(1)).ReturnsAsync((Ingredient?)null);

    // Act
    var result = await _controller.GetIngredient(1);

    // Assert
    Assert.IsType<NotFoundObjectResult>(result);
  }

  [Fact]
  public async Task CreateIngredient_ReturnsBadRequest_WhenIngredientDTOIsNull()
  {
    // Arrange
    // IngredientDTO? ingredientDTO = null;

    // Act
    var result = await _controller.CreateIngredient((IngredientDTO)null!);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    Assert.Equal("Ingredient is null", badRequestResult.Value);
  }

  [Fact]
  public async Task CreateIngredient_ReturnCreatedAtAction_WhenSuccessful()
  {
    // Arrange
    var ingredientDTO = new IngredientDTO
    {
      Name = "Test Ingredient 1",
      Description = "Description 1",
      FillLevel = 50,
      IsAvailable = true,
      UnitPrice = 10,
      CategoryId = 1
    };

    var ingredient = new Ingredient
    {
      Name = ingredientDTO.Name,
      Description = ingredientDTO.Description,
      FillLevel = ingredientDTO.FillLevel,
      IsAvailable = ingredientDTO.IsAvailable,
      UnitPrice = ingredientDTO.UnitPrice,
      CategoryId = ingredientDTO.CategoryId
    };

    _mockIngredientRepository.Setup(repo => repo.Create(It.IsAny<Ingredient>())).ReturnsAsync(true);
    _mockIngredientRepository.Setup(repo => repo.GetIngredientById(ingredient.IngredientId)).ReturnsAsync(ingredient);

    // Act
    var result = await _controller.CreateIngredient(ingredientDTO);

    // Assert
    var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
    var model = Assert.IsType<IngredientDTO>(createdAtActionResult.Value);
  }

  [Fact]
  public async Task CreateIngredient_ReturnsNotFound_WhenIngredientNotFoundAfterCreation()
  {
    // Arrange
    var ingredientDTO = new IngredientDTO
    {
      Name = "Test Ingredient 1",
      Description = "Description 1",
      FillLevel = 50,
      IsAvailable = true,
      UnitPrice = 10,
      CategoryId = 1
    };

    _mockIngredientRepository.Setup(repo => repo.Create(It.IsAny<Ingredient>())).ReturnsAsync(true);
    _mockIngredientRepository.Setup(repo => repo.GetIngredientById(1)).ReturnsAsync((Ingredient?)null);

    // Act
    var result = await _controller.CreateIngredient(ingredientDTO);

    // Assert
    var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    Assert.Equal("Ingredient not found", notFoundResult.Value);
  }

  [Fact]
  public async Task CreateIngredient_ReturnsInteralServerError_WhenCreationFails()
  {
    // Arrange
    var ingredientDTO = new IngredientDTO
    {
      Name = "Test Ingredient 1",
      Description = "Description 1",
      FillLevel = 50,
      IsAvailable = true,
      UnitPrice = 10,
      CategoryId = 1
    };

    _mockIngredientRepository.Setup(repo => repo.Create(It.IsAny<Ingredient>())).ReturnsAsync(false);

    // Act
    var result = await _controller.CreateIngredient(ingredientDTO);

    // Assert
    var statusCodeResult = Assert.IsType<ObjectResult>(result);
    Assert.Equal(500, statusCodeResult.StatusCode);
    Assert.Equal("Internal server error", statusCodeResult.Value);
  }

  [Fact]
  public async Task CreateIngredient_HandlesImageUploadSuccessfully()
  {
    // Arrange
    var mockFile = new Mock<IFormFile>();
    var content = "Fake file content";
    var fileName = "test.jpg";
    var ms = new MemoryStream();
    var writer = new StreamWriter(ms);
    writer.Write(content);
    writer.Flush();
    ms.Position = 0;

    mockFile.Setup(f => f.OpenReadStream()).Returns(ms);
    mockFile.Setup(f => f.FileName).Returns(fileName);
    mockFile.Setup(f => f.Length).Returns(ms.Length);

    var ingredientDTO = new IngredientDTO
    {
      Name = "Test Ingredient 1",
      Description = "Description 1",
      FillLevel = 50,
      IsAvailable = true,
      UnitPrice = 10,
      CategoryId = 1,
      ImagePath = "/ingredient-images/test.jpg",
      ImageFile = mockFile.Object
    };

    var ingredient = new Ingredient
    {
      Name = ingredientDTO.Name,
      Description = ingredientDTO.Description,
      FillLevel = ingredientDTO.FillLevel,
      IsAvailable = ingredientDTO.IsAvailable,
      UnitPrice = ingredientDTO.UnitPrice,
      CategoryId = ingredientDTO.CategoryId,
      ImagePath = ingredientDTO.ImagePath,
    };

    _mockIngredientRepository.Setup(repo => repo.Create(It.IsAny<Ingredient>())).ReturnsAsync(true);
    _mockIngredientRepository.Setup(repo => repo.GetIngredientById(It.IsAny<int>())).ReturnsAsync(ingredient);

    var tempFolder = Path.Combine(Path.GetTempPath(), "ingredient-images");
    Directory.CreateDirectory(tempFolder);

    // Act
    var result = await _controller.CreateIngredient(ingredientDTO);

    // Assert
    var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
    Assert.Equal(nameof(_controller.GetIngredient), createdAtActionResult.ActionName);
    Assert.NotNull(createdAtActionResult.RouteValues);

    var response = Assert.IsType<IngredientDTO>(createdAtActionResult.Value);
    Assert.NotNull(response.ImagePath);
    Assert.StartsWith("/ingredient-images/", response.ImagePath);

    var savedFilePath = Path.Combine(tempFolder, Path.GetFileName(response.ImagePath));
    Console.WriteLine($"Expected file path: {savedFilePath}");

    // Clean up
    if (File.Exists(savedFilePath))
    {
      File.Delete(savedFilePath);
    }
    Directory.Delete(tempFolder);
  }

  [Fact]
  public async Task UpdateIngredient_ReturnsBadRequest_WhenIngredientDTOIsNull()
  {
    // Arrange
    _controller.ModelState.AddModelError("IngredientDTO", "Required");

    // Act
    var result = await _controller.UpdateIngredient(1, null!);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    Assert.Equal("Invalid request: Ingredient is null or ID mismatch", badRequestResult.Value);
  }

  [Fact]
  public async Task UpdateCategory_HandlesImageUploadCorrectly()
  {
    // Arrange
    var mockFile = new Mock<IFormFile>();
    var content = "Fake file content";
    var fileName = "test.jpg";
    var ms = new MemoryStream();
    var writer = new StreamWriter(ms);
    writer.Write(content);
    writer.Flush();
    ms.Position = 0;

    mockFile.Setup(f => f.OpenReadStream()).Returns(ms);
    mockFile.Setup(f => f.FileName).Returns(fileName);
    mockFile.Setup(f => f.Length).Returns(ms.Length);

    var ingredientDTO = new IngredientDTO
    {
      IngredientId = 1,
      Name = "Test Ingredient 1",
      Description = "Description 1",
      FillLevel = 50,
      IsAvailable = true,
      UnitPrice = 10,
      CategoryId = 1,
      ImagePath = "/ingredient-images/test.jpg",
      ImageFile = mockFile.Object
    };

    var ingredient = new Ingredient
    {
      IngredientId = ingredientDTO.IngredientId,
      Name = ingredientDTO.Name,
      Description = ingredientDTO.Description,
      FillLevel = ingredientDTO.FillLevel,
      IsAvailable = ingredientDTO.IsAvailable,
      UnitPrice = ingredientDTO.UnitPrice,
      CategoryId = ingredientDTO.CategoryId,
      ImagePath = ingredientDTO.ImagePath,
    };

    _mockIngredientRepository.Setup(repo => repo.Update(It.IsAny<Ingredient>())).ReturnsAsync(true);
    _mockIngredientRepository.Setup(repo => repo.GetIngredientById(It.IsAny<int>())).ReturnsAsync(ingredient);

    var tempFolder = Path.Combine(Path.GetTempPath(), "ingredient-images");
    Directory.CreateDirectory(tempFolder);

    // Act
    var result = await _controller.UpdateIngredient(1, ingredientDTO);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var response = Assert.IsType<Ingredient>(okResult.Value);
    Assert.NotNull(response.ImagePath);
    Assert.StartsWith("/ingredient-images/", response.ImagePath);

    var savedFilePath = Path.Combine(tempFolder, Path.GetFileName(response.ImagePath));
    Console.WriteLine($"Expected file path: {savedFilePath}");

    // Clean up
    if (File.Exists(savedFilePath))
    {
      File.Delete(savedFilePath);
    }
    Directory.Delete(tempFolder);
  }

  [Fact]
  public async Task UpdateIngredient_RetainsExistingImagePath_WhenNoNewImageUploaded()
  {
    // Arrange
    var ingredientDTO = new IngredientDTO
    {
      IngredientId = 1,
      Name = "Test Ingredient 1",
      Description = "Description 1",
      FillLevel = 50,
      IsAvailable = true,
      UnitPrice = 10,
      CategoryId = 1,
      ImagePath = "/ingredient-images/test.jpg"
    };

    var ingredient = new Ingredient
    {
      IngredientId = ingredientDTO.IngredientId,
      Name = ingredientDTO.Name,
      Description = ingredientDTO.Description,
      FillLevel = ingredientDTO.FillLevel,
      IsAvailable = ingredientDTO.IsAvailable,
      UnitPrice = ingredientDTO.UnitPrice,
      CategoryId = ingredientDTO.CategoryId,
      ImagePath = ingredientDTO.ImagePath,
    };

    _mockIngredientRepository.Setup(repo => repo.Update(It.IsAny<Ingredient>())).ReturnsAsync(true);
    _mockIngredientRepository.Setup(repo => repo.GetIngredientById(It.IsAny<int>())).ReturnsAsync(ingredient);

    // Act
    var result = await _controller.UpdateIngredient(1, ingredientDTO);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var response = Assert.IsType<Ingredient>(okResult.Value);
    Assert.Equal(ingredientDTO.ImagePath, response.ImagePath);
  }

  [Fact]
  public async Task UpdateIngredient_Returns500ServerError_WhenUpdateFails()
  {
    // Arrange
    var ingredientDTO = new IngredientDTO
    {
      IngredientId = 1,
      Name = "Test Ingredient 1",
      Description = "Description 1",
      FillLevel = 50,
      IsAvailable = true,
      UnitPrice = 10,
      CategoryId = 1
    };

    _mockIngredientRepository.Setup(repo => repo.Update(It.IsAny<Ingredient>())).ReturnsAsync(false);

    // Act
    var result = await _controller.UpdateIngredient(1, ingredientDTO);

    // Assert
    var statusCodeResult = Assert.IsType<ObjectResult>(result);
    Assert.Equal(500, statusCodeResult.StatusCode);
    Assert.Equal("Internal server error", statusCodeResult.Value);
  }

  [Fact]
  public async Task UpdateIngredient_ReturnsNotFoundResult_WhenGetIngredientByIdFails()
  {
    // Arrange
    var ingredientDTO = new IngredientDTO
    {
      IngredientId = 1,
      Name = "Test Ingredient 1",
      Description = "Description 1",
      FillLevel = 50,
      IsAvailable = true,
      UnitPrice = 10,
      CategoryId = 1
    };

    var ingredient = new Ingredient
    {
      IngredientId = ingredientDTO.IngredientId,
      Name = ingredientDTO.Name,
      Description = ingredientDTO.Description,
      FillLevel = ingredientDTO.FillLevel,
      IsAvailable = ingredientDTO.IsAvailable,
      UnitPrice = ingredientDTO.UnitPrice,
      CategoryId = ingredientDTO.CategoryId
    };

    _mockIngredientRepository.Setup(repo => repo.Update(It.IsAny<Ingredient>())).ReturnsAsync(true);
    _mockIngredientRepository.Setup(repo => repo.GetIngredientById(1)).ReturnsAsync((Ingredient?)null);

    // Act
    var result = await _controller.UpdateIngredient(1, ingredientDTO);

    // Assert
    var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    Assert.Equal("Ingredient not found", notFoundResult.Value);
  }

  [Fact]
  public async Task DeleteIngredient_ReturnsOkResult_WhenSuccessful()
  {
    // Arrange
    _mockIngredientRepository.Setup(repo => repo.DeleteIngredient(1)).ReturnsAsync(true);

    // Act
    var result = await _controller.DeleteIngredient(1);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.Equal("Ingredient deleted", okResult.Value);
  }

  [Fact]
  public async Task DeleteIngredient_ReturnsInternalServerError_WhenDeleteFails()
  {
    // Arrange
    _mockIngredientRepository.Setup(repo => repo.DeleteIngredient(1)).ReturnsAsync(false);

    // Act
    var result = await _controller.DeleteIngredient(1);

    // Assert
    var statusCodeResult = Assert.IsType<ObjectResult>(result);
    Assert.Equal(500, statusCodeResult.StatusCode);
    Assert.Equal("Internal server error", statusCodeResult.Value);
  }
}