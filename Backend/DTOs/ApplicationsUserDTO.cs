using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class ApplicationUserDTO
{
  public string Id { get; set; } = string.Empty;
  public string Username { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public List<DrinkDTO>? FavoriteDrinks { get; set; }
  public List<DrinkDTO>? CreatedDrinks { get; set; }

}