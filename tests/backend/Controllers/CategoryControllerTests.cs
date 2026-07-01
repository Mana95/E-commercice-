using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using DevFlow.Api.DTOs;
using Xunit;

namespace DevFlow.Api.Tests.Controllers;

public class CategoryControllerTests : IClassFixture<AuthApiFactory>
{
    private readonly AuthApiFactory _factory;
    private const string ValidPassword = "Str0ng!Pass";

    public CategoryControllerTests(AuthApiFactory factory)
    {
        _factory = factory;
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

        var response = await client.PostAsJsonAsync("/api/v1/categories", new CreateCategoryRequest { Name = "Electronics" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Create_EmptyName_Returns400()
    {
        var client = await CreateAuthenticatedClientAsync();

        var response = await client.PostAsJsonAsync("/api/v1/categories", new CreateCategoryRequest { Name = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task List_ReturnsCreatedCategories()
    {
        var client = await CreateAuthenticatedClientAsync();
        await client.PostAsJsonAsync("/api/v1/categories", new CreateCategoryRequest { Name = "Books" });

        var response = await client.GetAsync("/api/v1/categories");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var categories = await response.Content.ReadFromJsonAsync<List<CategoryResponse>>();
        Assert.Contains(categories!, c => c.Name == "Books");
    }

    [Fact]
    public async Task List_WithoutAuthentication_Returns401()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/categories");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
