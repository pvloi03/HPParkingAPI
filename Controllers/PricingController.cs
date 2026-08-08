using Asp.Versioning;
using HPParkingAPI.Models.DTOs.Parking;
using HPParkingAPI.Services.Parking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HPParkingAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/pricing-policies")]
[Produces("application/json")]
[Authorize]
public class PricingController : ControllerBase
{
    private readonly IPricingService _pricingService;

    public PricingController(IPricingService pricingService)
    {
        _pricingService = pricingService;
    }

    /// <summary>Tạo cấu hình bảng giá mới (SuperAdmin, SiteAdmin)</summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,SiteAdmin")]
    [ProducesResponseType(typeof(PricingPolicyResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePolicy([FromBody] CreatePricingPolicyDto dto)
    {
        var result = await _pricingService.CreatePolicyAsync(dto);
        return CreatedAtAction(nameof(GetPolicyById), new { id = result.Id }, result);
    }

    /// <summary>Lấy danh sách bảng giá theo SiteId</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<PricingPolicyResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPoliciesBySite([FromQuery] string siteId)
    {
        var result = await _pricingService.GetPoliciesBySiteAsync(siteId);
        return Ok(result);
    }

    /// <summary>Lấy chi tiết bảng giá theo ID</summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PricingPolicyResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPolicyById(string id)
    {
        var result = await _pricingService.GetPolicyByIdAsync(id);
        if (result is null) return NotFound(new { message = "Không tìm thấy bảng giá." });
        return Ok(result);
    }

    /// <summary>Vô hiệu hóa bảng giá</summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin,SiteAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivatePolicy(string id)
    {
        var success = await _pricingService.DeactivatePolicyAsync(id);
        if (!success) return NotFound(new { message = "Không tìm thấy bảng giá." });
        return NoContent();
    }
}
