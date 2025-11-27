using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.DAL;

public class CategoryRepository : ICategoryRepository
{
  private readonly AppDbContext _context;
  private readonly ILogger<CategoryRepository> _logger;

  public CategoryRepository(AppDbContext context, ILogger<CategoryRepository> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task<IEnumerable<Category?>> GetCategories()
  {
    try
    {
      return await _context.Categories.ToListAsync();
    }
    catch (Exception e)
    {
      _logger.LogError("[CategoryRepository] Categories ToListAsync failed when GetCategories(), error message:{e}", e.Message);
      return Enumerable.Empty<Category>();
    }
  }

  public async Task<Category?> GetCategoryById(int id)
  {
    try
    {
      return await _context.Categories.FindAsync(id) ?? throw new InvalidOperationException("Category not found");
    }
    catch (Exception e)
    {
      _logger.LogError("[CategoryRepository] Categories FindAsync failed when GetCategoryById(), error message:{e}", e.Message);
      return null;
    }
  }

  public async Task<bool> Create(Category category)
  {
    try
    {
      _context.Categories.Add(category);
      await _context.SaveChangesAsync();
      return true;
    }
    catch (Exception e)
    {
      _logger.LogError("[CategoryRepository] Categories Add failed when Create(), error message:{e}", e.Message);
      return false;
    }
  }

  public async Task<bool> Update(Category category)
  {
    try
    {
      _context.Categories.Update(category);
      await _context.SaveChangesAsync();
      return true;
    }
    catch (Exception e)
    {
      _logger.LogError("[CategoryRepository] Categories Update failed when Update(), error message:{e}", e.Message);
      return false;
    }
  }

  public async Task<bool> DeleteCategory(int id)
  {
    try
    {
      var category = await _context.Categories.FindAsync(id);
      if (category == null)
      {
        throw new InvalidOperationException("Category not found");
      }
      _context.Categories.Remove(category);
      await _context.SaveChangesAsync();
      return true;
    }
    catch (Exception e)
    {
      _logger.LogError("[CategoryRepository] Categories Remove failed when DeleteCategory(), error message:{e}", e.Message);
      return false;
    }
  }
}