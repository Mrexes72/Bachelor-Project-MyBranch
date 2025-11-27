using Backend.Models;

namespace Backend.DAL;

public interface IImageRepository
{
  Task<IEnumerable<Image?>> GetImages();
  Task<Image?> GetImageById(int id);
  Task<bool> Create(Image image);
  Task<bool> Update(Image image);
  Task<bool> DeleteImage(int id);
}