using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HPParkingAPI.Models.DTOs.Auth;
using HPParkingAPI.Models.Entities.Auth;
using HPParkingAPI.Repository.Interfaces;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace HPParkingAPI.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IRepository<AppUser> _userRepo;
    private readonly IConfiguration _config;

    public AuthService(IRepository<AppUser> userRepo, IConfiguration config)
    {
        _userRepo = userRepo;
        _config = config;
    }

    // ===================== LOGIN =====================
    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto)
    {
        var user = await _userRepo.FindOneAsync(u => u.Username == dto.Username && u.IsActive);

        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null;

        // Cap nhat thoi gian dang nhap cuoi
        user.LastLoginAt = DateTime.UtcNow;
        await _userRepo.UpdateAsync(user);

        var token = GenerateJwtToken(user);
        var expiry = DateTime.UtcNow.AddMinutes(
            double.Parse(_config["JwtSettings:ExpiryMinutes"] ?? "480"));

        return new LoginResponseDto
        {
            Token = token,
            UserId = user.Id,
            FullName = user.FullName,
            Role = user.Role,
            SiteId = user.SiteId,
            ExpiresAt = expiry
        };
    }

    // ===================== CREATE USER =====================
    public async Task<UserResponseDto> CreateUserAsync(CreateUserDto dto)
    {
        if (await _userRepo.ExistsAsync(u => u.Username == dto.Username))
            throw new InvalidOperationException($"Ten dang nhap '{dto.Username}' da ton tai.");

        var user = new AppUser
        {
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            FullName = dto.FullName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Role = dto.Role,
            SiteId = dto.SiteId,
            IsActive = true
        };

        await _userRepo.InsertAsync(user);
        return MapToResponse(user);
    }

    // ===================== GET ALL =====================
    public async Task<List<UserResponseDto>> GetAllUsersAsync()
    {
        var users = await _userRepo.GetAllAsync();
        return [.. users.Select(MapToResponse)];
    }

    // ===================== GET BY ID =====================
    public async Task<UserResponseDto?> GetUserByIdAsync(string id)
    {
        var user = await _userRepo.GetByIdAsync(id);
        return user is null ? null : MapToResponse(user);
    }

    // ===================== CHANGE PASSWORD =====================
    public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user is null || !BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _userRepo.UpdateAsync(user);
        return true;
    }

    // ===================== DEACTIVATE =====================
    public async Task<bool> DeactivateUserAsync(string userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user is null) return false;

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        await _userRepo.UpdateAsync(user);
        return true;
    }

    // ===================== REACTIVATE =====================
    public async Task<bool> ReactivateUserAsync(string userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user is null) return false;

        user.IsActive = true;
        user.UpdatedAt = DateTime.UtcNow;
        await _userRepo.UpdateAsync(user);
        return true;
    }

    // ===================== SEED ADMIN =====================
    public async Task SeedInitialAdminAsync()
    {
        if (!await _userRepo.ExistsAsync(u => u.Role == UserRole.SuperAdmin))
        {
            var admin = new AppUser
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                FullName = "System Administrator",
                Email = "admin@hpparking.com",
                Role = UserRole.SuperAdmin,
                IsActive = true
            };
            await _userRepo.InsertAsync(admin);
        }
    }

    // ===================== HELPERS =====================
    private string GenerateJwtToken(AppUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = double.Parse(_config["JwtSettings:ExpiryMinutes"] ?? "480");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,  user.Id),
            new Claim(JwtRegisteredClaimNames.Name, user.FullName),
            new Claim(ClaimTypes.Role,              user.Role.ToString()),
            new Claim("siteId",                     user.SiteId ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti,  Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"],
            audience: _config["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiry),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static UserResponseDto MapToResponse(AppUser u) => new()
    {
        Id = u.Id,
        Username = u.Username,
        FullName = u.FullName,
        Email = u.Email,
        PhoneNumber = u.PhoneNumber,
        Role = u.Role,
        SiteId = u.SiteId,
        IsActive = u.IsActive,
        LastLoginAt = u.LastLoginAt,
        CreatedAt = u.CreatedAt
    };
}
