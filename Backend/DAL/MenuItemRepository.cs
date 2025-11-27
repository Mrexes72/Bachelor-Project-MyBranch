using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.DAL;

public class MenuItemRepository : IMenuItemRepository
{
  private readonly AppDbContext _context;
  private readonly ILogger<MenuItemRepository> _logger;

  public MenuItemRepository(AppDbContext context, ILogger<MenuItemRepository> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task<IEnumerable<MenuItem?>> GetMenuItems()
  {
    try
    {
      return await _context.MenuItems.ToListAsync();
    }
    catch (Exception e)
    {
      _logger.LogError("[MenuItemRepository] MenuItems ToListAsync failed when GetMenuItems(), error message:{e}", e.Message);
      return Enumerable.Empty<MenuItem>();
    }
  }

  public async Task<MenuItem?> GetMenuItemById(int id)
  {
    try
    {
      return await _context.MenuItems.FindAsync(id) ?? throw new InvalidOperationException("MenuItem not found");
    }
    catch (Exception e)
    {
      _logger.LogError("[MenuItemRepository] MenuItems FindAsync failed when GetMenuItemById(), error message:{e}", e.Message);
      return null;
    }
  }

  public async Task<bool> Create(MenuItem menuItem)
  {
    try
    {
      _context.MenuItems.Add(menuItem);
      await _context.SaveChangesAsync();
      return true;
    }
    catch (Exception e)
    {
      _logger.LogError("[MenuItemRepository] MenuItems Add failed when Create(), error message:{e}", e.Message);
      return false;
    }
  }

  public async Task<bool> Update(MenuItem menuItem)
  {
    try
    {
      _context.MenuItems.Update(menuItem);
      await _context.SaveChangesAsync();
      return true;
    }
    catch (Exception e)
    {
      _logger.LogError("[MenuItemRepository] MenuItems EntityState.Modified failed when Update(), error message:{e}", e.Message);
      return false;
    }
  }

  public async Task<bool> DeleteMenuItem(int id)
  {
    try
    {
      var menuItem = await _context.MenuItems.FindAsync(id);
      if (menuItem == null)
      {
        _logger.LogError("[MenuItemRepository] MenuItems FindAsync failed when DeleteMenuItem(), error message: menuItem is null");
        return false;
      }

      _context.MenuItems.Remove(menuItem);
      await _context.SaveChangesAsync();
      return true;
    }
    catch (Exception e)
    {
      _logger.LogError("[MenuItemRepository] MenuItems Remove failed when DeleteMenuItem(), error message:{e}", e.Message);
      return false;
    }
  }
  public async Task<IEnumerable<MenuItem>> GetMenuItemsByCategory(string category)
  {
    try
    {
      return await _context.MenuItems
          .Include(m => m.Category) // Include the Category relationship
          .Where(m => m.Category != null && m.Category.Name.Equals(category, StringComparison.OrdinalIgnoreCase))
          .ToListAsync();
    }
    catch (Exception e)
    {
      _logger.LogError("[MenuItemRepository] MenuItems filtering by category failed in GetMenuItemsByCategory(), error message:{e}", e.Message);
      return Enumerable.Empty<MenuItem>();
    }
  }
}