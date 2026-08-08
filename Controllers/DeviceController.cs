using Asp.Versioning;
using HPParkingAPI.Models.DTOs.Devices;
using HPParkingAPI.Services.Devices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HPParkingAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/devices")]
[Produces("application/json")]
[Authorize]
public class DeviceController : ControllerBase
{
    private readonly IDeviceService _deviceService;

    public DeviceController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    /// <summary>Thêm thiết bị ngoại vi mới</summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,SiteAdmin")]
    [ProducesResponseType(typeof(DeviceResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateDevice([FromBody] CreateDeviceDto dto)
    {
        try
        {
            var result = await _deviceService.CreateDeviceAsync(dto);
            return CreatedAtAction(nameof(GetDeviceById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>Cập nhật thông tin thiết bị</summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,SiteAdmin")]
    [ProducesResponseType(typeof(DeviceResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDevice(string id, [FromBody] UpdateDeviceDto dto)
    {
        var result = await _deviceService.UpdateDeviceAsync(id, dto);
        if (result is null) return NotFound(new { message = "Không tìm thấy thiết bị." });
        return Ok(result);
    }

    /// <summary>Lấy danh sách thiết bị theo SiteId</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<DeviceResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDevicesBySite([FromQuery] string siteId)
    {
        var result = await _deviceService.GetDevicesBySiteAsync(siteId);
        return Ok(result);
    }

    /// <summary>Lấy chi tiết thiết bị theo ID</summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DeviceResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDeviceById(string id)
    {
        var result = await _deviceService.GetDeviceByIdAsync(id);
        if (result is null) return NotFound(new { message = "Không tìm thấy thiết bị." });
        return Ok(result);
    }

    /// <summary>Gửi tín hiệu Heartbeat ping từ thiết bị</summary>
    [HttpPost("heartbeat")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ProcessHeartbeat([FromBody] DeviceHeartbeatDto dto)
    {
        var success = await _deviceService.ProcessHeartbeatAsync(dto);
        if (!success) return NotFound(new { message = "Không tìm thấy thiết bị." });
        return Ok(new { message = "Heartbeat received" });
    }

    /// <summary>Xóa thiết bị</summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin,SiteAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDevice(string id)
    {
        var success = await _deviceService.DeleteDeviceAsync(id);
        if (!success) return NotFound(new { message = "Không tìm thấy thiết bị." });
        return NoContent();
    }
}
