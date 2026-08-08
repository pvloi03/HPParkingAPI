using HPParkingAPI.Models.DTOs.Auth;

namespace HPParkingAPI.Services.Auth;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto);
    Task<UserResponseDto> CreateUserAsync(CreateUserDto dto);
    Task<List<UserResponseDto>> GetAllUsersAsync();
    Task<UserResponseDto?> GetUserByIdAsync(string id);
    Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    Task<bool> DeactivateUserAsync(string userId);
    Task SeedInitialAdminAsync();
}
