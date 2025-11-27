using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

public class Image
{
  public int ImageId { get; set; }
  public string ImagePath { get; set; } = string.Empty;
  [StringLength(100)]
  public string ImageName { get; set; } = string.Empty;
  [NotMapped]
  public IFormFile? ImageFile { get; set; }

  public bool IsCarouselImage { get; set; }
}