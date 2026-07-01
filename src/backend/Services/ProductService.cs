using DevFlow.Api.Data;
using DevFlow.Api.DTOs;
using DevFlow.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DevFlow.Api.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ProductResponse> CreateAsync(CreateProductRequest request)
    {
        ValidateProductFields(request.Name, request.Sku, request.Price);

        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
        {
            throw new CategoryNotFoundException();
        }

        var skuExists = await _context.Products.AnyAsync(p => p.Sku == request.Sku);
        if (skuExists)
        {
            throw new DuplicateSkuException();
        }

        var now = DateTime.UtcNow;
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Sku = request.Sku,
            Price = request.Price,
            Status = request.Status,
            CategoryId = request.CategoryId,
            BrandId = request.BrandId,
            CreatedAt = now,
            UpdatedAt = now,
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return ToResponse(product);
    }

    public async Task<ProductResponse> UpdateAsync(Guid id, UpdateProductRequest request)
    {
        ValidateProductFields(request.Name, sku: null, request.Price);

        var product = await _context.Products.FindAsync(id)
            ?? throw new ProductNotFoundException();

        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
        {
            throw new CategoryNotFoundException();
        }

        product.Name = request.Name;
        product.Price = request.Price;
        product.CategoryId = request.CategoryId;
        product.BrandId = request.BrandId;
        product.Status = request.Status;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return ToResponse(product);
    }

    public async Task<ProductResponse> ArchiveAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id)
            ?? throw new ProductNotFoundException();

        product.Status = ProductStatus.Archived;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return ToResponse(product);
    }

    public async Task<ProductResponse> GetByIdAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id)
            ?? throw new ProductNotFoundException();

        return ToResponse(product);
    }

    public async Task<IReadOnlyList<ProductResponse>> ListAsync()
    {
        var products = await _context.Products.ToListAsync();
        return products.Select(ToResponse).ToList();
    }

    private static void ValidateProductFields(string name, string? sku, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > 200)
        {
            throw new InvalidProductException("Product name is required and must be at most 200 characters.");
        }

        if (sku is not null && string.IsNullOrWhiteSpace(sku))
        {
            throw new InvalidProductException("SKU is required.");
        }

        if (price < 0.01m)
        {
            throw new InvalidProductException("Price must be greater than zero.");
        }
    }

    private static ProductResponse ToResponse(Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Sku = product.Sku,
        Price = product.Price,
        Status = product.Status,
        CategoryId = product.CategoryId,
        BrandId = product.BrandId,
        CreatedAt = product.CreatedAt,
        UpdatedAt = product.UpdatedAt,
    };
}
