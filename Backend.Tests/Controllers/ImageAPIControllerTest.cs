using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Backend.Controllers;
using Backend.DAL;
using Backend.Models;
using Backend.DTOs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Backend.Tests;

public class ImageAPIControllerTest
{
  private readonly Mock<IImageRepository> _mockImageRepository;
  private readonly Mock<ILogger<ImageAPIController>> _mockLogger;
  private readonly Mock<IWebHostEnvironment> _mockWebHostEnvironment;
  private readonly ImageAPIController _controller;

  public ImageAPIControllerTest()
  {
    _mockImageRepository = new Mock<IImageRepository>();
    _mockLogger = new Mock<ILogger<ImageAPIController>>();
    _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();

    _controller = new ImageAPIController(
        _mockImageRepository.Object,
        _mockLogger.Object,
        _mockWebHostEnvironment.Object);
  }

  // Mocking the IDirectoryWrapper interface to simulate file system operations
  // This is used to avoid actual file system access during unit tests.
  public interface IDirectoryWrapper
  {
    bool Exists(string path);
    string[] GetFiles(string path);
  }


  [Fact]
  public async Task GetImages_ReturnsNotFoundResult_WhenImagesIsNull()
  {
    // Arrange
    _mockImageRepository.Setup(repo => repo.GetImages()).ReturnsAsync((List<Image>?)null!);

    // Act
    var result = await _controller.GetImages();

    // Assert
    var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

  }

  [Fact]
  public async Task GetImages_ReturnsCombinedImages_WhenDatabaseAndFolderImagesExist()
  {
    // Arrange
    var dbImages = new List<Image>
    {
        new Image { ImageId = 1, ImageName = "dbImage1.jpg", IsCarouselImage = true },
        new Image { ImageId = 2, ImageName = "dbImage2.jpg", IsCarouselImage = false }
    };

    var imagesFolder = Path.Combine("wwwroot", "images");
    Directory.CreateDirectory(imagesFolder);
    File.WriteAllText(Path.Combine(imagesFolder, "folderImage1.jpg"), "dummy content");
    File.WriteAllText(Path.Combine(imagesFolder, "folderImage2.png"), "dummy content");

    _mockImageRepository.Setup(repo => repo.GetImages()).ReturnsAsync(dbImages);
    _mockWebHostEnvironment.Setup(env => env.WebRootPath).Returns("wwwroot");

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext
      {
        Request =
            {
                Scheme = "http",
                Host = new HostString("localhost")
            }
      }
    };

    // Act
    var result = await _controller.GetImages();

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var images = Assert.IsType<List<ImageDTO>>(okResult.Value);

    Assert.Contains(images, img => img.ImageName == "dbImage1.jpg");
    Assert.Contains(images, img => img.ImageName == "folderImage1.jpg");

    // Clean up
    Directory.Delete(imagesFolder, true);
  }

  [Fact]
  public async Task GetImages_ReturnsDatabaseImages_WhenFolderDoesNotExist()
  {
    // Arrange
    var dbImages = new List<Image>
    {
        new Image { ImageId = 1, ImageName = "dbImage1.jpg", IsCarouselImage = true },
        new Image { ImageId = 2, ImageName = "dbImage2.jpg", IsCarouselImage = false }
    };

    _mockImageRepository.Setup(repo => repo.GetImages()).ReturnsAsync(dbImages);
    _mockWebHostEnvironment.Setup(env => env.WebRootPath).Returns("wwwroot");

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext
      {
        Request =
            {
                Scheme = "http",
                Host = new HostString("localhost")
            }
      }
    };

    // Act
    var result = await _controller.GetImages();

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var images = Assert.IsType<List<ImageDTO>>(okResult.Value);

    Assert.NotNull(images);
    Assert.Contains(images, img => img.ImageName == "dbImage1.jpg");
  }

  [Fact]
  public async Task GetImage_ReturnsNotFoundResult_WhenImageIsNull()
  {
    // Arrange
    _mockImageRepository.Setup(repo => repo.GetImageById(1)).ReturnsAsync(default(Image));

    // Act
    var result = await _controller.GetImage(1);

    // Assert
    var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    Assert.Equal("Image not found", notFoundResult.Value);
  }

  [Fact]
  public async Task GetImage_ReturnsOkResult_WhenSuccessful()
  {
    // Arrange
    var dbImage = new Image { ImageId = 1, ImageName = "dbImage.jpg" };
    _mockImageRepository.Setup(repo => repo.GetImageById(1)).ReturnsAsync(dbImage);
    _mockWebHostEnvironment.Setup(env => env.WebRootPath).Returns("wwwroot");

    var imagesFolder = Path.Combine("wwwroot", "images");
    Directory.CreateDirectory(imagesFolder);
    var filePath = Path.Combine(imagesFolder, "folderImage.jpg");
    System.IO.File.Create(filePath).Dispose();

    // Act
    var result = await _controller.GetImage(1);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var model = Assert.IsAssignableFrom<Image>(okResult.Value);
    Assert.Equal(1, model.ImageId);
  }

  [Fact]
  public async Task CreateImage_ReturnsBadRequest_WhenImageFileIsNull()
  {
    // Arrange
    var imageDto = new ImageDTO { ImageFile = null };

    // Act
    var result = await _controller.CreateImage(imageDto);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    Assert.Equal("Image file not found", badRequestResult.Value);
  }

  [Fact]
  public async Task CreateImage_ReturnsBadRequest_WhenFileTypeIsInvalid()
  {
    // Arrange
    var imageFileMock = new Mock<IFormFile>();
    imageFileMock.Setup(f => f.FileName).Returns("invalid.txt");
    imageFileMock.Setup(f => f.Length).Returns(1); // Ensure the file length is greater than 0
    var imageDto = new ImageDTO { ImageFile = imageFileMock.Object };

    // Act
    var result = await _controller.CreateImage(imageDto);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    Assert.Equal("Only .jpg and .png files are allowed", badRequestResult.Value);
  }

  [Fact]
  public async Task CreateImage_ReturnsOk_WhenUploadIsSuccessful()
  {
    // Arrange
    var imageFileMock = new Mock<IFormFile>();
    imageFileMock.Setup(f => f.FileName).Returns("test.jpg");
    imageFileMock.Setup(f => f.Length).Returns(1);

    var imageDto = new ImageDTO { ImageFile = imageFileMock.Object, ImageName = "test" };

    var uploadFolder = Path.Combine("wwwroot", "images");
    _mockWebHostEnvironment.Setup(env => env.WebRootPath).Returns("wwwroot");

    _mockImageRepository.Setup(repo => repo.Create(It.IsAny<Image>())).ReturnsAsync(true);

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext
      {
        Request =
            {
                Scheme = "http",
                Host = new HostString("localhost")
            }
      }
    };

    // Act
    var result = await _controller.CreateImage(imageDto);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.NotNull(okResult.Value);

    var response = Assert.IsType<ImageResponse>(okResult.Value);
    Assert.NotNull(response.Image);

    Assert.Equal("Image uploaded successfully", response.Message);
    Assert.Equal("test.jpg", response.Image.ImageName);
    Assert.Equal("http://localhost/images/test.jpg", response.Image.ImagePath);


    // Clean up
    var filePath = Path.Combine(uploadFolder, "test.jpg");
    if (File.Exists(filePath))
    {
      File.Delete(filePath);
    }
  }

  [Fact]
  public async Task CreateImage_ReturnsInternalServerError_WhenUploadFails()
  {
    // Arrange
    var imageFileMock = new Mock<IFormFile>();
    imageFileMock.Setup(f => f.FileName).Returns("test.jpg");
    imageFileMock.Setup(f => f.Length).Returns(1);
    var imageDto = new ImageDTO { ImageFile = imageFileMock.Object, ImageName = "test" };

    var uploadFolder = Path.Combine("wwwroot", "images");
    _mockWebHostEnvironment.Setup(env => env.WebRootPath).Returns("wwwroot");

    _mockImageRepository.Setup(repo => repo.Create(It.IsAny<Image>())).ReturnsAsync(false);

    // Act
    var result = await _controller.CreateImage(imageDto);

    // Assert
    var statusCodeResult = Assert.IsType<ObjectResult>(result);
    Assert.Equal(500, statusCodeResult.StatusCode);
    Assert.Equal("Failed to save image to the database", statusCodeResult.Value);
  }

  [Fact]
  public async Task CreateImage_ReturnsInternalServerError_WhenExceptionIsThrown()
  {
    // Arrange
    var imageFileMock = new Mock<IFormFile>();
    imageFileMock.Setup(f => f.FileName).Returns("test.jpg");
    imageFileMock.Setup(f => f.Length).Returns(1);
    var imageDto = new ImageDTO { ImageFile = imageFileMock.Object, ImageName = "test" };

    var uploadFolder = Path.Combine("wwwroot", "images");
    _mockWebHostEnvironment.Setup(env => env.WebRootPath).Returns("wwwroot");

    _mockImageRepository.Setup(repo => repo.Create(It.IsAny<Image>())).ThrowsAsync(new Exception("Test exception"));

    // Act
    var result = await _controller.CreateImage(imageDto);

    // Assert
    var statusCodeResult = Assert.IsType<ObjectResult>(result);
    Assert.Equal(500, statusCodeResult.StatusCode);
    Assert.Equal("Internal server error", statusCodeResult.Value);

    // Clean up
    var filePath = Path.Combine(uploadFolder, "test.jpg");
    if (File.Exists(filePath))
    {
      File.Delete(filePath);
    }
  }

  [Fact]
  public async Task UpdateImage_ReturnsBadRequest_WhenImageIdIsNotProvided()
  {
    // Arrange
    var imageDto = new ImageDTO { ImageId = 0 };

    // Act
    var result = await _controller.UpdateImage(imageDto);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    Assert.Equal("Image ID not provided", badRequestResult.Value);
  }

  [Fact]
  public async Task UpdateImage_ReturnsNotFound_WhenImageDoesNotExist()
  {
    // Arrange
    var imageDto = new ImageDTO { ImageId = 1 };
    _mockImageRepository.Setup(repo => repo.GetImageById(1)).ReturnsAsync((Image?)null);

    // Act
    var result = await _controller.UpdateImage(imageDto);

    // Assert
    var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    Assert.Equal("Image not found", notFoundResult.Value);
  }

  [Fact]
  public async Task UpdateImage_ReturnsOk_WhenImageIsUpdatedSuccessfully()
  {
    // Arrange
    var existingImage = new Image { ImageId = 1, ImageName = "existing.jpg", ImagePath = "images/existing.jpg" };
    var imageDto = new ImageDTO { ImageId = 1, ImageName = "updated.jpg" };
    _mockImageRepository.Setup(repo => repo.GetImageById(1)).ReturnsAsync(existingImage);
    _mockImageRepository.Setup(repo => repo.Update(existingImage)).ReturnsAsync(true);

    // Act
    var result = await _controller.UpdateImage(imageDto);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.Equal("Image updated", okResult.Value);
  }

  [Fact]
  public async Task UpdateImage_ReturnsInternalServerError_WhenUpdateFails()
  {
    // Arrange
    var existingImage = new Image { ImageId = 1, ImageName = "existing.jpg", ImagePath = "images/existing.jpg" };
    var imageDto = new ImageDTO { ImageId = 1, ImageName = "updated.jpg" };
    _mockImageRepository.Setup(repo => repo.GetImageById(1)).ReturnsAsync(existingImage);
    _mockImageRepository.Setup(repo => repo.Update(existingImage)).ReturnsAsync(false);

    // Act
    var result = await _controller.UpdateImage(imageDto);

    // Assert
    var statusCodeResult = Assert.IsType<ObjectResult>(result);
    Assert.Equal(500, statusCodeResult.StatusCode);
    Assert.Equal("Image not updated", statusCodeResult.Value);
  }

  [Fact]
  public async Task UpdateImage_ReturnsOk_WhenNewImageFileIsProvided()
  {
    // Arrange
    var existingImage = new Image { ImageId = 1, ImageName = "existing.jpg", ImagePath = "images/existing.jpg" };
    var imageFileMock = new Mock<IFormFile>();
    imageFileMock.Setup(f => f.FileName).Returns("new.jpg");
    imageFileMock.Setup(f => f.Length).Returns(1);
    var imageDto = new ImageDTO { ImageId = 1, ImageName = "new.jpg", ImageFile = imageFileMock.Object };

    _mockImageRepository.Setup(repo => repo.GetImageById(1)).ReturnsAsync(existingImage);
    _mockImageRepository.Setup(repo => repo.Update(existingImage)).ReturnsAsync(true);
    _mockWebHostEnvironment.Setup(env => env.WebRootPath).Returns("wwwroot");

    var uploadFolder = Path.Combine("wwwroot", "images");
    Directory.CreateDirectory(uploadFolder);

    // Act
    var result = await _controller.UpdateImage(imageDto);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.Equal("Image updated", okResult.Value);

    // Clean up
    var filePath = Path.Combine(uploadFolder, "new.jpg");
    if (File.Exists(filePath))
    {
      File.Delete(filePath);
    }
  }

  [Fact]
  public async Task ToggleCarouselImage_ReturnsOk_WhenToggleIsSuccessful()
  {
    // Arrange
    var imageId = 1;
    var image = new Image
    {
      ImageId = imageId,
      ImageName = "test.jpg",
      IsCarouselImage = false
    };

    _mockImageRepository.Setup(repo => repo.GetImageById(imageId)).ReturnsAsync(image);
    _mockImageRepository.Setup(repo => repo.Update(It.IsAny<Image>())).ReturnsAsync(true);

    // Act
    var result = await _controller.ToggleCarouselImage(imageId);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.NotNull(okResult.Value);

    var response = Assert.IsType<ImageResponse>(okResult.Value);
    Assert.NotNull(response.Image);

    Assert.Equal("Carousel status updated successfully", response.Message);
    Assert.True(response.Image.IsCarouselImage); // Status skal være togglet til true
  }

  [Fact]
  public async Task ToggleCarouselImage_ReturnsNotFound_WhenImageDoesNotExist()
  {
    // Arrange
    var imageId = 1;

    _mockImageRepository.Setup(repo => repo.GetImageById(imageId)).ReturnsAsync((Image)null!);

    // Act
    var result = await _controller.ToggleCarouselImage(imageId);

    // Assert
    var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    Assert.Equal("Image not found", notFoundResult.Value);
  }

  [Fact]
  public async Task ToggleCarouselImage_ReturnsInternalServerError_WhenUpdateFails()
  {
    // Arrange
    var imageId = 1;
    var image = new Image
    {
      ImageId = imageId,
      ImageName = "test.jpg",
      IsCarouselImage = false
    };

    _mockImageRepository.Setup(repo => repo.GetImageById(imageId)).ReturnsAsync(image);
    _mockImageRepository.Setup(repo => repo.Update(It.IsAny<Image>())).ReturnsAsync(false);

    // Act
    var result = await _controller.ToggleCarouselImage(imageId);

    // Assert
    var statusCodeResult = Assert.IsType<ObjectResult>(result);
    Assert.Equal(500, statusCodeResult.StatusCode);
    Assert.Equal("Failed to update carousel status", statusCodeResult.Value);
  }

  [Fact]
  public async Task GetCarouselImages_ReturnsOk_WhenCarouselImagesExist()
  {
    // Arrange
    var dbImages = new List<Image>
    {
        new Image { ImageId = 1, ImageName = "carousel1.jpg", IsCarouselImage = true },
        new Image { ImageId = 2, ImageName = "carousel2.jpg", IsCarouselImage = true },
        new Image { ImageId = 3, ImageName = "noncarousel.jpg", IsCarouselImage = false }
    };

    _mockImageRepository.Setup(repo => repo.GetImages()).ReturnsAsync(dbImages);

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext
      {
        Request =
            {
                Scheme = "http",
                Host = new HostString("localhost")
            }
      }
    };

    // Act
    var result = await _controller.GetCarouselImages();

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var carouselImages = Assert.IsType<List<ImageDTO>>(okResult.Value);

    Assert.Equal(2, carouselImages.Count); // Kun karusellbilder
    Assert.All(carouselImages, img => Assert.True(img.IsCarouselImage));
    Assert.Contains(carouselImages, img => img.ImageName == "carousel1.jpg");
    Assert.Contains(carouselImages, img => img.ImageName == "carousel2.jpg");
  }

  [Fact]
  public async Task GetCarouselImages_ReturnsEmptyList_WhenNoCarouselImagesExist()
  {
    // Arrange
    var dbImages = new List<Image>
    {
        new Image { ImageId = 1, ImageName = "noncarousel1.jpg", IsCarouselImage = false },
        new Image { ImageId = 2, ImageName = "noncarousel2.jpg", IsCarouselImage = false }
    };

    _mockImageRepository.Setup(repo => repo.GetImages()).ReturnsAsync(dbImages);

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext
      {
        Request =
            {
                Scheme = "http",
                Host = new HostString("localhost")
            }
      }
    };

    // Act
    var result = await _controller.GetCarouselImages();

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var carouselImages = Assert.IsType<List<ImageDTO>>(okResult.Value);

    Assert.Empty(carouselImages); // Ingen karusellbilder
  }

  [Fact]
  public async Task GetCarouselImages_ReturnsNotFound_WhenDatabaseReturnsNull()
  {
    // Arrange
    _mockImageRepository.Setup(repo => repo.GetImages()).ReturnsAsync((List<Image>?)null!);

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext
      {
        Request =
            {
                Scheme = "http",
                Host = new HostString("localhost")
            }
      }
    };

    // Act
    var result = await _controller.GetCarouselImages();

    // Assert
    var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    Assert.Equal("No images found", notFoundResult.Value);
  }


  [Fact]
  public async Task DeleteImage_ReturnsNotFound_WhenImageDoesNotExist()
  {
    // Arrange
    _mockImageRepository.Setup(repo => repo.GetImageById(1)).ReturnsAsync(default(Image));

    // Act
    var result = await _controller.DeleteImage(1);

    // Assert
    var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    Assert.Equal("Image not found", notFoundResult.Value);
  }

  [Fact]
  public async Task DeleteImage_ReturnsOk_WhenFileDoesNotExist()
  {
    // Arrange
    var existingImage = new Image { ImageId = 1, ImageName = "existing.jpg", ImagePath = "images/existing.jpg" };
    _mockImageRepository.Setup(repo => repo.GetImageById(1)).ReturnsAsync(existingImage);
    _mockImageRepository.Setup(repo => repo.DeleteImage(1)).ReturnsAsync(true);
    _mockWebHostEnvironment.Setup(env => env.WebRootPath).Returns("wwwroot");

    var filePath = Path.Combine("wwwroot", existingImage.ImagePath);
    if (File.Exists(filePath))
    {
      File.Delete(filePath);
    }

    // Act
    var result = await _controller.DeleteImage(1);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
  }

  [Fact]
  public async Task DeleteImage_ReturnsOk_WhenFileIsDeletedSuccessfully()
  {
    // Arrange
    var existingImage = new Image { ImageId = 1, ImageName = "existing.jpg", ImagePath = "images/existing.jpg" };
    _mockImageRepository.Setup(repo => repo.GetImageById(1)).ReturnsAsync(existingImage);
    _mockImageRepository.Setup(repo => repo.DeleteImage(1)).ReturnsAsync(true);
    _mockWebHostEnvironment.Setup(env => env.WebRootPath).Returns("wwwroot");

    var filePath = Path.Combine("wwwroot", existingImage.ImagePath);
    var directoryPath = Path.GetDirectoryName(filePath);
    if (directoryPath != null)
    {
      Directory.CreateDirectory(directoryPath);
    }
    File.WriteAllText(filePath, "dummy content");

    // Act
    var result = await _controller.DeleteImage(1);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);

    // Clean up
    if (File.Exists(filePath))
    {
      File.Delete(filePath);
    }
  }

  [Fact]
  public async Task DeleteImage_ReturnsInternalServerError_WhenFileDeletionFails()
  {
    // Arrange
    var existingImage = new Image { ImageId = 1, ImageName = "existing.jpg", ImagePath = "images/existing.jpg" };
    _mockImageRepository.Setup(repo => repo.GetImageById(1)).ReturnsAsync(existingImage);
    _mockWebHostEnvironment.Setup(env => env.WebRootPath).Returns("wwwroot");

    var filePath = Path.Combine("wwwroot", existingImage.ImagePath);
    var directoryPath = Path.GetDirectoryName(filePath);
    if (directoryPath != null)
    {
      Directory.CreateDirectory(directoryPath);
    }
    File.WriteAllText(filePath, "dummy content");

    // Act
    var result = await _controller.DeleteImage(1);

    // Assert
    var statusCodeResult = Assert.IsType<ObjectResult>(result);

    // Clean up
    if (File.Exists(filePath))
    {
      File.Delete(filePath);
    }
  }

  [Fact]
  public async Task DeleteImage_ReturnsInternalServerError_WhenDeletionFails()
  {
    // Arrange
    var existingImage = new Image { ImageId = 1, ImageName = "existing.jpg", ImagePath = "images/existing.jpg" };
    _mockImageRepository.Setup(repo => repo.GetImageById(1)).ReturnsAsync(existingImage);
    _mockImageRepository.Setup(repo => repo.DeleteImage(1)).ReturnsAsync(false);
    _mockWebHostEnvironment.Setup(env => env.WebRootPath).Returns("wwwroot");

    var filePath = Path.Combine("wwwroot", existingImage.ImagePath);
    var directoryPath = Path.GetDirectoryName(filePath);
    if (directoryPath != null)
    {
      Directory.CreateDirectory(directoryPath);
    }
    File.WriteAllText(filePath, "dummy content");

    // Act
    var result = await _controller.DeleteImage(1);

    // Assert
    var statusCodeResult = Assert.IsType<ObjectResult>(result);
    Assert.Equal(500, statusCodeResult.StatusCode);
    Assert.Equal("Image not deleted", statusCodeResult.Value);

    // Clean up
    if (File.Exists(filePath))
    {
      File.Delete(filePath);
    }
  }

}