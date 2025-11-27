using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Backend.DTOs;

public class ImageDTO
{
  public int ImageId { get; set; }
  public string ImagePath { get; set; } = String.Empty;
  [StringLength(100)]
  public string ImageName { get; set; } = String.Empty;
  public IFormFile? ImageFile { get; set; }

  public bool IsCarouselImage { get; set; }
}
