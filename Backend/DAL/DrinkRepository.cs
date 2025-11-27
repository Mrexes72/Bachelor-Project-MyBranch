using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.DAL;

public class DrinkRepository : IDrinkRepository
{
  private readonly AppDbContext _context;
  private readonly ILogger<DrinkRepository> _logger;

  public DrinkRepository(AppDbContext context, ILogger<DrinkRepository> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task<IEnumerable<Drink?>> GetDrinks()
  {
    try
    {
      return await _context.Drinks.ToListAsync();
    }
    catch (Exception e)
    {
      _logger.LogError("[DrinkRepository] Drinks ToListAsync failed when GetDrinks(), error message:{e}", e.Message);
      return Enumerable.Empty<Drink>();
    }
  }

  public async Task<Drink?> GetDrinkById(int id)
  {
    try
    {
      return await _context.Drinks.FindAsync(id) ?? throw new InvalidOperationException("Drink not found");
    }
    catch (Exception e)
    {
      _logger.LogError("[DrinkRepository] Drinks FindAsync failed when GetDrinkById(), error message:{e}", e.Message);
      return null;
    }
  }

  public async Task<bool> Create(Drink drink)
  {
    try
    {
      _context.Drinks.Add(drink);
      await _context.SaveChangesAsync();
      return true;
    }
    catch (Exception e)
    {
      _logger.LogError("[DrinkRepository] Drinks Add failed when Create(), error message:{e}", e.Message);
      return false;
    }
  }

  public async Task<bool> Update(Drink drink)
  {
    try
    {
      _context.Drinks.Update(drink);
      await _context.SaveChangesAsync();
      return true;
    }
    catch (Exception e)
    {
      _logger.LogError("[DrinkRepository] Drinks Update failed when Update(), error message:{e}", e.Message);
      return false;
    }
  }

  public async Task<bool> DeleteDrink(int id)
  {
    try
    {
      var drink = await _context.Drinks
      .Include(d => d.Ingredients)
      .FirstOrDefaultAsync(d => d.DrinkId == id);
      if (drink == null)
      {
        _logger.LogError("[DrinkRepository] Drink not found when DeleteDrink()");
        return false;
      }
      if (drink.Ingredients != null)
      {
        _context.Ingredients.RemoveRange(drink.Ingredients);
      }
      _context.Drinks.Remove(drink);
      await _context.SaveChangesAsync();
      return true;
    }
    catch (Exception e)
    {
      _logger.LogError("[DrinkRepository] Drinks Remove failed when DeleteDrink(), error message:{e}", e.Message);
      return false;
    }
  }

  public async Task<List<Ingredient>> GetIngredientsByIds(List<int> ingredientIds)
  {
    return await _context.Ingredients
        .Where(i => ingredientIds.Contains(i.IngredientId))
        .ToListAsync();
  }
}