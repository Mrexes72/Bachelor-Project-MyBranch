using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class CategoryDTO
{
  public int CategoryId { get; set; }
  [StringLength(70)]
  public string Name { get; set; } = String.Empty;
  public string Description { get; set; } = String.Empty;

}