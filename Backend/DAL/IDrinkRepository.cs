using Backend.Models;

namespace Backend.DAL;

public interface IDrinkRepository
{
  Task<IEnumerable<Drink?>> GetDrinks();
  Task<Drink?> GetDrinkById(int id);

  Task<bool> Create(Drink drink);
  Task<bool> Update(Drink drink);
  Task<bool> DeleteDrink(int id);
  Task<List<Ingredient>> GetIngredientsByIds(List<int> ingredientIds);
}