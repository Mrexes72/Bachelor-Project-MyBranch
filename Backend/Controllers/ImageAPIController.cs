using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.DTOs;
using Backend.DAL;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImageAPIController : Controller
{
  private readonly IImageRepository _imageRepository;
  private readonly ILogger<ImageAPIController> _logger;
  private readonly IWebHostEnvironment _webHostEnvironment;

  public ImageAPIController(IImageRepository imageRepository, ILogger<ImageAPIController> logger, IWebHostEnvironment webHostEnvironment)
  {
    _imageRepository = imageRepository;
    _logger = logger;
    _webHostEnvironment = webHostEnvironment;
  }

  [HttpGet("imageslist")]
  public async Task<IActionResult> GetImages()
  {
    // Get images from the database
    var dbImages = await _imageRepository.GetImages();
    if (dbImages == null)
    {
      _logger.LogError("[ImageAPIController] Image list not found while executing _imageRepository.GetImages()");
      return NotFound("Image list not found");
    }

    var dbImageDtos = dbImages
      .Where(image => image != null)
      .Select(image => new ImageDTO
      {
        ImageId = image!.ImageId, // Ensure that ImageId is not null
        ImagePath = $"{Request.Scheme}://{Request.Host}/images/{image.ImageName}", // Ensure the relative path is correct
        ImageName = image.ImageName,
        IsCarouselImage = image.IsCarouselImage
      }).ToList() ?? new List<ImageDTO>();

    // Get images from the wwwroot/images folder
    var imagesFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
    if (!Directory.Exists(imagesFolder))
    {
      _logger.LogWarning("[ImageAPIController] Images folder does not exist: {ImagesFolder}", imagesFolder);
      return Ok(dbImageDtos); // Return only database images if folder doesn't exist
    }

    var folderImages = Directory.GetFiles(imagesFolder)
        .Select((filePath, index) => new ImageDTO
        {
          ImageId = -(index + 1), // Assign negative IDs to folder images
          ImagePath = $"{Request.Scheme}://{Request.Host}/images/{Path.GetFileName(filePath)}", // Full URL
          ImageName = Path.GetFileName(filePath),
          IsCarouselImage = false
        });

    // Combine database images and folder images
    var allImages = dbImageDtos.Concat(folderImages).ToList();

    return Ok(allImages);
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetImage(int id)
  {
    var image = await _imageRepository.GetImageById(id);
    if (image == null)
    {
      _logger.LogError("[ImageAPIController] Image not found while executing _imageRepository.GetImageById(id)");
      return NotFound("Image not found");
    }

    return Ok(image);
  }

  [HttpPost("create")]
  public async Task<IActionResult> CreateImage([FromForm] ImageDTO imageDto)
  {
    if (imageDto.ImageFile == null || imageDto.ImageFile.Length == 0)
    {
      _logger.LogError("[ImageAPIController] Image file not found while executing CreateImage");
      return BadRequest("Image file not found");
    }

    try
    {
      // Validate file type
      var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
      var fileExtension = Path.GetExtension(imageDto.ImageFile.FileName).ToLowerInvariant();
      if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
      {
        _logger.LogError("[ImageAPIController] Invalid file type: {FileExtension}", fileExtension);
        return BadRequest("Only .jpg and .png files are allowed");
      }

      // Ensure the ImageName includes the file extension
      var imageName = Path.GetFileNameWithoutExtension(imageDto.ImageName) + fileExtension;

      // Create the upload folder if it doesn't exist
      var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
      if (!Directory.Exists(uploadFolder))
      {
        Directory.CreateDirectory(uploadFolder);
      }

      // Save the file to the server
      var filePath = Path.Combine(uploadFolder, imageName);
      using (var fileStream = new FileStream(filePath, FileMode.Create))
      {
        await imageDto.ImageFile.CopyToAsync(fileStream);
      }

      // Save the image details to the database
      var image = new Image
      {
        ImagePath = Path.Combine("images", imageName), // Store relative path
        ImageName = imageName
      };

      var result = await _imageRepository.Create(image);
      if (!result)
      {
        _logger.LogError("[ImageAPIController] Failed to save image to the database");
        return StatusCode(500, "Failed to save image to the database");
      }

      return Ok(new ImageResponse
      {
        Message = "Image uploaded successfully",
        Image = new ImageDetails
        {
          ImageName = image.ImageName,
          ImagePath = $"{Request.Scheme}://{Request.Host}/images/{image.ImageName}"
        }
      });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "[ImageAPIController] Error while uploading image");
      return StatusCode(500, "Internal server error");
    }
  }

  [HttpPut("update/{id}")]
  public async Task<IActionResult> UpdateImage([FromBody] ImageDTO imageDto)
  {
    // Check if the image ID is provided
    if (imageDto.ImageId == 0)
    {
      _logger.LogError("[ImageAPIController] Image ID not provided while executing UpdateImage");
      return BadRequest("Image ID not provided");
    }

    // Get the existing image details
    var existingImage = await _imageRepository.GetImageById(imageDto.ImageId);
    if (existingImage == null)
    {
      _logger.LogError("[ImageAPIController] Image not found while executing _imageRepository.GetImageById(imageDto.Id)");
      return NotFound("Image not found");
    }


    // Check if a new image file is provided
    if (imageDto.ImageFile != null && imageDto.ImageFile.Length > 0)
    {
      // Create the images folder if it doesn't exist
      var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
      if (!Directory.Exists(uploadFolder))
      {
        Directory.CreateDirectory(uploadFolder);
      }

      // Save the new image file to the server
      var filePath = Path.Combine(uploadFolder, imageDto.ImageName);
      using (var fileStream = new FileStream(filePath, FileMode.Create))
      {
        await imageDto.ImageFile.CopyToAsync(fileStream);
      }

      // Update the image details
      existingImage.ImagePath = filePath;
      existingImage.ImageName = imageDto.ImageName;
    }

    // Update the image details in the database
    var result = await _imageRepository.Update(existingImage);
    if (!result)
    {
      _logger.LogError("[ImageAPIController] Image not updated while executing _imageRepository.UpdateImage(existingImage)");
      return StatusCode(500, "Image not updated");
    }

    return Ok("Image updated");
  }

  [HttpPut("toggle-carousel/{id}")]
  public async Task<IActionResult> ToggleCarouselImage(int id)
  {
    var image = await _imageRepository.GetImageById(id);
    if (image == null)
    {
      _logger.LogWarning("Image with ID {Id} not found.", id);
      return NotFound("Image not found");
    }

    image.IsCarouselImage = !image.IsCarouselImage;
    var result = await _imageRepository.Update(image);

    if (!result)
    {
      _logger.LogError("Failed to update carousel status for image with ID {Id}.", id);
      return StatusCode(500, "Failed to update carousel status");
    }

    return Ok(new ImageResponse
    {
      Message = "Carousel status updated successfully",
      Image = new ImageDetails
      {
        IsCarouselImage = image.IsCarouselImage
      }
    });
  }

  [HttpGet("carousel-images")]
  public async Task<IActionResult> GetCarouselImages()
  {
    try
    {
      var dbImages = await _imageRepository.GetImages();
      if (dbImages == null)
      {
        _logger.LogError("[ImageAPIController] No images found in GetCarouselImages()");
        return NotFound("No images found");
      }

      var carouselImages = dbImages
          .Where(image => image != null && image.IsCarouselImage)
          .Select(image => new ImageDTO
          {
            ImageId = image?.ImageId ?? 0,
            ImagePath = $"{Request.Scheme}://{Request.Host}/images/{image?.ImageName}",
            ImageName = image?.ImageName ?? string.Empty, // Add null check
            IsCarouselImage = image?.IsCarouselImage ?? false // Add null check
          }).ToList();

      _logger.LogInformation("[ImageAPIController] Successfully retrieved {Count} carousel images", carouselImages.Count);
      return Ok(carouselImages);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "[ImageAPIController] Error retrieving carousel images");
      return StatusCode(500, new { error = "Error retrieving carousel images", message = ex.Message });
    }
  }

  [HttpDelete("delete/{id}")]
  public async Task<IActionResult> DeleteImage(int id)
  {
    _logger.LogInformation("Attempting to delete image with ID: {Id}", id);

    // Get the existing image details
    var existingImage = await _imageRepository.GetImageById(id);
    if (existingImage == null)
    {
      _logger.LogWarning("Image with ID {Id} not found in the database.", id);
      return NotFound("Image not found");
    }

    // Ensure the relative path is correctly resolved to an absolute path
    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, existingImage.ImagePath.TrimStart('/'));

    // Delete the image file from the server
    if (System.IO.File.Exists(filePath))
    {
      try
      {
        System.IO.File.Delete(filePath);
        _logger.LogInformation("File deleted successfully: {FilePath}", filePath);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
        return StatusCode(500, "Error deleting file");
      }
    }
    else
    {
      _logger.LogWarning("File not found at path: {FilePath}", filePath);
    }

    // Delete the image details from the database
    var result = await _imageRepository.DeleteImage(id);
    if (!result)
    {
      _logger.LogError("Failed to delete image with ID {Id} from the database.", id);
      return StatusCode(500, "Image not deleted");
    }

    _logger.LogInformation("Image with ID {Id} deleted successfully.", id);
    return Ok(new ImageResponse { Message = "Image deleted successfully" });
  }
}

public class ImageResponse
{
  public string Message { get; set; } = string.Empty;
  public ImageDetails? Image { get; set; }
}

public class ImageDetails
{
  public string ImageName { get; set; } = string.Empty;
  public string ImagePath { get; set; } = string.Empty;
  public bool IsCarouselImage { get; set; } = false;
}