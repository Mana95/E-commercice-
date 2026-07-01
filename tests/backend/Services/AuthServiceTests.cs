using DevFlow.Api.Data;
using DevFlow.Api.DTOs;
using DevFlow.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Xunit;

namespace DevFlow.Api.Tests.Services;

public class AuthServiceTests
{
    private const string ValidPassword = "Str0ng!Pass";

    private static (AuthService service, AppDbContext context) CreateService(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        var context = new AppDbContext(options);

        var jwtSettings = new JwtSettings
        {
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            Key = "test-signing-key-at-least-32-characters-long",
            AccessTokenExpiryMinutes = 60,
            RefreshTokenExpiryDays = 7
        };
        var tokenService = new JwtTokenService(Options.Create(jwtSettings));
        var service = new AuthService(context, tokenService, Options.Create(jwtSettings));

        return (service, context);
    }

    [Fact]
    public async Task RegisterAsync_ValidRequest_CreatesUserAndReturnsTokens()
    {
        var (service, context) = CreateService(nameof(RegisterAsync_ValidRequest_CreatesUserAndReturnsTokens));
        var request = new RegisterRequest
        {
            Email = "new.customer@example.com",
            Password = ValidPassword,
            FirstName = "Ada",
            LastName = "Lovelace"
        };

        var result = await service.RegisterAsync(request);

        Assert.NotEqual(Guid.Empty, result.UserId);
        Assert.False(string.IsNullOrEmpty(result.AccessToken));
        Assert.False(string.IsNullOrEmpty(result.RefreshToken));
        var storedUser = await context.Users.SingleAsync();
        Assert.NotEqual(ValidPassword, storedUser.PasswordHash);
    }

    [Fact]
    public async Task RegisterAsync_DuplicateEmail_ThrowsDuplicateEmailException()
    {
        var (service, _) = CreateService(nameof(RegisterAsync_DuplicateEmail_ThrowsDuplicateEmailException));
        var request = new RegisterRequest
        {
            Email = "dupe@example.com",
            Password = ValidPassword,
            FirstName = "Ada",
            LastName = "Lovelace"
        };
        await service.RegisterAsync(request);

        await Assert.ThrowsAsync<DuplicateEmailException>(() => service.RegisterAsync(request));
    }

    [Fact]
    public async Task RegisterAsync_WeakPassword_ThrowsWeakPasswordException()
    {
        var (service, _) = CreateService(nameof(RegisterAsync_WeakPassword_ThrowsWeakPasswordException));
        var request = new RegisterRequest
        {
            Email = "weak@example.com",
            Password = "weak",
            FirstName = "Ada",
            LastName = "Lovelace"
        };

        await Assert.ThrowsAsync<WeakPasswordException>(() => service.RegisterAsync(request));
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsTokens()
    {
        var (service, _) = CreateService(nameof(LoginAsync_ValidCredentials_ReturnsTokens));
        await service.RegisterAsync(new RegisterRequest
        {
            Email = "login@example.com",
            Password = ValidPassword,
            FirstName = "Ada",
            LastName = "Lovelace"
        });

        var result = await service.LoginAsync(new LoginRequest { Email = "login@example.com", Password = ValidPassword });

        Assert.False(string.IsNullOrEmpty(result.AccessToken));
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ThrowsInvalidCredentialsException()
    {
        var (service, _) = CreateService(nameof(LoginAsync_WrongPassword_ThrowsInvalidCredentialsException));
        await service.RegisterAsync(new RegisterRequest
        {
            Email = "wrongpass@example.com",
            Password = ValidPassword,
            FirstName = "Ada",
            LastName = "Lovelace"
        });

        await Assert.ThrowsAsync<InvalidCredentialsException>(() =>
            service.LoginAsync(new LoginRequest { Email = "wrongpass@example.com", Password = "WrongPass1!" }));
    }

    [Fact]
    public async Task LoginAsync_UnknownEmail_ThrowsInvalidCredentialsException()
    {
        var (service, _) = CreateService(nameof(LoginAsync_UnknownEmail_ThrowsInvalidCredentialsException));

        await Assert.ThrowsAsync<InvalidCredentialsException>(() =>
            service.LoginAsync(new LoginRequest { Email = "ghost@example.com", Password = ValidPassword }));
    }

    [Fact]
    public async Task RefreshTokenAsync_ValidToken_ReturnsNewTokensAndRevokesOld()
    {
        var (service, context) = CreateService(nameof(RefreshTokenAsync_ValidToken_ReturnsNewTokensAndRevokesOld));
        var registerResult = await service.RegisterAsync(new RegisterRequest
        {
            Email = "refresh@example.com",
            Password = ValidPassword,
            FirstName = "Ada",
            LastName = "Lovelace"
        });

        var refreshed = await service.RefreshTokenAsync(registerResult.RefreshToken);

        Assert.False(string.IsNullOrEmpty(refreshed.AccessToken));
        Assert.NotEqual(registerResult.RefreshToken, refreshed.RefreshToken);
        var tokens = await context.RefreshTokens.ToListAsync();
        Assert.Contains(tokens, t => t.RevokedAt != null);
    }

    [Fact]
    public async Task RefreshTokenAsync_InvalidToken_ThrowsInvalidRefreshTokenException()
    {
        var (service, _) = CreateService(nameof(RefreshTokenAsync_InvalidToken_ThrowsInvalidRefreshTokenException));

        await Assert.ThrowsAsync<InvalidRefreshTokenException>(() =>
            service.RefreshTokenAsync("not-a-real-token"));
    }

    [Fact]
    public async Task LogoutAsync_ValidToken_RevokesToken()
    {
        var (service, context) = CreateService(nameof(LogoutAsync_ValidToken_RevokesToken));
        var registerResult = await service.RegisterAsync(new RegisterRequest
        {
            Email = "logout@example.com",
            Password = ValidPassword,
            FirstName = "Ada",
            LastName = "Lovelace"
        });

        await service.LogoutAsync(registerResult.RefreshToken);

        var token = await context.RefreshTokens.SingleAsync();
        Assert.NotNull(token.RevokedAt);
    }
}
