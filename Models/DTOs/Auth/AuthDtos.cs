using System.ComponentModel.DataAnnotations;
using HPParkingAPI.Models.Entities.Auth;

namespace HPParkingAPI.Models.DTOs.Auth;

public class LoginRequestDto
{
    [Required(ErrorMessage = "Ten dang nhap khong duoc de trong")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Mat khau khong duoc de trong")]
    public string Password { get; set; } = null!;
}

public class LoginResponseDto
{
    public string Token { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public UserRole Role { get; set; }
    public string? SiteId { get; set; }
    public DateTime ExpiresAt { get; set; }
}

public class CreateUserDto
{
    [Required(ErrorMessage = "Ten dang nhap khong duoc de trong")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Mat khau khong duoc de trong")]
    [MinLength(8, ErrorMessage = "Mat khau toi thieu 8 ky tu")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Ho ten khong duoc de trong")]
    public string FullName { get; set; } = null!;

    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public UserRole Role { get; set; } = UserRole.Operator;
    public string? SiteId { get; set; }
}

public class UserResponseDto
{
    public string Id { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public UserRole Role { get; set; }
    public string? SiteId { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ChangePasswordDto
{
    [Required(ErrorMessage = "Mat khau hien tai khong duoc de trong")]
    public string CurrentPassword { get; set; } = null!;

    [Required(ErrorMessage = "Mat khau moi khong duoc de trong")]
    [MinLength(8, ErrorMessage = "Mat khau moi toi thieu 8 ky tu")]
    public string NewPassword { get; set; } = null!;
}
