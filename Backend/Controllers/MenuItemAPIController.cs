using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.DTOs;
using Backend.DAL;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuItemAPIController : Controller
{
  private readonly IMenuItemRepository _menuItemRepository;
  private readonly ILogger<MenuItemAPIController> _logger;

  public MenuItemAPIController(IMenuItemRepository menuItemRepository, ILogger<MenuItemAPIController> logger)
  {
    _menuItemRepository = menuItemRepository;
    _logger = logger;
  }

  [HttpGet("menuitemslist")]
  public async Task<IActionResult> GetMenuItems([FromQuery] string? category)
  {
    if (string.IsNullOrEmpty(category))
    {
      return BadRequest("Category is required.");
    }

    var menuItems = await _menuItemRepository.GetMenuItems();
    if (menuItems == null || !menuItems.Any())
    {
      _logger.LogError("[MenuItemAPIController] Menu item list not found while executing _menuItemRepository.GetMenuItems()");
      return NotFound("Menu item list not found");
    }

    var menuItemDtos = menuItems
    .Where(menuItem => menuItem != null)
    .Select(menuItem => new MenuItemDTO
    {
      MenuItemId = menuItem!.MenuItemId, // Use null-forgiving operator to avoid null reference exception
      Name = menuItem.Name,
      Description = menuItem.Description,
      Price = menuItem.Price,
      IsAvailable = menuItem.IsAvailable,
      CategoryId = menuItem.CategoryId ?? 0,
      CategoryName = menuItem.Category?.Name ?? string.Empty // Ensure that CategoryName is not null
    }).ToList();
    return Ok(menuItemDtos);
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetMenuItem(int id)
  {
    var menuItem = await _menuItemRepository.GetMenuItemById(id);
    if (menuItem == null)
    {
      _logger.LogError("[MenuItemAPIController] Menu item not found while executing _menuItemRepository.GetMenuItem(id)");
      return NotFound("Menu item not found");
    }

    var menuItemDto = new MenuItemDTO
    {
      MenuItemId = menuItem.MenuItemId,
      Name = menuItem.Name,
      Description = menuItem.Description,
      Price = menuItem.Price,
      IsAvailable = menuItem.IsAvailable,
      CategoryId = menuItem.CategoryId ?? 0,
      CategoryName = menuItem.Category?.Name ?? string.Empty // Ensure that CategoryName is not null
    };

    return Ok(menuItemDto);
  }

  [HttpPost("create")]
  public async Task<IActionResult> CreateMenuItem([FromBody] MenuItemDTO menuItemDto)
  {
    if (menuItemDto == null)
    {
      _logger.LogError("[MenuItemAPIController] Menu item DTO is null while executing CreateMenuItem()");
      return BadRequest("Menu item DTO is null");
    }

    var menuItem = new MenuItem
    {
      Name = menuItemDto.Name,
      Description = menuItemDto.Description,
      Price = menuItemDto.Price,
      IsAvailable = menuItemDto.IsAvailable,
      CategoryId = menuItemDto.CategoryId,
    };

    var result = await _menuItemRepository.Create(menuItem);
    if (!result)
    {
      _logger.LogError("[MenuItemAPIController] Menu item creation failed while executing _menuItemRepository.Create(menuItem)");
      return BadRequest("Menu item creation failed");
    }

    var menuItemDtoResponse = new MenuItemDTO
    {
      MenuItemId = menuItem.MenuItemId,
      Name = menuItem.Name,
      Description = menuItem.Description,
      Price = menuItem.Price,
      IsAvailable = menuItem.IsAvailable,
      CategoryId = menuItem.CategoryId ?? 0,
      CategoryName = menuItem.Category?.Name
    };

    return CreatedAtAction(nameof(GetMenuItem), new { id = menuItem.MenuItemId }, menuItemDtoResponse);
  }

  [HttpPut("update/{id}")]
  public async Task<IActionResult> UpdateMenuItem(int id, [FromBody] MenuItemDTO menuItemDto)
  {
    if (menuItemDto == null)
    {
      _logger.LogError("[MenuItemAPIController] Menu item DTO is null while executing UpdateMenuItem()");
      return BadRequest("Menu item DTO is null");
    }

    var menuItem = new MenuItem
    {
      MenuItemId = id,
      Name = menuItemDto.Name,
      Description = menuItemDto.Description,
      Price = menuItemDto.Price,
      IsAvailable = menuItemDto.IsAvailable,
      CategoryId = menuItemDto.CategoryId
    };

    var result = await _menuItemRepository.Update(menuItem);
    if (!result)
    {
      _logger.LogError("[MenuItemAPIController] Menu item update failed while executing _menuItemRepository.Update(menuItem)");
      return BadRequest("Menu item update failed");
    }

    var menuItemDtoResponse = new MenuItemDTO
    {
      MenuItemId = menuItem.MenuItemId,
      Name = menuItem.Name,
      Description = menuItem.Description,
      Price = menuItem.Price,
      IsAvailable = menuItem.IsAvailable,
      CategoryId = menuItem.CategoryId ?? 0,
      CategoryName = menuItem.Category?.Name
    };

    return Ok(menuItemDtoResponse);
  }

  [HttpDelete("delete/{id}")]
  public async Task<IActionResult> DeleteMenuItem(int id)
  {
    var result = await _menuItemRepository.DeleteMenuItem(id);
    if (!result)
    {
      _logger.LogError("[MenuItemAPIController] Menu item deletion failed while executing _menuItemRepository.DeleteMenuItem(id)");
      return BadRequest("Menu item deletion failed");
    }

    return Ok("Menu item deleted successfully");
  }
}