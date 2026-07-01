using DevFlow.Api.Data;
using DevFlow.Api.DTOs;
using DevFlow.Api.Models;
using DevFlow.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DevFlow.Api.Tests.Services;

public class ProductServiceTests
{
    private static (ProductService service, AppDbContext context, Guid categoryId) CreateService(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        var context = new AppDbContext(options);

        var category = new Category { Id = Guid.NewGuid(), Name = "Electronics" };
        context.Categories.Add(category);
        context.SaveChanges();

        return (new ProductService(context), context, category.Id);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_PersistsProduct()
    {
        var (service, context, categoryId) = CreateService(nameof(CreateAsync_ValidRequest_PersistsProduct));

        var result = await service.CreateAsync(new CreateProductRequest
        {
            Name = "Wireless Mouse",
            Sku = "SKU-100",
            Price = 29.99m,
            CategoryId = categoryId,
        });

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(ProductStatus.Draft, result.Status);
        Assert.Single(context.Products);
    }

    [Fact]
    public async Task CreateAsync_DuplicateSku_ThrowsDuplicateSkuException()
    {
        var (service, _, categoryId) = CreateService(nameof(CreateAsync_DuplicateSku_ThrowsDuplicateSkuException));
        await service.CreateAsync(new CreateProductRequest { Name = "Wireless Mouse", Sku = "SKU-DUP", Price = 29.99m, CategoryId = categoryId });

        await Assert.ThrowsAsync<DuplicateSkuException>(() => service.CreateAsync(new CreateProductRequest
        {
            Name = "Wireless Keyboard",
            Sku = "SKU-DUP",
            Price = 49.99m,
            CategoryId = categoryId,
        }));
    }

    [Fact]
    public async Task CreateAsync_ZeroPrice_ThrowsInvalidProductException()
    {
        var (service, _, categoryId) = CreateService(nameof(CreateAsync_ZeroPrice_ThrowsInvalidProductException));

        await Assert.ThrowsAsync<InvalidProductException>(() => service.CreateAsync(new CreateProductRequest
        {
            Name = "Free Item",
            Sku = "SKU-ZERO",
            Price = 0m,
            CategoryId = categoryId,
        }));
    }

    [Fact]
    public async Task CreateAsync_NegativePrice_ThrowsInvalidProductException()
    {
        var (service, _, categoryId) = CreateService(nameof(CreateAsync_NegativePrice_ThrowsInvalidProductException));

        await Assert.ThrowsAsync<InvalidProductException>(() => service.CreateAsync(new CreateProductRequest
        {
            Name = "Negative Item",
            Sku = "SKU-NEG",
            Price = -5m,
            CategoryId = categoryId,
        }));
    }

    [Fact]
    public async Task CreateAsync_MissingName_ThrowsInvalidProductException()
    {
        var (service, _, categoryId) = CreateService(nameof(CreateAsync_MissingName_ThrowsInvalidProductException));

        await Assert.ThrowsAsync<InvalidProductException>(() => service.CreateAsync(new CreateProductRequest
        {
            Name = "",
            Sku = "SKU-NONAME",
            Price = 10m,
            CategoryId = categoryId,
        }));
    }

    [Fact]
    public async Task CreateAsync_UnknownCategory_ThrowsCategoryNotFoundException()
    {
        var (service, _, _) = CreateService(nameof(CreateAsync_UnknownCategory_ThrowsCategoryNotFoundException));

        await Assert.ThrowsAsync<CategoryNotFoundException>(() => service.CreateAsync(new CreateProductRequest
        {
            Name = "Orphan Product",
            Sku = "SKU-ORPHAN",
            Price = 10m,
            CategoryId = Guid.NewGuid(),
        }));
    }

    [Fact]
    public async Task UpdateAsync_ExistingProduct_UpdatesFields()
    {
        var (service, _, categoryId) = CreateService(nameof(UpdateAsync_ExistingProduct_UpdatesFields));
        var created = await service.CreateAsync(new CreateProductRequest
        {
            Name = "Wireless Mouse", Sku = "SKU-UPD", Price = 29.99m, CategoryId = categoryId,
        });

        var updated = await service.UpdateAsync(created.Id, new UpdateProductRequest
        {
            Name = "Wireless Mouse Pro",
            Price = 39.99m,
            CategoryId = categoryId,
            Status = ProductStatus.Active,
        });

        Assert.Equal("Wireless Mouse Pro", updated.Name);
        Assert.Equal(39.99m, updated.Price);
        Assert.Equal(ProductStatus.Active, updated.Status);
    }

    [Fact]
    public async Task UpdateAsync_UnknownProduct_ThrowsProductNotFoundException()
    {
        var (service, _, categoryId) = CreateService(nameof(UpdateAsync_UnknownProduct_ThrowsProductNotFoundException));

        await Assert.ThrowsAsync<ProductNotFoundException>(() => service.UpdateAsync(Guid.NewGuid(), new UpdateProductRequest
        {
            Name = "Ghost", Price = 10m, CategoryId = categoryId, Status = ProductStatus.Draft,
        }));
    }

    [Fact]
    public async Task ArchiveAsync_ExistingProduct_SetsStatusArchivedWithoutDeleting()
    {
        var (service, context, categoryId) = CreateService(nameof(ArchiveAsync_ExistingProduct_SetsStatusArchivedWithoutDeleting));
        var created = await service.CreateAsync(new CreateProductRequest
        {
            Name = "Wireless Mouse", Sku = "SKU-ARCH", Price = 29.99m, CategoryId = categoryId,
        });

        var archived = await service.ArchiveAsync(created.Id);

        Assert.Equal(ProductStatus.Archived, archived.Status);
        Assert.Single(context.Products);
    }

    [Fact]
    public async Task GetByIdAsync_UnknownProduct_ThrowsProductNotFoundException()
    {
        var (service, _, _) = CreateService(nameof(GetByIdAsync_UnknownProduct_ThrowsProductNotFoundException));

        await Assert.ThrowsAsync<ProductNotFoundException>(() => service.GetByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task ListAsync_ReturnsAllProductsRegardlessOfStatus()
    {
        var (service, _, categoryId) = CreateService(nameof(ListAsync_ReturnsAllProductsRegardlessOfStatus));
        await service.CreateAsync(new CreateProductRequest { Name = "Draft Product", Sku = "SKU-A", Price = 10m, CategoryId = categoryId, Status = ProductStatus.Draft });
        var active = await service.CreateAsync(new CreateProductRequest { Name = "Active Product", Sku = "SKU-B", Price = 20m, CategoryId = categoryId, Status = ProductStatus.Active });
        await service.ArchiveAsync(active.Id);

        var result = await service.ListAsync();

        Assert.Equal(2, result.Count);
    }
}
