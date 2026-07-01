using DevFlow.Api.Data;
using DevFlow.Api.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DevFlow.Api.Tests.Data;

public class AppDbContextTests
{
    private static AppDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public void AddUser_ValidUser_PersistsToDatabase()
    {
        using var context = CreateContext(nameof(AddUser_ValidUser_PersistsToDatabase));
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "customer@example.com",
            PasswordHash = "hashed-password",
            FirstName = "Ada",
            LastName = "Lovelace",
            CreatedAt = DateTime.UtcNow
        };

        context.Users.Add(user);
        context.SaveChanges();

        Assert.Single(context.Users);
    }

    [Fact]
    public void RefreshToken_LinkedToUser_CascadeDeletesWithUser()
    {
        using var context = CreateContext(nameof(RefreshToken_LinkedToUser_CascadeDeletesWithUser));
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "customer2@example.com",
            PasswordHash = "hashed-password",
            FirstName = "Grace",
            LastName = "Hopper",
            CreatedAt = DateTime.UtcNow
        };
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            User = user,
            TokenHash = "token-hash",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        context.Users.Add(user);
        context.RefreshTokens.Add(refreshToken);
        context.SaveChanges();

        Assert.Single(context.RefreshTokens);
        Assert.Equal(user.Id, context.RefreshTokens.First().UserId);
    }

    [Fact]
    public void RefreshToken_IsActive_FalseWhenExpired()
    {
        var expiredToken = new RefreshToken
        {
            ExpiresAt = DateTime.UtcNow.AddDays(-1),
            RevokedAt = null
        };

        Assert.False(expiredToken.IsActive);
    }

    [Fact]
    public void RefreshToken_IsActive_FalseWhenRevoked()
    {
        var revokedToken = new RefreshToken
        {
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            RevokedAt = DateTime.UtcNow
        };

        Assert.False(revokedToken.IsActive);
    }

    [Fact]
    public void RefreshToken_IsActive_TrueWhenNotExpiredAndNotRevoked()
    {
        var activeToken = new RefreshToken
        {
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            RevokedAt = null
        };

        Assert.True(activeToken.IsActive);
    }
}
