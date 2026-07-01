using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using DevFlow.Api.Data;
using DevFlow.Api.DTOs;
using DevFlow.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DevFlow.Api.Tests.Controllers;

public class ProductControllerTests : IClassFixture<AuthApiFactory>
{
    private readonly AuthApiFactory _factory;
    private const string ValidPassword = "Str0ng!Pass";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public ProductControllerTests(AuthApiFactory factory)
    {
        _factory = factory;
    }

    private Guid SeedCategory()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var category = new Category { Id = Guid.NewGuid(), Name = "Electronics" };
        context.Categories.Add(category);
        context.SaveChanges();
        return category.Id;
    }

    private async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = _factory.CreateClient();
        var registerResponse = await client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest
        {
            Email = $"{Guid.NewGuid()}@example.com",
            Password = ValidPassword,
            FirstName = "Admin",
            LastName = "User",
        });
        var body = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body!.AccessToken);
        return client;
    }

    [Fact]
    public async Task Create_ValidRequest_Returns201()
    {
        var client = await CreateAuthenticatedClientAsync();
        var categoryId = SeedCategory();

        var response = await client.PostAsJsonAsync("/api/v1/products", new CreateProductRequest
        {
            Name = "Wireless Mouse",
            Sku = $"SKU-{Guid.NewGuid()}",
            Price = 29.99m,
            CategoryId = categoryId,
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Create_DuplicateSku_Returns409()
    {
        var client = await CreateAuthenticatedClientAsync();
        var categoryId = SeedCategory();
        var sku = $"SKU-{Guid.NewGuid()}";
        await client.PostAsJsonAsync("/api/v1/products", new CreateProductRequest { Name = "A", Sku = sku, Price = 10m, CategoryId = categoryId });

        var response = await client.PostAsJsonAsync("/api/v1/products", new CreateProductRequest { Name = "B", Sku = sku, Price = 20m, CategoryId = categoryId });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Create_InvalidPrice_Returns400()
    {
        var client = await CreateAuthenticatedClientAsync();
        var categoryId = SeedCategory();

        var response = await client.PostAsJsonAsync("/api/v1/products", new CreateProductRequest
        {
            Name = "Free Item", Sku = $"SKU-{Guid.NewGuid()}", Price = 0m, CategoryId = categoryId,
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_UnknownProduct_Returns404()
    {
        var client = await CreateAuthenticatedClientAsync();

        var response = await client.GetAsync($"/api/v1/products/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ExistingProduct_Returns200()
    {
        var client = await CreateAuthenticatedClientAsync();
        var categoryId = SeedCategory();
        var createResponse = await client.PostAsJsonAsync("/api/v1/products", new CreateProductRequest
        {
            Name = "Wireless Mouse", Sku = $"SKU-{Guid.NewGuid()}", Price = 29.99m, CategoryId = categoryId,
        });
        var created = await createResponse.Content.ReadFromJsonAsync<ProductResponse>(JsonOptions);

        var response = await client.PutAsJsonAsync($"/api/v1/products/{created!.Id}", new UpdateProductRequest
        {
            Name = "Wireless Mouse Pro", Price = 39.99m, CategoryId = categoryId, Status = ProductStatus.Active,
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingProduct_ArchivesAndReturns200()
    {
        var client = await CreateAuthenticatedClientAsync();
        var categoryId = SeedCategory();
        var createResponse = await client.PostAsJsonAsync("/api/v1/products", new CreateProductRequest
        {
            Name = "Wireless Mouse", Sku = $"SKU-{Guid.NewGuid()}", Price = 29.99m, CategoryId = categoryId,
        });
        var created = await createResponse.Content.ReadFromJsonAsync<ProductResponse>(JsonOptions);

        var response = await client.DeleteAsync($"/api/v1/products/{created!.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var archived = await response.Content.ReadFromJsonAsync<ProductResponse>(JsonOptions);
        Assert.Equal(ProductStatus.Archived, archived!.Status);
    }

    [Fact]
    public async Task List_WithoutAuthentication_Returns401()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/products");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Create_ValidRequest_SerializesStatusAsString()
    {
        var client = await CreateAuthenticatedClientAsync();
        var categoryId = SeedCategory();

        var response = await client.PostAsJsonAsync("/api/v1/products", new CreateProductRequest
        {
            Name = "Wireless Mouse", Sku = $"SKU-{Guid.NewGuid()}", Price = 29.99m, CategoryId = categoryId,
        });

        var rawBody = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"status\":\"Draft\"", rawBody);
    }

    [Fact]
    public async Task Create_WithoutAuthentication_Returns401()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/products", new CreateProductRequest
        {
            Name = "Unauthorized Product", Sku = "SKU-NOAUTH", Price = 10m, CategoryId = Guid.NewGuid(),
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
