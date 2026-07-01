using DevFlow.Api.Data;
using DevFlow.Api.DTOs;
using DevFlow.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DevFlow.Api.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;

    public CategoryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length > 200)
        {
            throw new InvalidProductException("Category name is required and must be at most 200 characters.");
        }

        var category = new Category { Id = Guid.NewGuid(), Name = request.Name };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return new CategoryResponse { Id = category.Id, Name = category.Name };
    }

    public async Task<IReadOnlyList<CategoryResponse>> ListAsync()
    {
        var categories = await _context.Categories.ToListAsync();
        return categories.Select(c => new CategoryResponse { Id = c.Id, Name = c.Name }).ToList();
    }
}
