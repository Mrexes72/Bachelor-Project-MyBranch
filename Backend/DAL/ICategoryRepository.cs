using Backend.Models;

namespace Backend.DAL;

public interface ICategoryRepository
{
  Task<IEnumerable<Category?>> GetCategories();
  Task<Category?> GetCategoryById(int id);

  Task<bool> Create(Category category);
  Task<bool> Update(Category category);
  Task<bool> DeleteCategory(int id);
}