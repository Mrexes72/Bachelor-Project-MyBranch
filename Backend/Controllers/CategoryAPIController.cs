using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.DTOs;
using Backend.DAL;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryAPIController : Controller
{
  private readonly ICategoryRepository _categoryRepository;
  private readonly ILogger<CategoryAPIController> _logger;

  public CategoryAPIController(ICategoryRepository categoryRepository, ILogger<CategoryAPIController> logger)
  {
    _categoryRepository = categoryRepository;
    _logger = logger;
  }

  [HttpGet("categorieslist")]
  public async Task<IActionResult> GetCategories()
  {
    var categories = await _categoryRepository.GetCategories();
    if (categories == null)
    {
      _logger.LogError("[CategoryAPIController] Category list not found while executing _categoryRepository.GetCategories()");
      return NotFound("Category list not found");
    }

    var categoryDtos = categories
    .Where(category => category != null)
    .Select(category => new CategoryDTO
    {
      CategoryId = category!.CategoryId, // Ensure that CategoryId is not null
      Name = category.Name,
      Description = category.Description ?? string.Empty
    });
    return Ok(categoryDtos.ToList());
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetCategory(int id)
  {
    var category = await _categoryRepository.GetCategoryById(id);
    if (category == null)
    {
      _logger.LogError("[CategoryAPIController] Category not found while executing _categoryRepository.GetCategory(id)");
      return NotFound("Category not found");
    }

    var categoryDto = new CategoryDTO
    {
      CategoryId = category.CategoryId,
      Name = category.Name,
      Description = category.Description ?? string.Empty
    };

    return Ok(categoryDto);
  }

  [HttpPost("create")]
  public async Task<IActionResult> CreateCategory([FromBody] CategoryDTO categoryDto)
  {
    if (categoryDto == null)
    {
      _logger.LogError("[CategoryAPIController] Category is null while executing CreateCategory([FromBody] CategoryDTO categoryDto)");
      return BadRequest("Category is null");
    }

    var category = new Category
    {
      Name = categoryDto.Name,
      Description = categoryDto.Description
    };

    bool returnOk = await _categoryRepository.Create(category);
    if (returnOk)
    {
      var categoryDtoResponse = new CategoryDTO
      {
        CategoryId = category.CategoryId,
        Name = category.Name,
        Description = category.Description ?? string.Empty
      };
      return CreatedAtAction(nameof(GetCategory), new { id = category.CategoryId }, categoryDtoResponse);
    }
    else
    {
      _logger.LogError("[CategoryAPIController] Error while executing _categoryRepository.CreateCategory(category)");
      return StatusCode(500, "Internal server error");
    }
  }

  [HttpPut("update/{id}")]
  public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDTO categoryDto)
  {
    if (categoryDto == null || id != categoryDto.CategoryId)
    {
      _logger.LogError("[CategoryAPIController] Category is null or id does not match while executing UpdateCategory(int id, [FromBody] CategoryDTO categoryDto)");
      return BadRequest("Category is null or id does not match");
    }

    var category = new Category
    {
      CategoryId = categoryDto.CategoryId,
      Name = categoryDto.Name,
      Description = categoryDto.Description
    };

    bool returnOk = await _categoryRepository.Update(category);
    if (returnOk)
    {
      var categoryDtoResponse = new CategoryDTO
      {
        CategoryId = category.CategoryId,
        Name = category.Name,
        Description = category.Description ?? string.Empty
      };
      return Ok(categoryDtoResponse);
    }
    else
    {
      _logger.LogError("[CategoryAPIController] Error while executing _categoryRepository.UpdateCategory(category)");
      return StatusCode(500, "Internal server error");
    }
  }

  [HttpDelete("delete/{id}")]
  public async Task<IActionResult> DeleteCategory(int id)
  {
    bool returnOk = await _categoryRepository.DeleteCategory(id);
    if (returnOk)
    {
      return Ok("Category deleted");
    }
    else
    {
      _logger.LogError("[CategoryAPIController] Error while executing _categoryRepository.DeleteCategory(id)");
      return StatusCode(500, "Internal server error");
    }
  }
}