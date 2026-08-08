using Asp.Versioning;
using HPParkingAPI.Models.DTOs.Subscription;
using HPParkingAPI.Services.Subscription;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HPParkingAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/subscriptions")]
[Produces("application/json")]
[Authorize]
public class SubscriptionController : ControllerBase
{
    private readonly IMonthlySubscriptionService _subscriptionService;

    public SubscriptionController(IMonthlySubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    /// <summary>Đăng ký vé tháng mới</summary>
    [HttpPost]
    [ProducesResponseType(typeof(MonthlySubscriptionResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateSubscription([FromBody] CreateMonthlySubscriptionDto dto)
    {
        try
        {
            var result = await _subscriptionService.CreateSubscriptionAsync(dto);
            return CreatedAtAction(nameof(GetSubscriptionById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>Gia hạn vé tháng</summary>
    [HttpPost("{id}/extend")]
    [ProducesResponseType(typeof(MonthlySubscriptionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExtendSubscription(string id, [FromQuery] int extraDays, [FromQuery] decimal extraAmount)
    {
        var result = await _subscriptionService.ExtendSubscriptionAsync(id, extraDays, extraAmount);
        if (result is null) return NotFound(new { message = "Không tìm thấy vé tháng." });
        return Ok(result);
    }

    /// <summary>Lấy danh sách vé tháng theo SiteId</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<MonthlySubscriptionResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSubscriptionsBySite([FromQuery] string siteId)
    {
        var result = await _subscriptionService.GetSubscriptionsBySiteAsync(siteId);
        return Ok(result);
    }

    /// <summary>Lấy chi tiết vé tháng theo ID</summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MonthlySubscriptionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSubscriptionById(string id)
    {
        var result = await _subscriptionService.GetSubscriptionByIdAsync(id);
        if (result is null) return NotFound(new { message = "Không tìm thấy vé tháng." });
        return Ok(result);
    }

    /// <summary>Vô hiệu hóa vé tháng</summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin,SiteAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateSubscription(string id)
    {
        var success = await _subscriptionService.DeactivateSubscriptionAsync(id);
        if (!success) return NotFound(new { message = "Không tìm thấy vé tháng." });
        return NoContent();
    }
}
