namespace Backend.Models;

public class Category
{
  public int CategoryId { get; set; } // PK
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }

}