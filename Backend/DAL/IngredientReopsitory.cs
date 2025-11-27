using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.DAL;

public class IngredientRepository : IIngredientRepository
{
  private readonly AppDbContext _context;
  private readonly ILogger<IngredientRepository> _logger;

  public IngredientRepository(AppDbContext context, ILogger<IngredientRepository> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task<IEnumerable<Ingredient?>> GetIngredients()
  {
    try
    {
      return await _context.Ingredients.ToListAsync();
    }
    catch (Exception ex)
    {
      _logger.LogError("[IngredientRepository] Ingredients ToListAsync failed when GetIngredients(), error: {ex}", ex.Message);
      return Enumerable.Empty<Ingredient>();
    }
  }

  public async Task<Ingredient?> GetIngredientById(int id)
  {
    try
    {
      return await _context.Ingredients.FindAsync(id);
    }
    catch (Exception ex)
    {
      _logger.LogError("[IngredientRepository] Ingredients FindAsync failed when GetIngredientById(), error: {ex}", ex.Message);
      return null;
    }
  }

  public async Task<bool> Create(Ingredient ingredient)
  {
    try
    {
      _context.Ingredients.Add(ingredient);
      await _context.SaveChangesAsync();
      return true;
    }
    catch (Exception ex)
    {
      _logger.LogError("[IngredientRepository] Ingredients Add failed when Create(), error: {ex}", ex.Message);
      return false;
    }
  }

  public async Task<bool> Update(Ingredient ingredient)
  {
    try
    {
      var trackedEntity = _context.Ingredients.Local.FirstOrDefault(e => e.IngredientId == ingredient.IngredientId);
      if (trackedEntity != null)
      {
        _context.Entry(trackedEntity).State = EntityState.Detached; // Detach the existing tracked entity
      }

      _context.Ingredients.Update(ingredient); // Attach the new entity
      await _context.SaveChangesAsync();
      return true;
    }
    catch (Exception ex)
    {
      _logger.LogError("[IngredientRepository] Ingredients Update failed when Update(), error: {ex}", ex.Message);
      return false;
    }
  }

  public async Task<bool> DeleteIngredient(int id)
  {
    try
    {
      var ingredient = await _context.Ingredients.FindAsync(id);
      if (ingredient == null)
      {
        _logger.LogError("[IngredientRepository] Ingredient not found when DeleteIngredient()");
        return false;
      }

      _context.Ingredients.Remove(ingredient);
      await _context.SaveChangesAsync();
      return true;
    }
    catch (Exception ex)
    {
      _logger.LogError("[IngredientRepository] Ingredients Remove failed when DeleteIngredient(), error: {ex}", ex.Message);
      return false;
    }
  }
}