using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Backend.Models;

public class ApplicationUser : IdentityUser
{
    // Navigation properties with properly configured collections
    public virtual ICollection<Drink> FavoriteDrinks { get; set; } = new List<Drink>();
    public virtual ICollection<Drink> CreatedDrinks { get; set; } = new List<Drink>();
}