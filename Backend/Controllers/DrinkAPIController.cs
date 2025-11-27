using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.DTOs;
using Backend.DAL;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DrinkAPIController : Controller
{
  private readonly IDrinkRepository _drinkRepository;
  private readonly ILogger<DrinkAPIController> _logger;

  public DrinkAPIController(IDrinkRepository drinkRepository, ILogger<DrinkAPIController> logger)
  {
    _drinkRepository = drinkRepository;
    _logger = logger;
  }

  [HttpGet("drinkslist")]
  public async Task<IActionResult> GetDrinks()
  {
    var drinks = await _drinkRepository.GetDrinks();
    if (drinks == null)
    {
      _logger.LogError("[DrinkAPIController] Drink list not found while executing _drinkRepository.GetDrinks()");
      return NotFound("Drink list not found");
    }

    var drinkDtos = drinks
    .Where(drink => drink != null)
    .Select(drink => new DrinkDTO
    {
      DrinkId = drink!.DrinkId, // Ensure that DrinkId is not null
      Name = drink.Name,
      BasePrice = drink.BasePrice,
      SalePrice = drink.SalePrice,
      TimesFavorite = drink.TimesFavorite,
      CreatedByUserId = drink.CreatedByUserId,
      CategoryId = drink.CategoryId,
      ImagePath = drink.ImagePath,
      IngredientDTOs = drink.Ingredients?.Select(ingredient => new IngredientDTO
      {
        IngredientId = ingredient.IngredientId,
        Name = ingredient.Name,
        Description = ingredient.Description,
        Color = ingredient.Color,
        IsAvailable = ingredient.IsAvailable,
        UnitPrice = ingredient.UnitPrice,
        CategoryId = ingredient.CategoryId.HasValue ? ingredient.CategoryId.Value : 0 // Ensure that CategoryId is not null
      }).ToList() ?? new List<IngredientDTO>() // Use an empty list if Ingredients is null
    });
    return Ok(drinkDtos);
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetDrink(int id)
  {
    var drink = await _drinkRepository.GetDrinkById(id);
    if (drink == null)
    {
      _logger.LogError("[DrinkAPIController] Drink not found while executing _drinkRepository.GetDrink(id)");
      return NotFound("Drink not found");
    }

    var drinkDto = new DrinkDTO
    {
      DrinkId = drink.DrinkId,
      Name = drink.Name,
      BasePrice = drink.BasePrice,
      SalePrice = drink.SalePrice,
      TimesFavorite = drink.TimesFavorite,
      CreatedByUserId = drink.CreatedByUserId,
      CategoryId = drink.CategoryId,
      ImagePath = drink.ImagePath,
      IngredientDTOs = drink.Ingredients?.Select(ingredient => new IngredientDTO
      {
        IngredientId = ingredient.IngredientId,
        Name = ingredient.Name,
        Description = ingredient.Description,
        Color = ingredient.Color,
        IsAvailable = ingredient.IsAvailable,
        UnitPrice = ingredient.UnitPrice,
        CategoryId = ingredient.CategoryId.HasValue ? ingredient.CategoryId.Value : 0
      }).ToList() ?? new List<IngredientDTO>()
    };
    return Ok(drinkDto);
  }

  [HttpPost("create")]
  public async Task<IActionResult> CreateDrink([FromBody] DrinkDTO drinkDto)
  {
    _logger.LogInformation("Received drink DTO: {@drinkDto}", drinkDto);

    if (drinkDto == null)
    {
      _logger.LogError("[DrinkAPIController] Drink is null while executing CreateDrink([FromBody] DrinkDTO drinkDto)");
      return BadRequest("Received drink data is null");
    }

    if (string.IsNullOrEmpty(drinkDto.Name))
    {
      _logger.LogError("[DrinkAPIController] Drink name is missing");
      return BadRequest("Drink name is required");
    }

    if (drinkDto.IngredientDTOs == null || !drinkDto.IngredientDTOs.Any())
    {
      _logger.LogError("[DrinkAPIController] No ingredients provided");
      return BadRequest("At least one ingredient is required");
    }

    // Handle ImagePath (optional: save as a file)
    string? imagePath = null;
    if (!string.IsNullOrEmpty(drinkDto.ImagePath))
    {
      try
      {
        // Decode the base64 string and save it as a file
        var base64Data = drinkDto.ImagePath.Split(',')[1];
        var imageBytes = Convert.FromBase64String(base64Data);
        var fileName = $"{Guid.NewGuid()}.png";
        var filePath = Path.Combine("wwwroot/images", fileName);

        // Ensure the directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);
        imagePath = $"/images/{fileName}"; // Save the relative path
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "[DrinkAPIController] Failed to process ImagePath");
        return BadRequest("Failed to process the image");
      }
    }

    // Fetch existing ingredients to prevent duplicate tracking
    var ingredientIds = drinkDto.IngredientDTOs.Select(i => i.IngredientId).ToList();
    var existingIngredients = await _drinkRepository.GetIngredientsByIds(ingredientIds);

    var drink = new Drink
    {
      Name = drinkDto.Name,
      BasePrice = drinkDto.BasePrice,
      SalePrice = drinkDto.SalePrice,
      TimesFavorite = drinkDto.TimesFavorite,
      CreatedByUserId = drinkDto.CreatedByUserId,
      CategoryId = drinkDto.CategoryId,
      Ingredients = existingIngredients,
      ImagePath = imagePath ?? drinkDto.ImagePath // Use the file path or the original base64 string
    };

    var result = await _drinkRepository.Create(drink);
    if (!result)
    {
      _logger.LogError("[DrinkAPIController] Drink creation failed while executing _drinkRepository.Create(drink)");
      return BadRequest("Drink creation failed");
    }

    return Ok(new { message = "Drink created successfully", drink });
  }


  [HttpPut("update/{id}")]
  public async Task<IActionResult> UpdateDrink(int id, [FromBody] DrinkDTO drinkDto)
  {
    if (drinkDto == null || id != drinkDto.DrinkId)
    {
      _logger.LogError("[DrinkAPIController] Drink is null while executing UpdateDrink([FromBody] DrinkDto drinkDto)");
      return BadRequest("Drink is null");
    }

    var drink = new Drink
    {
      DrinkId = drinkDto.DrinkId,
      Name = drinkDto.Name,
      BasePrice = drinkDto.BasePrice,
      SalePrice = drinkDto.SalePrice,
      TimesFavorite = drinkDto.TimesFavorite,
      CreatedByUserId = drinkDto.CreatedByUserId,
      CategoryId = drinkDto.CategoryId,
      ImagePath = drinkDto.ImagePath,
      Ingredients = drinkDto.IngredientDTOs?.Select(ingredientDto => new Ingredient
      {
        IngredientId = ingredientDto.IngredientId,
        Name = ingredientDto.Name,
        Description = ingredientDto.Description,
        Color = ingredientDto.Color,
        FillLevel = ingredientDto.FillLevel,
        IsAvailable = ingredientDto.IsAvailable,
        UnitPrice = ingredientDto.UnitPrice,
        CategoryId = ingredientDto.CategoryId
      }).ToList() ?? new List<Ingredient>()
    };

    var result = await _drinkRepository.Update(drink);
    if (!result)
    {
      _logger.LogError("[DrinkAPIController] Drink update failed while executing _drinkRepository.Update(drink)");
      return BadRequest("Drink update failed");
    }

    return Ok("Drink updated");
  }

  [HttpDelete("delete/{id}")]
  public async Task<IActionResult> DeleteDrink(int id)
  {
    bool returnOk = await _drinkRepository.DeleteDrink(id);
    if (returnOk)
    {
      return Ok("Drink deleted");
    }
    else
    {
      _logger.LogError("[DrinkAPIController] Drink deletion failed while executing _drinkRepository.DeleteDrink(id)");
      return StatusCode(500, "Internal server error");
    }
  }

  [HttpPost("upvote/{id}")]
  public async Task<IActionResult> UpvoteDrink(int id)
  {
    var drink = await _drinkRepository.GetDrinkById(id);
    if (drink == null)
    {
      _logger.LogError($"[DrinkAPIController] Drink with ID {id} not found while executing UpvoteDrink.");
      return NotFound("Drink not found");
    }

    // Increment the timesFavorite count
    drink.TimesFavorite = (drink.TimesFavorite ?? 0) + 1;

    var result = await _drinkRepository.Update(drink);
    if (!result)
    {
      _logger.LogError($"[DrinkAPIController] Failed to update drink with ID {id} while executing UpvoteDrink.");
      return StatusCode(500, "Failed to upvote drink");
    }

    // Return a valid JSON response
    return Ok(new { message = "Drink upvoted successfully", timesFavorite = drink.TimesFavorite });
  }

  [HttpPost("remove-upvote/{id}")]
  public async Task<IActionResult> RemoveUpvote(int id)
  {
    var drink = await _drinkRepository.GetDrinkById(id);
    if (drink == null)
    {
      _logger.LogError($"[DrinkAPIController] Drink with ID {id} not found while executing RemoveUpvote.");
      return NotFound("Drink not found");
    }

    // Decrement the timesFavorite count, ensuring it doesn't go below 0
    drink.TimesFavorite = Math.Max((drink.TimesFavorite ?? 0) - 1, 0);

    var result = await _drinkRepository.Update(drink);
    if (!result)
    {
      _logger.LogError($"[DrinkAPIController] Failed to update drink with ID {id} while executing RemoveUpvote.");
      return StatusCode(500, "Failed to remove upvote");
    }

    return Ok(new { message = "Upvote removed successfully", timesFavorite = drink.TimesFavorite });
  }

}