using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

public class Drink
{
    public int DrinkId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal? BasePrice { get; set; }
    public decimal SalePrice { get; set; }

    // Foreign keys for ApplicationUser relationships
    public string? CreatedByUserId { get; set; }

    // Navigation property for creator
    [ForeignKey("CreatedByUserId")]
    public virtual ApplicationUser? CreatedByUser { get; set; }

    // Category relationship
    public int? CategoryId { get; set; }
    public virtual Category? Category { get; set; }

    // Ingredients collection
    public virtual ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

    // Other properties
    public string? ImagePath { get; set; }
    public int? TimesFavorite { get; set; }
}