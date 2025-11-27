using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class DrinkDTO
{
  public int DrinkId { get; set; }
  [StringLength(70)]
  public string Name { get; set; } = String.Empty;
  public decimal? BasePrice { get; set; }
  public decimal SalePrice { get; set; }
  // The number of times this drink has been favorited by users. 
  public int? TimesFavorite { get; set; }
  // En verdi for vekt/volum for utregning av pris?
  public string? CreatedByUserId { get; set; } // FK
  public virtual List<IngredientDTO>? IngredientDTOs { get; set; }
  public virtual int? CategoryId { get; set; } // FK
  public string? ImagePath { get; set; }
}