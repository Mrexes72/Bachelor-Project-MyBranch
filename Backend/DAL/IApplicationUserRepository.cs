using Backend.Models;

namespace Backend.DAL;

public interface IApplicationUserRepository
{
  Task<IEnumerable<ApplicationUser>> GetApplicationUsers();
  Task<ApplicationUser?> GetApplicationUserById(string id);
  Task<bool> AddFavoriteDrinkAsync(string userId, Drink drink);
  Task<bool> AddCreatedDrinkAsync(string userId, Drink drink);

  Task<bool> Create(ApplicationUser applicationUser);
  Task<bool> Update(ApplicationUser applicationUser);
  Task<bool> DeleteApplicationUser(string id);
}