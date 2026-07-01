using DevFlow.Api.Data;
using DevFlow.Api.DTOs;
using DevFlow.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DevFlow.Api.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IJwtTokenService _tokenService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(AppDbContext context, IJwtTokenService tokenService, IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _tokenService = tokenService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var emailExists = await _context.Users.AnyAsync(u => u.Email == normalizedEmail);
        if (emailExists)
        {
            throw new DuplicateEmailException();
        }

        if (!PasswordPolicy.IsValid(request.Password))
        {
            throw new WeakPasswordException(PasswordPolicy.PolicyDescription);
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = normalizedEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            IsEmailVerified = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return await IssueTokensAsync(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == normalizedEmail);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new InvalidCredentialsException();
        }

        return await IssueTokensAsync(user);
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        var tokenHash = _tokenService.HashRefreshToken(refreshToken);
        var existingToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .SingleOrDefaultAsync(rt => rt.TokenHash == tokenHash);

        if (existingToken is null || !existingToken.IsActive)
        {
            throw new InvalidRefreshTokenException();
        }

        existingToken.RevokedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return await IssueTokensAsync(existingToken.User);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var tokenHash = _tokenService.HashRefreshToken(refreshToken);
        var existingToken = await _context.RefreshTokens
            .SingleOrDefaultAsync(rt => rt.TokenHash == tokenHash);

        if (existingToken is not null && existingToken.RevokedAt is null)
        {
            existingToken.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    private async Task<AuthResponse> IssueTokensAsync(User user)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        _context.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = _tokenService.HashRefreshToken(refreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
}
