using Backend.Models;

namespace Backend.DAL;

public interface IMenuItemRepository
{
  Task<IEnumerable<MenuItem?>> GetMenuItems();
  Task<MenuItem?> GetMenuItemById(int id);
  Task<IEnumerable<MenuItem>> GetMenuItemsByCategory(string category); // New method

  Task<bool> Create(MenuItem menuItem);
  Task<bool> Update(MenuItem menuItem);
  Task<bool> DeleteMenuItem(int id);
}