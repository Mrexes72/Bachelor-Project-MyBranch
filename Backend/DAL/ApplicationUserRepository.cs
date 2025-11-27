using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Backend.DAL;

public class ApplicationUserRepository : IApplicationUserRepository
{
  private readonly AppDbContext _context;
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly ILogger<ApplicationUserRepository> _logger;

  public ApplicationUserRepository(AppDbContext context, UserManager<ApplicationUser> userManager, ILogger<ApplicationUserRepository> logger)
  {
    _context = context;
    _userManager = userManager;
    _logger = logger;
  }


  public async Task<IEnumerable<ApplicationUser>> GetApplicationUsers()
  {
    try
    {
      return await _context.ApplicationUsers.ToListAsync();
    }
    catch (Exception e)
    {
      _logger.LogError("[ApplicationUserRepository] notes ToListAsync failed when GetApplicationUsers(), error message:{e}", e.Message);
      return Enumerable.Empty<ApplicationUser>();
    }
  }

  public async Task<ApplicationUser?> GetApplicationUserById(string id)
  {
    try
    {
      return await _context.ApplicationUsers.FindAsync(id) ?? throw new InvalidOperationException("ApplicationUser not found");
    }
    catch (Exception e)
    {
      _logger.LogError("[ApplicationUserRepository] ApplicationUsers FindAsync failed when GetApplicationUserById(), error message:{e}", e.Message);
      return null;
    }
  }

  public async Task<bool> AddFavoriteDrinkAsync(string userId, Drink drink)
  {
    var user = await GetApplicationUserById(userId);
    if (user == null)
    {
      return false;
    }

    if (user.FavoriteDrinks == null)
    {
      user.FavoriteDrinks = new List<Drink>();
    }

    user.FavoriteDrinks.Add(drink);
    var result = await _userManager.UpdateAsync(user);
    return result.Succeeded;
  }

  public async Task<bool> AddCreatedDrinkAsync(string userId, Drink drink)
  {
    var user = await GetApplicationUserById(userId);
    if (user == null)
    {
      return false;
    }

    if (user.CreatedDrinks == null)
    {
      user.CreatedDrinks = new List<Drink>();
    }


    user.CreatedDrinks.Add(drink);
    var result = await _userManager.UpdateAsync(user);
    return result.Succeeded;
  }

  public async Task<bool> Create(ApplicationUser applicationUser)
  {
    try
    {
      _context.ApplicationUsers.Add(applicationUser);
      await _context.SaveChangesAsync();
      return true;
    }
    catch (Exception e)
    {
      _logger.LogError("[ApplicationUserRepository] ApplicationUsers Add failed when Create(), error message:{e}", e.Message);
      return false;
    }
  }

  public async Task<bool> Update(ApplicationUser applicationUser)
  {
    try
    {
      _context.ApplicationUsers.Update(applicationUser);
      await _context.SaveChangesAsync();
      return true;
    }
    catch (Exception e)
    {
      _logger.LogError("[ApplicationUserRepository] ApplicationUsers EntityState failed when Update(), error message:{e}", e.Message);
      return false;
    }
  }

  public async Task<bool> DeleteApplicationUser(string id)
  {
    try
    {
      var applicationUser = await _context.ApplicationUsers.FindAsync(id);
      if (applicationUser == null)
      {
        _logger.LogError("[ApplicationUserRepository] ApplicationUsers deletion failed for id:{id}", id);
        return false;
      }

      _context.ApplicationUsers.Remove(applicationUser);
      await _context.SaveChangesAsync();
      return true;
    }
    catch (Exception e)
    {
      _logger.LogError("[ApplicationUserRepository] ApplicationUsers Remove failed when DeleteApplicationUser(), error message:{e}", e.Message);
      return false;
    }
  }
}