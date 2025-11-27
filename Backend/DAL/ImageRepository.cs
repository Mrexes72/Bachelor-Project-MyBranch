using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.DAL;

public class ImageRepository : IImageRepository
{
  private readonly AppDbContext _context;
  private readonly ILogger<ImageRepository> _logger;

  public ImageRepository(AppDbContext context, ILogger<ImageRepository> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task<IEnumerable<Image?>> GetImages()
  {
    try
    {
      return await _context.Images.ToListAsync();
    }
    catch (Exception ex)
    {
      _logger.LogError($"[ImageRepository] Images ToListAsync failed when GetImages(), error: {ex.Message}");
      return Enumerable.Empty<Image>();
    }
  }

  public async Task<Image?> GetImageById(int id)
  {
    try
    {
      return await _context.Images.FindAsync(id);
    }
    catch (Exception ex)
    {
      _logger.LogError($"[ImageRepository] Images FindAsync failed when GetImageById(), error: {ex.Message}");
      return null;
    }
  }

  public async Task<bool> Create(Image image)
  {
    try
    {
      _context.Images.Add(image);
      await _context.SaveChangesAsync();
      return true;
    }
    catch (Exception ex)
    {
      _logger.LogError("[ImageRepository] Images Add failed when Create(), error: {ex}", ex.Message);
      return false;
    }
  }

  public async Task<bool> Update(Image image)
  {
    try
    {
      _context.Images.Update(image);
      await _context.SaveChangesAsync();
      return true;
    }
    catch (Exception ex)
    {
      _logger.LogError("[ImageRepository] Images Update failed when Update(), error: {ex}", ex.Message);
      return false;
    }
  }

  public async Task<bool> DeleteImage(int id)
  {
    try
    {
      var image = await _context.Images.FindAsync(id);
      if (image == null)
      {
        return false;
      }
      _context.Images.Remove(image);
      await _context.SaveChangesAsync();
      return true;
    }
    catch (Exception ex)
    {
      _logger.LogError("[ImageRepository] Images Remove failed when DeleteImage(), error: {ex}", ex.Message);
      return false;
    }
  }
}