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

public class MenuItemAPIControllerTest
{
  private readonly Mock<IMenuItemRepository> _mockMenuItemRepository;
  private readonly Mock<ILogger<MenuItemAPIController>> _mockLogger;
  private readonly MenuItemAPIController _controller;

  public MenuItemAPIControllerTest()
  {
    _mockMenuItemRepository = new Mock<IMenuItemRepository>();
    _mockLogger = new Mock<ILogger<MenuItemAPIController>>();

    _controller = new MenuItemAPIController(
        _mockMenuItemRepository.Object,
        _mockLogger.Object);
  }

  [Fact]
  public async Task GetMenuItems_ReturnsOkResult_WhenSuccessful()
  {
    // Arrange
    var menuItems = new List<MenuItem>
    {
        new MenuItem { MenuItemId = 1, Name = "Test MenuItem 1", Description = "Description 1", Price=65, IsAvailable = true, CategoryId = 1, Category = new Category { Name = "Category 1" } },
        new MenuItem { MenuItemId = 2, Name = "Test MenuItem 2", Description = "Description 2", Price=75, IsAvailable = true, CategoryId = 2, Category = new Category { Name = "Category 2" } }
    };
    _mockMenuItemRepository.Setup(repo => repo.GetMenuItems()).ReturnsAsync(menuItems);

    var expectedDtos = menuItems.Select(menuItem => new MenuItemDTO
    {
      MenuItemId = menuItem.MenuItemId,
      Name = menuItem.Name,
      Description = menuItem.Description,
      Price = menuItem.Price,
      IsAvailable = menuItem.IsAvailable,
      CategoryId = menuItem.CategoryId ?? 0,
    }).ToList();
    // Act
    var result = await _controller.GetMenuItems("Brus");

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var actualDtos = Assert.IsAssignableFrom<IEnumerable<MenuItemDTO>>(okResult.Value);
    Assert.Equal(expectedDtos.Count(), actualDtos.Count());
  }

  [Fact]
  public async Task GetMenuItems_ReturnsNotFoundResult_WhenMenuItemsIsNull()
  {
    // Arrange
    _mockMenuItemRepository.Setup(repo => repo.GetMenuItems()).ReturnsAsync((List<MenuItem>?)null!);

    // Act
    var result = await _controller.GetMenuItems("Brus");

    // Assert
    var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    Assert.Equal("Menu item list not found", notFoundResult.Value);
  }

  [Fact]
  public async Task GetmenuItem_ReturnNotFound_WhenMenuItemNotFound()
  {
    // Arrange
    int menuItemId = 1;
    _mockMenuItemRepository.Setup(repo => repo.GetMenuItemById(menuItemId)).ReturnsAsync((MenuItem?)null!);

    // Act
    var result = await _controller.GetMenuItem(menuItemId);

    // Assert
    var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    Assert.Equal("Menu item not found", notFoundResult.Value);
  }

  [Fact]
  public async Task GetMenuItem_ReturnOkResult_WhenMenuItemFound()
  {
    // Arrange
    int menuItemId = 1;
    var menuItem = new MenuItem { MenuItemId = menuItemId, Name = "Test MenuItem", Description = "Description", Price = 65, IsAvailable = true, CategoryId = 1, Category = new Category { Name = "Category 1" } };
    _mockMenuItemRepository.Setup(repo => repo.GetMenuItemById(menuItemId)).ReturnsAsync(menuItem);

    var expectedDto = new MenuItemDTO
    {
      MenuItemId = menuItem.MenuItemId,
      Name = menuItem.Name,
      Description = menuItem.Description,
      Price = menuItem.Price,
      IsAvailable = menuItem.IsAvailable,
      CategoryId = menuItem.CategoryId ?? 0,
    };

    // Act
    var result = await _controller.GetMenuItem(menuItemId);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var actualDto = Assert.IsType<MenuItemDTO>(okResult.Value);
    Assert.Equal(expectedDto.MenuItemId, actualDto.MenuItemId);
  }

  [Fact]
  public async Task CreateMenuItem_ReturnsBadRequest_WhenDtoIsNull()
  {
    // Arrange
    MenuItemDTO? menuItemDto = null;

    // Act
    var result = await _controller.CreateMenuItem(menuItemDto!);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    Assert.Equal("Menu item DTO is null", badRequestResult.Value);
  }

  [Fact]
  public async Task CreateMenuItem_ReturnsCreatedAtAction_WhenSuccessful()
  {
    // Arrange
    var menuItemDto = new MenuItemDTO { MenuItemId = 0, Name = "Test MenuItem", Description = "Description", Price = 65, IsAvailable = true, CategoryId = 1 };
    var menuItem = new MenuItem { Name = menuItemDto.Name, Description = menuItemDto.Description, Price = menuItemDto.Price, IsAvailable = menuItemDto.IsAvailable, CategoryId = menuItemDto.CategoryId };
    _mockMenuItemRepository.Setup(repo => repo.Create(It.IsAny<MenuItem>())).ReturnsAsync(true);
    _mockMenuItemRepository.Setup(repo => repo.GetMenuItemById(menuItem.MenuItemId)).ReturnsAsync(menuItem);

    // Act
    var result = await _controller.CreateMenuItem(menuItemDto);

    // Assert
    var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
    var createdMenuItemDto = Assert.IsType<MenuItemDTO>(createdAtActionResult.Value);
    Assert.Equal(menuItem.MenuItemId, createdMenuItemDto.MenuItemId);
  }

  [Fact]
  public async Task CreateMenuItem_ReturnsBadRequest_WhenCreationFails()
  {
    // Arrange
    var menuItemDto = new MenuItemDTO { Name = "Test MenuItem", Description = "Description", Price = 65, IsAvailable = true, CategoryId = 1 };
    _mockMenuItemRepository.Setup(repo => repo.Create(It.IsAny<MenuItem>())).ReturnsAsync(false);

    // Act
    var result = await _controller.CreateMenuItem(menuItemDto);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    Assert.Equal("Menu item creation failed", badRequestResult.Value);
  }

  [Fact]
  public async Task UpdateMenuItem_ReturnsBadRequest_WhenDtoIsNull()
  {
    // Arrange
    int menuItemId = 1;
    MenuItemDTO? menuItemDto = null;

    // Act
    var result = await _controller.UpdateMenuItem(menuItemId, menuItemDto!);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    Assert.Equal("Menu item DTO is null", badRequestResult.Value);
  }

  [Fact]
  public async Task UpdateMenuItem_ReturnsBadRequest_WhenUpdateFails()
  {
    // Arrange
    int menuItemId = 1;
    var menuItemDto = new MenuItemDTO { MenuItemId = menuItemId, Name = "Test MenuItem", Description = "Description", Price = 65, IsAvailable = true, CategoryId = 1 };
    var menuItem = new MenuItem { MenuItemId = menuItemId, Name = menuItemDto.Name, Description = menuItemDto.Description, Price = menuItemDto.Price, IsAvailable = menuItemDto.IsAvailable, CategoryId = menuItemDto.CategoryId };
    _mockMenuItemRepository.Setup(repo => repo.Update(It.IsAny<MenuItem>())).ReturnsAsync(false);

    // Act
    var result = await _controller.UpdateMenuItem(menuItemId, menuItemDto);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    Assert.Equal("Menu item update failed", badRequestResult.Value);
  }

  [Fact]
  public async Task UpdateMenuItem_ReturnsOkResult_WhenSuccessful()
  {
    // Arrange
    int menuItemId = 1;
    var menuItemDto = new MenuItemDTO { MenuItemId = menuItemId, Name = "Test MenuItem", Description = "Description", Price = 65, IsAvailable = true, CategoryId = 1 };
    var menuItem = new MenuItem { MenuItemId = menuItemId, Name = menuItemDto.Name, Description = menuItemDto.Description, Price = menuItemDto.Price, IsAvailable = menuItemDto.IsAvailable, CategoryId = menuItemDto.CategoryId };
    _mockMenuItemRepository.Setup(repo => repo.Update(It.IsAny<MenuItem>())).ReturnsAsync(true);
    _mockMenuItemRepository.Setup(repo => repo.GetMenuItemById(menuItem.MenuItemId)).ReturnsAsync(menuItem);

    // Act
    var result = await _controller.UpdateMenuItem(menuItemId, menuItemDto);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var updatedMenuItemDto = Assert.IsType<MenuItemDTO>(okResult.Value);
    Assert.Equal(menuItem.MenuItemId, updatedMenuItemDto.MenuItemId);
  }

  [Fact]
  public async Task DeleteMenuItem_ReturnesBadRequest_WhenDeletionFails()
  {
    // Arrange
    int menuItemId = 1;
    _mockMenuItemRepository.Setup(repo => repo.DeleteMenuItem(menuItemId)).ReturnsAsync(false);

    // Act
    var result = await _controller.DeleteMenuItem(menuItemId);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    Assert.Equal("Menu item deletion failed", badRequestResult.Value);
  }

  [Fact]
  public async Task DeleteMenuItem_ReturnsOkResult_WhenSuccessful()
  {
    // Arrange
    int menuItemId = 1;
    _mockMenuItemRepository.Setup(repo => repo.DeleteMenuItem(menuItemId)).ReturnsAsync(true);

    // Act
    var result = await _controller.DeleteMenuItem(menuItemId);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.Equal("Menu item deleted successfully", okResult.Value);
  }
}