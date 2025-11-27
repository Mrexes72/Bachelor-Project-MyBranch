using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class MenuItemDTO
{
  public int MenuItemId { get; set; }
  [StringLength(70)]
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public decimal Price { get; set; }
  public bool IsAvailable { get; set; }
  public int CategoryId { get; set; } // FK
  public string? CategoryName { get; set; } // Optional for display purposes
}