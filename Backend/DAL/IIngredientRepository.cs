using Backend.Models;

namespace Backend.DAL;

public interface IIngredientRepository
{
  Task<IEnumerable<Ingredient?>> GetIngredients();
  Task<Ingredient?> GetIngredientById(int id);

  Task<bool> Create(Ingredient ingredient);
  Task<bool> Update(Ingredient ingredient);
  Task<bool> DeleteIngredient(int id);
}