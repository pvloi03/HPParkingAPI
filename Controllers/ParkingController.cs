using Asp.Versioning;
using HPParkingAPI.Hubs;
using HPParkingAPI.Models.DTOs.Parking;
using HPParkingAPI.Models.DTOs.Realtime;
using HPParkingAPI.Models.Entities.AccessLogs;
using HPParkingAPI.Services.Parking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace HPParkingAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/parking")]
[Produces("application/json")]
[Authorize]
public class ParkingController : ControllerBase
{
    private readonly IParkingTicketService _ticketService;
    private readonly IHubContext<GateAccessHub> _hubContext;

    public ParkingController(IParkingTicketService ticketService, IHubContext<GateAccessHub> hubContext)
    {
        _ticketService = ticketService;
        _hubContext = hubContext;
    }

    /// <summary>Ghi nhận xe vào bãi (Check-In)</summary>
    [HttpPost("check-in")]
    [ProducesResponseType(typeof(ParkingTicketDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CheckIn([FromBody] CheckInRequestDto dto)
    {
        var result = await _ticketService.CheckInAsync(dto);

        // SignalR Realtime Notification
        await _hubContext.Clients.Group($"site_{dto.SiteId}").SendAsync("ReceiveGateAccessEvent", new GateAccessEventDto
        {
            EventType = "VEHICLE_CHECK_IN",
            SiteId = dto.SiteId,
            GateId = dto.InGateId,
            Direction = AccessDirection.In,
            Identifier = dto.LicensePlate,
            DisplayName = $"Xe {dto.LicensePlate}",
            ImageUrl = dto.InImageUrl,
            IsAllowed = true,
            Reason = "Check-in thành công"
        });

        return CreatedAtAction(nameof(GetTicketById), new { id = result.Id }, result);
    }

    /// <summary>Ghi nhận xe ra & Tính tiền (Check-Out)</summary>
    [HttpPost("check-out")]
    [ProducesResponseType(typeof(CheckOutResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CheckOut([FromBody] CheckOutRequestDto dto)
    {
        var result = await _ticketService.CheckOutAsync(dto);

        // SignalR Realtime Notification
        await _hubContext.Clients.Group($"site_{dto.SiteId}").SendAsync("ReceiveGateAccessEvent", new GateAccessEventDto
        {
            EventType = "VEHICLE_CHECK_OUT",
            SiteId = dto.SiteId,
            GateId = dto.OutGateId,
            Direction = AccessDirection.Out,
            Identifier = dto.LicensePlate,
            DisplayName = $"Xe {dto.LicensePlate}",
            ImageUrl = dto.OutImageUrl,
            IsAllowed = result.IsAllowedExit,
            Reason = result.Message
        });

        return Ok(result);
    }

    /// <summary>Lấy danh sách xe đang gửi trong bãi</summary>
    [HttpGet("tickets/active")]
    [ProducesResponseType(typeof(List<ParkingTicketDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveTickets([FromQuery] string siteId)
    {
        var tickets = await _ticketService.GetActiveTicketsAsync(siteId);
        return Ok(tickets);
    }

    /// <summary>Lấy chi tiết vé đỗ xe theo ID</summary>
    [HttpGet("tickets/{id}")]
    [ProducesResponseType(typeof(ParkingTicketDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTicketById(string id)
    {
        var ticket = await _ticketService.GetTicketByIdAsync(id);
        if (ticket is null) return NotFound(new { message = "Không tìm thấy vé đỗ xe." });
        return Ok(ticket);
    }

    /// <summary>Tra cứu / Tìm kiếm vé đỗ xe</summary>
    [HttpGet("tickets/search")]
    [ProducesResponseType(typeof(List<ParkingTicketDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchTickets(
        [FromQuery] string siteId,
        [FromQuery] string? licensePlate,
        [FromQuery] string? cardNumber,
        [FromQuery] bool? isActive)
    {
        var tickets = await _ticketService.SearchTicketsAsync(siteId, licensePlate, cardNumber, isActive);
        return Ok(tickets);
    }
}
