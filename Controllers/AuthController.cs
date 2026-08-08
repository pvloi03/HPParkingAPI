using Asp.Versioning;
using HPParkingAPI.Models.DTOs.Auth;
using HPParkingAPI.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HPParkingAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Dang nhap he thong</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        if (result is null)
            return Unauthorized(new { message = "Sai ten dang nhap hoac mat khau." });

        return Ok(result);
    }

    /// <summary>Tao tai khoan nguoi dung moi (SuperAdmin only)</summary>
    [HttpPost("users")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
        try
        {
            var result = await _authService.CreateUserAsync(dto);
            return CreatedAtAction(nameof(GetUserById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>Lay danh sach tat ca nguoi dung (SuperAdmin, SiteAdmin)</summary>
    [HttpGet("users")]
    [Authorize(Roles = "SuperAdmin,SiteAdmin")]
    [ProducesResponseType(typeof(List<UserResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _authService.GetAllUsersAsync();
        return Ok(result);
    }

    /// <summary>Lay thong tin nguoi dung theo ID</summary>
    [HttpGet("users/{id}")]
    [Authorize]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserById(string id)
    {
        var result = await _authService.GetUserByIdAsync(id);
        if (result is null) return NotFound(new { message = "Khong tim thay nguoi dung." });
        return Ok(result);
    }

    /// <summary>Doi mat khau</summary>
    [HttpPut("users/{id}/password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword(
        string id, [FromBody] ChangePasswordDto dto)
    {
        var success = await _authService.ChangePasswordAsync(id, dto.CurrentPassword, dto.NewPassword);
        if (!success)
            return BadRequest(new { message = "Mat khau hien tai khong dung." });

        return NoContent();
    }

    /// <summary>Vo hieu hoa tai khoan (SuperAdmin only)</summary>
    [HttpDelete("users/{id}")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeactivateUser(string id)
    {
        var success = await _authService.DeactivateUserAsync(id);
        if (!success) return NotFound(new { message = "Khong tim thay nguoi dung." });
        return NoContent();
    }
}

