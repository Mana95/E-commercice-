using System.Net;
using System.Net.Http.Json;
using DevFlow.Api.DTOs;
using Xunit;

namespace DevFlow.Api.Tests.Controllers;

public class AuthControllerTests : IClassFixture<AuthApiFactory>
{
    private readonly HttpClient _client;
    private const string ValidPassword = "Str0ng!Pass";

    public AuthControllerTests(AuthApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ValidRequest_Returns201WithUserId()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest
        {
            Email = $"{Guid.NewGuid()}@example.com",
            Password = ValidPassword,
            FirstName = "Ada",
            LastName = "Lovelace"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(body);
        Assert.NotEqual(Guid.Empty, body!.UserId);
    }

    [Fact]
    public async Task Register_DuplicateEmail_Returns409()
    {
        var email = $"{Guid.NewGuid()}@example.com";
        var request = new RegisterRequest { Email = email, Password = ValidPassword, FirstName = "Ada", LastName = "Lovelace" };
        await _client.PostAsJsonAsync("/api/v1/auth/register", request);

        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Register_WeakPassword_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest
        {
            Email = $"{Guid.NewGuid()}@example.com",
            Password = "weak",
            FirstName = "Ada",
            LastName = "Lovelace"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_ValidCredentials_Returns200WithTokens()
    {
        var email = $"{Guid.NewGuid()}@example.com";
        await _client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest
        {
            Email = email, Password = ValidPassword, FirstName = "Ada", LastName = "Lovelace"
        });

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest { Email = email, Password = ValidPassword });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.False(string.IsNullOrEmpty(body!.AccessToken));
    }

    [Fact]
    public async Task Login_InvalidCredentials_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest
        {
            Email = "nonexistent@example.com",
            Password = ValidPassword
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_ValidToken_Returns200WithNewTokens()
    {
        var email = $"{Guid.NewGuid()}@example.com";
        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest
        {
            Email = email, Password = ValidPassword, FirstName = "Ada", LastName = "Lovelace"
        });
        var registerBody = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();

        var response = await _client.PostAsJsonAsync("/api/v1/auth/refresh-token", new RefreshTokenRequest { RefreshToken = registerBody!.RefreshToken });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_InvalidToken_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/auth/refresh-token", new RefreshTokenRequest { RefreshToken = "invalid-token" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Logout_ValidToken_Returns200()
    {
        var email = $"{Guid.NewGuid()}@example.com";
        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest
        {
            Email = email, Password = ValidPassword, FirstName = "Ada", LastName = "Lovelace"
        });
        var registerBody = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();

        var response = await _client.PostAsJsonAsync("/api/v1/auth/logout", new RefreshTokenRequest { RefreshToken = registerBody!.RefreshToken });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
