using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class IngredientDTO
{
    public int IngredientId { get; set; }
    [StringLength(70)]
    public String Name { get; set; } = String.Empty;
    public string? Description { get; set; }
    public string? Color { get; set; }
    [Range(0, 100, ErrorMessage = "Fill level must be between 0 and 100")]
    public int? FillLevel { get; set; }
    public bool IsAvailable { get; set; }
    public decimal UnitPrice { get; set; }
    public int CategoryId { get; set; } // FK
    public string? CategoryName { get; set; } // Optional for display purposes
    public string? ImagePath { get; set; }
    public IFormFile? ImageFile { get; set; }

}