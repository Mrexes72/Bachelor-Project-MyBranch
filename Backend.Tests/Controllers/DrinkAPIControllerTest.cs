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
using System.Linq;

namespace Backend.Tests;

public class DrinkAPIControllerTest
{
  private readonly Mock<IDrinkRepository> _mockDrinkRepository;
  private readonly Mock<ILogger<DrinkAPIController>> _mockLogger;
  private readonly DrinkAPIController _controller;

  public DrinkAPIControllerTest()
  {
    _mockDrinkRepository = new Mock<IDrinkRepository>();
    _mockLogger = new Mock<ILogger<DrinkAPIController>>();

    _controller = new DrinkAPIController(
        _mockDrinkRepository.Object,
        _mockLogger.Object);
  }

  [Fact]
  public async Task GetDrinks_ReturnsOkResult_WhenSuccessful()
  {
    // Arrange
    var drinks = new List<Drink>
    {
        new Drink { DrinkId = 1, Name = "Test Drink 1",  BasePrice = 10, SalePrice=65, TimesFavorite=1, CategoryId = 1, Category = new Category { Name = "Category 1" } },
        new Drink { DrinkId = 2, Name = "Test Drink 2", BasePrice = 20, SalePrice=75, TimesFavorite=2, CategoryId = 2, Category = new Category { Name = "Category 2" } }
    };
    _mockDrinkRepository.Setup(repo => repo.GetDrinks()).ReturnsAsync(drinks);

    var expectedDtos = drinks.Select(drink => new DrinkDTO
    {
      DrinkId = drink.DrinkId,
      Name = drink.Name,
      BasePrice = drink.BasePrice,
      SalePrice = drink.SalePrice,
      TimesFavorite = drink.TimesFavorite,
      CategoryId = drink.CategoryId ?? 0,
    }).ToList();

    // Act
    var result = await _controller.GetDrinks();

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.NotNull(okResult);
    var actualDtos = Assert.IsAssignableFrom<IEnumerable<DrinkDTO>>(okResult.Value);
    Assert.NotNull(actualDtos);
    Assert.Equal(expectedDtos.Count(), actualDtos.Count());
  }

  [Fact]
  public async Task GetDrinks_ReturnsNotFoundResult_WhenDrinksIsNull()
  {
    // Arrange
    _mockDrinkRepository.Setup(repo => repo.GetDrinks()).ReturnsAsync((List<Drink>?)null!);

    // Act
    var result = await _controller.GetDrinks();

    // Assert
    var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
  }

  [Fact]
  public async Task GetDrink_ReturnsOkResult_WhenSuccessful()
  {
    // Arrange
    var drink = new Drink { DrinkId = 1, Name = "Test Drink 1", BasePrice = 10, SalePrice = 65, TimesFavorite = 1, CategoryId = 1, Category = new Category { Name = "Category 1" } };
    _mockDrinkRepository.Setup(repo => repo.GetDrinkById(1)).ReturnsAsync(drink);

    var expectedDto = new DrinkDTO
    {
      DrinkId = drink.DrinkId,
      Name = drink.Name,
      BasePrice = drink.BasePrice,
      SalePrice = drink.SalePrice,
      TimesFavorite = drink.TimesFavorite,
      CategoryId = drink.CategoryId ?? 0,
    };

    // Act
    var result = await _controller.GetDrink(1);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.NotNull(okResult);
    var actualDto = Assert.IsType<DrinkDTO>(okResult.Value);
    Assert.NotNull(actualDto);
    Assert.Equal(expectedDto.DrinkId, actualDto.DrinkId);
    Assert.Equal(expectedDto.Name, actualDto.Name);
    Assert.Equal(expectedDto.BasePrice, actualDto.BasePrice);
    Assert.Equal(expectedDto.SalePrice, actualDto.SalePrice);
    Assert.Equal(expectedDto.TimesFavorite, actualDto.TimesFavorite);
    Assert.Equal(expectedDto.CategoryId, actualDto.CategoryId);
  }

  [Fact]
  public async Task GetDrink_ReturnsNotFoundResult_WhenDrinkIsNull()
  {
    // Arrange
    _mockDrinkRepository.Setup(repo => repo.GetDrinkById(1)).ReturnsAsync((Drink?)null!);

    // Act
    var result = await _controller.GetDrink(1);

    // Assert
    var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
  }

  [Fact]
  public async Task CreateDrink_ReturnsBadRequestResult_WhenDrinkIsNull()
  {
    // Arrange
    _controller.ModelState.AddModelError("Name", "Name is required");

    // Act
    var result = await _controller.CreateDrink(null!);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    Assert.NotNull(badRequestResult.Value);
    Assert.Equal("Received drink data is null", badRequestResult.Value);
  }

  [Fact]
  public async Task CreateDrink_ReturnsBadRequestResult_WhenDrinkNameIsEmpty()
  {
    // Arrange
    var drink = new DrinkDTO { Name = "", BasePrice = 10, SalePrice = 65, TimesFavorite = 1, CategoryId = 1 };

    // Act
    var result = await _controller.CreateDrink(drink);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    Assert.NotNull(badRequestResult.Value);
    Assert.Equal("Drink name is required", badRequestResult.Value);
  }

  [Fact]
  public async Task CreateDrink_ReturnsBadRequestResult_WhenIngredientDTOsIsNull()
  {
    // Arrange
    var drink = new DrinkDTO { Name = "Test Drink 1", BasePrice = 10, SalePrice = 65, TimesFavorite = 1, CategoryId = 1, IngredientDTOs = null! };

    // Act
    var result = await _controller.CreateDrink(drink);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    Assert.NotNull(badRequestResult.Value);
    Assert.Equal("At least one ingredient is required", badRequestResult.Value);
  }

  [Fact]
  public async Task CreateDrink_ReturnsBadRequestResult_WhenDrinkCretaionFails()
  {
    // Arrange
    var drink = new DrinkDTO
    {
      Name = "Test Drink 1",
      BasePrice = 10,
      SalePrice = 65,
      TimesFavorite = 1,
      CategoryId = 1,
      IngredientDTOs = new List<IngredientDTO>
      {
        new IngredientDTO { IngredientId = 1, Name = "Ingredient 1", FillLevel = 50, IsAvailable = true, UnitPrice = 10, CategoryId = 1 },
      }
    };

    _mockDrinkRepository.Setup(repo => repo.Create(It.IsAny<Drink>())).ReturnsAsync(false);

    // Act
    var result = await _controller.CreateDrink(drink);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    Assert.NotNull(badRequestResult.Value);
    Assert.Equal("Drink creation failed", badRequestResult.Value);
  }

  [Fact]
  public async Task CreateDrink_ReturnsOkResult_WhenSuccessful()
  {
    // Arrange
    var drink = new DrinkDTO
    {
      Name = "Test Drink 1",
      BasePrice = 10,
      SalePrice = 65,
      TimesFavorite = 1,
      CategoryId = 1,
      IngredientDTOs = new List<IngredientDTO>
      {
        new IngredientDTO { IngredientId = 1, Name = "Ingredient 1", FillLevel = 50, IsAvailable = true, UnitPrice = 10, CategoryId = 1 },
      }
    };

    var existingIngredients = new List<Ingredient>
    {
      new Ingredient { IngredientId = 1, Name = "Ingredient 1", FillLevel = 50, IsAvailable = true, UnitPrice = 10, CategoryId = 1 }
    };

    _mockDrinkRepository.Setup(repo => repo.GetIngredientsByIds(It.IsAny<List<int>>())).ReturnsAsync(existingIngredients);
    _mockDrinkRepository.Setup(repo => repo.Create(It.IsAny<Drink>())).ReturnsAsync(true);

    // Act
    var result = await _controller.CreateDrink(drink);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.NotNull(okResult);
  }

  [Fact]
  public async Task UpdateDrink_ReturnsBadRequestResult_WhenDrinkIsNull()
  {
    // Arrange
    _controller.ModelState.AddModelError("Name", "Name is required");

    // Act
    var result = await _controller.UpdateDrink(1, null!);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    Assert.NotNull(badRequestResult.Value);
    Assert.Equal("Drink is null", badRequestResult.Value);
  }

  [Fact]
  public async Task UpdateDrink_ReturnsBadRequestResult_WhenDrinkUpdateFails()
  {
    // Arrange
    var drink = new DrinkDTO
    {
      DrinkId = 1,
      Name = "Test Drink 1",
      BasePrice = 10,
      SalePrice = 65,
      TimesFavorite = 1,
      CategoryId = 1,
      IngredientDTOs = new List<IngredientDTO>
      {
        new IngredientDTO { IngredientId = 1, Name = "Ingredient 1", FillLevel = 50, IsAvailable = true, UnitPrice = 10, CategoryId = 1 },
      }
    };

    _mockDrinkRepository.Setup(repo => repo.GetDrinkById(1)).ReturnsAsync(new Drink());
    _mockDrinkRepository.Setup(repo => repo.Update(It.IsAny<Drink>())).ReturnsAsync(false);

    // Act
    var result = await _controller.UpdateDrink(1, drink);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    Assert.NotNull(badRequestResult.Value);
    Assert.Equal("Drink update failed", badRequestResult.Value);
  }

  [Fact]
  public async Task UpdateDrink_ReturnsOkResult_WhenSuccessful()
  {
    // Arrange
    var drink = new DrinkDTO
    {
      DrinkId = 1,
      Name = "Test Drink 1",
      BasePrice = 10,
      SalePrice = 65,
      TimesFavorite = 1,
      CategoryId = 1,
      IngredientDTOs = new List<IngredientDTO>
      {
        new IngredientDTO { IngredientId = 1, Name = "Ingredient 1", FillLevel = 50, IsAvailable = true, UnitPrice = 10, CategoryId = 1 },
      }
    };

    var existingDrink = new Drink
    {
      DrinkId = 1,
      Name = "Existing Drink",
      BasePrice = 10,
      SalePrice = 65,
      TimesFavorite = 1,
      CategoryId = 1,
      Ingredients = new List<Ingredient>
        {
            new Ingredient { IngredientId = 1, Name = "Ingredient 1", FillLevel = 50, IsAvailable = true, UnitPrice = 10, CategoryId = 1 }
        }
    };

    var existingIngredients = new List<Ingredient>
    {
        new Ingredient { IngredientId = 1, Name = "Ingredient 1", FillLevel = 50, IsAvailable = true, UnitPrice = 10, CategoryId = 1 }
    };

    _mockDrinkRepository.Setup(repo => repo.GetDrinkById(1)).ReturnsAsync(existingDrink);
    _mockDrinkRepository.Setup(repo => repo.GetIngredientsByIds(It.IsAny<List<int>>())).ReturnsAsync(existingIngredients);
    _mockDrinkRepository.Setup(repo => repo.Update(It.IsAny<Drink>())).ReturnsAsync(true);

    // Act
    var result = await _controller.UpdateDrink(1, drink);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.NotNull(okResult);
    Assert.Equal("Drink updated", okResult.Value);
  }

  [Fact]
  public async Task DeleteDrink_ReturnsStatusCode_WhenDeletionFails()
  {
    // Arrange
    _mockDrinkRepository.Setup(repo => repo.DeleteDrink(1)).ReturnsAsync(false);

    // Act
    var result = await _controller.DeleteDrink(1);

    // Assert
    var statusCodeResult = Assert.IsType<ObjectResult>(result);
    Assert.Equal(500, statusCodeResult.StatusCode);
  }

  [Fact]
  public async Task DeleteDrink_ReturnsOk_WhenDeletionIsSuccessful()
  {
    // Arrange
    _mockDrinkRepository.Setup(repo => repo.DeleteDrink(1)).ReturnsAsync(true);

    // Act
    var result = await _controller.DeleteDrink(1);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.NotNull(okResult);
    Assert.Equal("Drink deleted", okResult.Value);
  }

  [Fact]
  public async Task UpvoteDrink_ReturnsNotFoundObjectResult_WhenDrinkIsNull()
  {
    // Arrange
    _mockDrinkRepository.Setup(repo => repo.GetDrinkById(1)).ReturnsAsync((Drink?)null!);

    // Act
    var result = await _controller.UpvoteDrink(1);

    // Assert
    var badRequestResult = Assert.IsType<NotFoundObjectResult>(result);
    Assert.NotNull(badRequestResult.Value);
    Assert.Equal("Drink not found", badRequestResult.Value);
  }

  [Fact]
  public async Task UpvoteDrink_ReturnsStatusCode_WhenUpvoteFails()
  {
    // Arrange
    var drink = new Drink { DrinkId = 1, Name = "Test Drink 1", BasePrice = 10, SalePrice = 65, TimesFavorite = 1, CategoryId = 1, Category = new Category { Name = "Category 1" } };
    _mockDrinkRepository.Setup(repo => repo.GetDrinkById(1)).ReturnsAsync(drink);
    _mockDrinkRepository.Setup(repo => repo.Update(It.IsAny<Drink>())).ReturnsAsync(false);

    // Act
    var result = await _controller.UpvoteDrink(1);

    // Assert
    var statusCodeResult = Assert.IsType<ObjectResult>(result);
    Assert.Equal(500, statusCodeResult.StatusCode);
  }

  [Fact]
  public async Task UpvoteDrink_ReturnsOk_WhenUpvoteIsSuccessful()
  {
    // Arrange
    var drink = new Drink { DrinkId = 1, Name = "Test Drink 1", BasePrice = 10, SalePrice = 65, TimesFavorite = 1, CategoryId = 1, Category = new Category { Name = "Category 1" } };
    _mockDrinkRepository.Setup(repo => repo.GetDrinkById(1)).ReturnsAsync(drink);
    _mockDrinkRepository.Setup(repo => repo.Update(It.IsAny<Drink>())).ReturnsAsync(true);

    // Act
    var result = await _controller.UpvoteDrink(1);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.NotNull(okResult);
  }

  [Fact]
  public async Task RemoveUpvote_ReturnsNotFoundObjectResult_WhenDrinkIsNull()
  {
    // Arrange
    _mockDrinkRepository.Setup(repo => repo.GetDrinkById(1)).ReturnsAsync((Drink?)null!);

    // Act
    var result = await _controller.RemoveUpvote(1);

    // Assert
    var badRequestResult = Assert.IsType<NotFoundObjectResult>(result);
    Assert.NotNull(badRequestResult.Value);
    Assert.Equal("Drink not found", badRequestResult.Value);
  }

  [Fact]
  public async Task RemoveUpvote_ReturnsStatusCode_WhenUpvoteFails()
  {
    // Arrange
    var drink = new Drink { DrinkId = 1, Name = "Test Drink 1", BasePrice = 10, SalePrice = 65, TimesFavorite = 1, CategoryId = 1, Category = new Category { Name = "Category 1" } };
    _mockDrinkRepository.Setup(repo => repo.GetDrinkById(1)).ReturnsAsync(drink);
    _mockDrinkRepository.Setup(repo => repo.Update(It.IsAny<Drink>())).ReturnsAsync(false);

    // Act
    var result = await _controller.RemoveUpvote(1);

    // Assert
    var statusCodeResult = Assert.IsType<ObjectResult>(result);
    Assert.Equal(500, statusCodeResult.StatusCode);
  }

  [Fact]
  public async Task RemoveUpvote_ReturnsOk_WhenRemoveUpvoteIsSuccessful()
  {
    // Arrange
    var drink = new Drink { DrinkId = 1, Name = "Test Drink 1", BasePrice = 10, SalePrice = 65, TimesFavorite = 1, CategoryId = 1, Category = new Category { Name = "Category 1" } };
    _mockDrinkRepository.Setup(repo => repo.GetDrinkById(1)).ReturnsAsync(drink);
    _mockDrinkRepository.Setup(repo => repo.Update(It.IsAny<Drink>())).ReturnsAsync(true);

    // Act
    var result = await _controller.RemoveUpvote(1);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.NotNull(okResult);
  }
}
