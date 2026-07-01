using DevFlow.Api.Data;
using DevFlow.Api.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DevFlow.Api.Tests.Data;

public class ProductCatalogDbContextTests
{
    private static AppDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options);
    }

    private static Category CreateCategory() => new()
    {
        Id = Guid.NewGuid(),
        Name = "Electronics",
    };

    [Fact]
    public void AddProduct_ValidProduct_PersistsToDatabase()
    {
        using var context = CreateContext(nameof(AddProduct_ValidProduct_PersistsToDatabase));
        var category = CreateCategory();
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Wireless Mouse",
            Sku = "SKU-001",
            Price = 29.99m,
            Status = ProductStatus.Draft,
            CategoryId = category.Id,
            Category = category,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        context.Categories.Add(category);
        context.Products.Add(product);
        context.SaveChanges();

        Assert.Single(context.Products);
    }

    [Fact]
    public void Product_Sku_HasUniqueIndexConfigured()
    {
        using var context = CreateContext(nameof(Product_Sku_HasUniqueIndexConfigured));

        var entityType = context.Model.FindEntityType(typeof(Product));
        var skuIndex = entityType!.GetIndexes().Single(i => i.Properties.Single().Name == nameof(Product.Sku));

        Assert.True(skuIndex.IsUnique);
    }

    [Fact]
    public void Product_WithoutBrand_PersistsWithNullBrandId()
    {
        using var context = CreateContext(nameof(Product_WithoutBrand_PersistsWithNullBrandId));
        var category = CreateCategory();
        context.Categories.Add(category);

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Generic Cable",
            Sku = "SKU-002",
            Price = 5.99m,
            CategoryId = category.Id,
            BrandId = null,
        };
        context.Products.Add(product);
        context.SaveChanges();

        var stored = context.Products.Single();
        Assert.Null(stored.BrandId);
    }

    [Fact]
    public void Product_DefaultStatus_IsDraft()
    {
        var product = new Product();

        Assert.Equal(ProductStatus.Draft, product.Status);
    }
}
