using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.DTOs;
using Backend.DAL;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IngredientAPIController : Controller
{
  private readonly IIngredientRepository _ingredientRepository;
  private readonly ILogger<IngredientAPIController> _logger;

  public IngredientAPIController(IIngredientRepository ingredientRepository, ILogger<IngredientAPIController> logger)
  {
    _ingredientRepository = ingredientRepository;
    _logger = logger;
  }

  [HttpGet("ingredientslist")]
  public async Task<IActionResult> GetIngredients()
  {
    var ingredients = await _ingredientRepository.GetIngredients();
    if (ingredients == null)
    {
      _logger.LogError("[IngredientAPIController] Ingredient list not found while executing _ingredientRepository.GetIngredients()");
      return NotFound("Ingredient list not found");
    }

    var ingredientDtos = ingredients
    .Where(ingredient => ingredient != null)
    .Select(ingredient => new IngredientDTO
    {
      IngredientId = ingredient!.IngredientId, // Ensure that IngredientId is not null
      Name = ingredient.Name,
      Description = ingredient.Description,
      Color = ingredient.Color,
      FillLevel = ingredient.FillLevel.HasValue ? ingredient.FillLevel.Value : 0, // Ensure that FillLevel is not null
      IsAvailable = ingredient.IsAvailable,
      UnitPrice = ingredient.UnitPrice,
      CategoryId = ingredient.CategoryId ?? 0,
      CategoryName = ingredient.Category?.Name ?? string.Empty, // Ensure that CategoryName is not null
      ImagePath = ingredient.ImagePath // Map the ImagePath property
    });
    return Ok(ingredientDtos);
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetIngredient(int id)
  {
    var ingredient = await _ingredientRepository.GetIngredientById(id);
    if (ingredient == null)
    {
      _logger.LogError("[IngredientAPIController] Ingredient not found while executing _ingredientRepository.GetIngredient(id)");
      return NotFound("Ingredient not found");
    }

    var ingredientDto = new IngredientDTO
    {
      IngredientId = ingredient.IngredientId,
      Name = ingredient.Name,
      Description = ingredient.Description,
      Color = ingredient.Color,
      FillLevel = ingredient.FillLevel.HasValue ? ingredient.FillLevel.Value : 0, // Ensure that FillLevel is not null
      IsAvailable = ingredient.IsAvailable,
      UnitPrice = ingredient.UnitPrice,
      CategoryId = ingredient.CategoryId ?? 0,
      CategoryName = ingredient.Category?.Name,
      ImagePath = ingredient.ImagePath
    };

    return Ok(ingredientDto);
  }

  [HttpPost("create")]
  public async Task<IActionResult> CreateIngredient([FromForm] IngredientDTO ingredientDto)
  {
    // Check if the ingredient is provided
    if (ingredientDto == null)
    {
      _logger.LogError("[IngredientAPIController] Ingredient is null while executing CreateIngredient([FromForm] IngredientDto ingredientDto)");
      return BadRequest("Ingredient is null");
    }

    // Handle image upload
    if (ingredientDto.ImageFile != null)
    {
      var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ingredient-images");
      if (!Directory.Exists(uploadFolder))
      {
        Directory.CreateDirectory(uploadFolder);
      }

      var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ingredientDto.ImageFile.FileName);
      var filePath = Path.Combine(uploadFolder, fileName);

      using (var fileStream = new FileStream(filePath, FileMode.Create))
      {
        await ingredientDto.ImageFile.CopyToAsync(fileStream);
      }

      ingredientDto.ImagePath = $"/ingredient-images/{fileName}"; // Save relative path
    }

    var ingredient = new Ingredient
    {
      Name = ingredientDto.Name,
      Description = ingredientDto.Description,
      IsAvailable = ingredientDto.IsAvailable,
      Color = ingredientDto.Color,
      FillLevel = ingredientDto.FillLevel,
      UnitPrice = ingredientDto.UnitPrice,
      CategoryId = ingredientDto.CategoryId,
      ImagePath = ingredientDto.ImagePath
    };

    bool returnOk = await _ingredientRepository.Create(ingredient);
    if (returnOk)
    {
      var createdIngredient = await _ingredientRepository.GetIngredientById(ingredient.IngredientId);
      // Check if the ingredient was created
      if (createdIngredient == null)
      {
        _logger.LogError("[IngredientAPIController] Ingredient not found while executing _ingredientRepository.GetIngredientById(ingredient.IngredientId)");
        return NotFound("Ingredient not found");
      }

      var ingredientDtoResponse = new IngredientDTO
      {
        IngredientId = createdIngredient.IngredientId,
        Name = createdIngredient.Name,
        Description = createdIngredient.Description,
        Color = createdIngredient.Color,
        IsAvailable = createdIngredient.IsAvailable,
        UnitPrice = createdIngredient.UnitPrice,
        CategoryId = createdIngredient.CategoryId ?? 0,
        CategoryName = createdIngredient.Category?.Name,
        ImagePath = createdIngredient.ImagePath
      };
      return CreatedAtAction(nameof(GetIngredient), new { id = ingredient.IngredientId }, ingredientDtoResponse);
    }
    else
    {
      _logger.LogError("[IngredientAPIController] Error while executing _ingredientRepository.CreateIngredient(ingredient)");
      return StatusCode(500, "Internal server error");
    }
  }

  [HttpPut("update/{id}")]
  public async Task<IActionResult> UpdateIngredient(int id, [FromForm] IngredientDTO ingredientDto)
  {
    if (ingredientDto == null || id != ingredientDto.IngredientId)
    {
      _logger.LogError($"[IngredientAPIController] Invalid request: Ingredient is null or ID mismatch (id: {id}, ingredientDto.IngredientId: {ingredientDto?.IngredientId})");
      return BadRequest("Invalid request: Ingredient is null or ID mismatch");
    }

    // Handle image upload
    if (ingredientDto.ImageFile != null)
    {
      var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ingredient-images");
      if (!Directory.Exists(uploadFolder))
      {
        Directory.CreateDirectory(uploadFolder);
      }

      var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ingredientDto.ImageFile.FileName);
      var filePath = Path.Combine(uploadFolder, fileName);

      using (var fileStream = new FileStream(filePath, FileMode.Create))
      {
        await ingredientDto.ImageFile.CopyToAsync(fileStream);
      }

      ingredientDto.ImagePath = $"/ingredient-images/{fileName}"; // Save relative path
    }
    else
    {
      // Retain the existing image path if no new image is uploaded
      var existingIngredient = await _ingredientRepository.GetIngredientById(id);
      if (existingIngredient != null)
      {
        ingredientDto.ImagePath = existingIngredient.ImagePath;
      }
    }

    var ingredient = new Ingredient
    {
      IngredientId = ingredientDto.IngredientId,
      Name = ingredientDto.Name,
      Description = ingredientDto.Description,
      Color = ingredientDto.Color,
      FillLevel = ingredientDto.FillLevel,
      IsAvailable = ingredientDto.IsAvailable,
      UnitPrice = ingredientDto.UnitPrice,
      CategoryId = ingredientDto.CategoryId,
      ImagePath = ingredientDto.ImagePath
    };

    bool returnOk = await _ingredientRepository.Update(ingredient);
    if (!returnOk)
    {
      _logger.LogError($"[IngredientAPIController] Failed to update ingredient with ID {id}");
      return StatusCode(500, "Internal server error");
    }

    var updatedIngredient = await _ingredientRepository.GetIngredientById(ingredient.IngredientId);
    if (updatedIngredient == null)
    {
      _logger.LogError($"[IngredientAPIController] Ingredient with ID {id} not found after update");
      return NotFound("Ingredient not found");
    }

    return Ok(updatedIngredient);
  }

  [HttpDelete("delete/{id}")]
  public async Task<IActionResult> DeleteIngredient(int id)
  {
    bool returnOk = await _ingredientRepository.DeleteIngredient(id);
    if (returnOk)
    {
      return Ok("Ingredient deleted");
    }
    else
    {
      _logger.LogError("[IngredientAPIController] Error while executing _ingredientRepository.DeleteIngredient(id)");
      return StatusCode(500, "Internal server error");
    }
  }

}