using DevFlow.Api.DTOs;
using DevFlow.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/products")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create(CreateProductRequest request)
    {
        try
        {
            var result = await _productService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (DuplicateSkuException ex)
        {
            return Conflict(new ProblemDetails { Title = "Product creation failed", Detail = ex.Message, Status = StatusCodes.Status409Conflict });
        }
        catch (InvalidProductException ex)
        {
            return BadRequest(new ProblemDetails { Title = "Product creation failed", Detail = ex.Message, Status = StatusCodes.Status400BadRequest });
        }
        catch (CategoryNotFoundException ex)
        {
            return NotFound(new ProblemDetails { Title = "Product creation failed", Detail = ex.Message, Status = StatusCodes.Status404NotFound });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductResponse>>> List()
    {
        var result = await _productService.ListAsync();
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> GetById(Guid id)
    {
        try
        {
            var result = await _productService.GetByIdAsync(id);
            return Ok(result);
        }
        catch (ProductNotFoundException ex)
        {
            return NotFound(new ProblemDetails { Title = "Product not found", Detail = ex.Message, Status = StatusCodes.Status404NotFound });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> Update(Guid id, UpdateProductRequest request)
    {
        try
        {
            var result = await _productService.UpdateAsync(id, request);
            return Ok(result);
        }
        catch (ProductNotFoundException ex)
        {
            return NotFound(new ProblemDetails { Title = "Product update failed", Detail = ex.Message, Status = StatusCodes.Status404NotFound });
        }
        catch (InvalidProductException ex)
        {
            return BadRequest(new ProblemDetails { Title = "Product update failed", Detail = ex.Message, Status = StatusCodes.Status400BadRequest });
        }
        catch (CategoryNotFoundException ex)
        {
            return NotFound(new ProblemDetails { Title = "Product update failed", Detail = ex.Message, Status = StatusCodes.Status404NotFound });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> Archive(Guid id)
    {
        try
        {
            var result = await _productService.ArchiveAsync(id);
            return Ok(result);
        }
        catch (ProductNotFoundException ex)
        {
            return NotFound(new ProblemDetails { Title = "Product archive failed", Detail = ex.Message, Status = StatusCodes.Status404NotFound });
        }
    }
}
