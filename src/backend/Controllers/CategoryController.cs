using DevFlow.Api.DTOs;
using DevFlow.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/categories")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryResponse>>> List()
    {
        var result = await _categoryService.ListAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> Create(CreateCategoryRequest request)
    {
        try
        {
            var result = await _categoryService.CreateAsync(request);
            return CreatedAtAction(nameof(List), result);
        }
        catch (InvalidProductException ex)
        {
            return BadRequest(new ProblemDetails { Title = "Category creation failed", Detail = ex.Message, Status = StatusCodes.Status400BadRequest });
        }
    }
}
