using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class Ingredient
{
    public int IngredientId { get; set; }
    public string Name { get; set; } = String.Empty;
    public string? Description { get; set; }
    public bool IsAvailable { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Color { get; set; }
    [Range(0, 100, ErrorMessage = "Fill level must be between 0 and 100")]
    public int? FillLevel { get; set; }
    public int? CategoryId { get; set; } // FK
    public virtual Category? Category { get; set; } // Navigation property
    public string? ImagePath { get; set; }
}
