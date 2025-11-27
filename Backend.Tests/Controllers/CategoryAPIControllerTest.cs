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

namespace Backend.Tests;

public class CategoryAPIControllerTest
{
  private readonly Mock<ICategoryRepository> _mockCategoryRepository;
  private readonly Mock<ILogger<CategoryAPIController>> _mockLogger;
  private readonly CategoryAPIController _controller;

  public CategoryAPIControllerTest()
  {
    _mockCategoryRepository = new Mock<ICategoryRepository>();
    _mockLogger = new Mock<ILogger<CategoryAPIController>>();

    _controller = new CategoryAPIController(
        _mockCategoryRepository.Object,
        _mockLogger.Object);
  }

  [Fact]
  public async Task GetCategories_ReturnsOkResult_WhenSuccessful()
  {
    // Arrange
    var categories = new List<Category>
    {
        new Category { CategoryId = 1, Name = "Category 1" },
        new Category { CategoryId = 2, Name = "Category 2" }
    };
    _mockCategoryRepository.Setup(repo => repo.GetCategories()).ReturnsAsync(categories);

    var expectedDtos = categories.Select(category => new CategoryDTO
    {
      CategoryId = category.CategoryId,
      Name = category.Name
    }).ToList();

    // Act
    var result = await _controller.GetCategories();

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var actualDtos = Assert.IsAssignableFrom<IEnumerable<CategoryDTO>>(okResult.Value);
    Assert.Equal(expectedDtos.Count(), actualDtos.Count());
  }

  [Fact]
  public async Task GetCategories_ReturnsNotFoundResult_WhenCategoriesIsNull()
  {
    // Arrange
    _mockCategoryRepository.Setup(repo => repo.GetCategories()).ReturnsAsync((List<Category>?)null!);

    // Act
    var result = await _controller.GetCategories();

    // Assert
    var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    Assert.Equal("Category list not found", notFoundResult.Value);
  }

  [Fact]
  public async Task GetCategory_ReturnsOkResult_WhenCategoryIsFound()
  {
    // Arrange
    var category = new Category { CategoryId = 1, Name = "Category 1" };
    _mockCategoryRepository.Setup(repo => repo.GetCategoryById(1)).ReturnsAsync(category);

    var expectedDto = new CategoryDTO
    {
      CategoryId = category.CategoryId,
      Name = category.Name
    };

    // Act
    var result = await _controller.GetCategory(1);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var actualDto = Assert.IsType<CategoryDTO>(okResult.Value);
    Assert.Equal(expectedDto.CategoryId, actualDto.CategoryId);
    Assert.Equal(expectedDto.Name, actualDto.Name);
  }

  [Fact]
  public async Task GetCategory_ReturnsNotFoundResult_WhenCategoryIsNull()
  {
    // Arrange
    _mockCategoryRepository.Setup(repo => repo.GetCategoryById(1)).ReturnsAsync((Category?)null!);

    // Act
    var result = await _controller.GetCategory(1);

    // Assert
    var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    Assert.Equal("Category not found", notFoundResult.Value);
  }

  [Fact]
  public async Task CreateCategory_ReturnsBadRequest_WhenCategoryDtoIsNull()
  {
    // Arrange
    _controller.ModelState.AddModelError("Name", "Name is required");

    // Act
    var result = await _controller.CreateCategory(null!);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    Assert.Equal("Category is null", badRequestResult.Value);
  }

  [Fact]
  public async Task CreateCategory_ReturnsOkResult_WhenCategoryIsCreated()
  {
    // Arrange
    var categoryDto = new CategoryDTO
    {
      Name = "Test Category",
      Description = "Test Description"
    };

    var category = new Category
    {
      CategoryId = 1,
      Name = "Test Category",
      Description = "Test Description"
    };

    _mockCategoryRepository.Setup(repo => repo.Create(It.IsAny<Category>())).ReturnsAsync(true);

    // Act
    var result = await _controller.CreateCategory(categoryDto);

    // Assert
    var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
    Assert.Equal(nameof(_controller.GetCategory), createdAtActionResult.ActionName);
    Assert.NotNull(createdAtActionResult.RouteValues);
    Assert.Equal(0, createdAtActionResult.RouteValues["id"]);
    var response = Assert.IsType<CategoryDTO>(createdAtActionResult.Value);
    Assert.Equal(category.Name, response.Name);
    Assert.Equal(category.Description, response.Description);
  }

  [Fact]
  public async Task CreateCategory_ReturnsStatusCode500_WhenCategoryIsNotCreated()
  {
    // Arrange
    var categoryDto = new CategoryDTO
    {
      Name = "Test Category",
      Description = "Test Description"
    };

    _mockCategoryRepository.Setup(repo => repo.Create(It.IsAny<Category>())).ReturnsAsync(false);

    // Act
    var result = await _controller.CreateCategory(categoryDto);

    // Assert
    var statusCodeResult = Assert.IsType<ObjectResult>(result);
    Assert.Equal(500, statusCodeResult.StatusCode);
    Assert.Equal("Internal server error", statusCodeResult.Value);
  }

  [Fact]
  public async Task UpdateCategory_ReturnsBadRequestResult_WhenCategoryDtoIsNull()
  {
    // Arrange
    _controller.ModelState.AddModelError("Name", "Name is required");

    // Act
    var result = await _controller.UpdateCategory(1, null!);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    Assert.Equal("Category is null or id does not match", badRequestResult.Value);
  }

  [Fact]
  public async Task UpdateCategory_ReturnsOkResult_WhenUpdateIsSuccessful()
  {
    // Arrange
    var categoryDto = new CategoryDTO
    {
      CategoryId = 1,
      Name = "Test Category",
      Description = "Test Description"
    };

    var category = new Category
    {
      CategoryId = 1,
      Name = "Test Category",
      Description = "Test Description"
    };

    _mockCategoryRepository.Setup(repo => repo.Update(It.IsAny<Category>())).ReturnsAsync(true);

    // Act
    var result = await _controller.UpdateCategory(1, categoryDto);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var response = Assert.IsType<CategoryDTO>(okResult.Value);
    Assert.Equal(category.Name, response.Name);
    Assert.Equal(category.Description, response.Description);
  }

  [Fact]
  public async Task UpdateCategory_Returns500StatusCode_WhenUpdateIsNotSuccessful()
  {
    // Arrange
    var categoryDto = new CategoryDTO
    {
      CategoryId = 1,
      Name = "Test Category",
      Description = "Test Description"
    };

    _mockCategoryRepository.Setup(repo => repo.Update(It.IsAny<Category>())).ReturnsAsync(false);

    // Act
    var result = await _controller.UpdateCategory(1, categoryDto);

    // Assert
    var statusCodeResult = Assert.IsType<ObjectResult>(result);
    Assert.Equal(500, statusCodeResult.StatusCode);
    Assert.Equal("Internal server error", statusCodeResult.Value);
  }

  [Fact]
  public async Task DeleteCategory_ReturnsOkResult_WhenDeleteIsSuccessful()
  {
    // Arrange
    _mockCategoryRepository.Setup(repo => repo.DeleteCategory(1)).ReturnsAsync(true);

    // Act
    var result = await _controller.DeleteCategory(1);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.Equal("Category deleted", okResult.Value);
  }

  [Fact]
  public async Task DeleteCategory_Returns500StatusCode_WhenDeleteIsNotSuccessful()
  {
    // Arrange
    _mockCategoryRepository.Setup(repo => repo.DeleteCategory(1)).ReturnsAsync(false);

    // Act
    var result = await _controller.DeleteCategory(1);

    // Assert
    var statusCodeResult = Assert.IsType<ObjectResult>(result);
    Assert.Equal(500, statusCodeResult.StatusCode);
    Assert.Equal("Internal server error", statusCodeResult.Value);
  }
}