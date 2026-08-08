using HPParkingAPI.Models.DTOs.Parking;
using HPParkingAPI.Models.Entities.AccessLogs;
using HPParkingAPI.Models.Entities.Parking;
using HPParkingAPI.Repository.Interfaces;

namespace HPParkingAPI.Services.Parking;

public class ParkingTicketService : IParkingTicketService
{
    private readonly IRepository<ParkingTicket> _ticketRepo;
    private readonly IRepository<MonthlySubscription> _subscriptionRepo;
    private readonly IRepository<VehicleAccessLog> _logRepo;
    private readonly IPricingService _pricingService;

    public ParkingTicketService(
        IRepository<ParkingTicket> ticketRepo,
        IRepository<MonthlySubscription> subscriptionRepo,
        IRepository<VehicleAccessLog> logRepo,
        IPricingService pricingService)
    {
        _ticketRepo = ticketRepo;
        _subscriptionRepo = subscriptionRepo;
        _logRepo = logRepo;
        _pricingService = pricingService;
    }

    public async Task<ParkingTicketDto> CheckInAsync(CheckInRequestDto dto)
    {
        // Check for active monthly subscription for this plate or card
        var now = DateTime.UtcNow;
        var sub = await _subscriptionRepo.FindOneAsync(s =>
            s.SiteId == dto.SiteId &&
            s.IsActive &&
            s.StartDate <= now &&
            s.ExpiryDate >= now &&
            (s.LicensePlate == dto.LicensePlate || (s.CardNumber != null && s.CardNumber == dto.CardNumber)));

        var ticketType = sub is not null ? TicketType.Monthly : TicketType.Casual;

        var ticket = new ParkingTicket
        {
            SiteId = dto.SiteId,
            InGateId = dto.InGateId,
            CardNumber = dto.CardNumber,
            LicensePlate = dto.LicensePlate,
            VehicleCategory = dto.VehicleCategory,
            TicketType = ticketType,
            MonthlySubscriptionId = sub?.Id,
            InImageUrl = dto.InImageUrl,
            Status = TicketStatus.Active,
            CheckInTime = DateTime.UtcNow,
            Amount = 0,
            IsPaid = (ticketType == TicketType.Monthly),
            Note = dto.Note
        };

        await _ticketRepo.InsertAsync(ticket);

        // Record Vehicle Access Log (In)
        var accessLog = new VehicleAccessLog
        {
            SiteId = dto.SiteId,
            GateId = dto.InGateId,
            LicensePlate = dto.LicensePlate,
            Category = dto.VehicleCategory,
            Direction = AccessDirection.In,
            AccessTime = DateTime.UtcNow,
            PlateCropImageUrl = dto.InImageUrl,
            ConfidenceScore = dto.AiConfidenceScore,
            IsSuccess = true,
            Note = dto.Note
        };

        await _logRepo.InsertAsync(accessLog);

        return MapToDto(ticket);
    }

    public async Task<CheckOutResponseDto> CheckOutAsync(CheckOutRequestDto dto)
    {
        // Find active ticket by LicensePlate or CardNumber
        var ticket = await _ticketRepo.FindOneAsync(t =>
            t.SiteId == dto.SiteId &&
            t.Status == TicketStatus.Active &&
            (t.LicensePlate == dto.LicensePlate || (dto.CardNumber != null && t.CardNumber == dto.CardNumber)));

        if (ticket is null)
        {
            return new CheckOutResponseDto
            {
                IsAllowedExit = false,
                Message = $"Không tìm thấy lượt xe vào hợp lệ cho biển số {dto.LicensePlate}"
            };
        }

        var checkOutTime = DateTime.UtcNow;
        var totalMinutes = (checkOutTime - ticket.CheckInTime).TotalMinutes;

        // Calculate fee
        var amount = await _pricingService.CalculateParkingFeeAsync(
            dto.SiteId,
            ticket.VehicleCategory,
            ticket.TicketType,
            ticket.CheckInTime,
            checkOutTime);

        // Update Ticket status
        ticket.OutGateId = dto.OutGateId;
        ticket.OutImageUrl = dto.OutImageUrl;
        ticket.CheckOutTime = checkOutTime;
        ticket.Amount = amount;
        ticket.Status = TicketStatus.Completed;
        ticket.IsPaid = (amount == 0);
        ticket.UpdatedAt = DateTime.UtcNow;

        await _ticketRepo.UpdateAsync(ticket);

        // Record Vehicle Access Log (Out)
        var accessLog = new VehicleAccessLog
        {
            SiteId = dto.SiteId,
            GateId = dto.OutGateId,
            LicensePlate = dto.LicensePlate,
            Category = ticket.VehicleCategory,
            Direction = AccessDirection.Out,
            AccessTime = checkOutTime,
            PlateCropImageUrl = dto.OutImageUrl,
            ConfidenceScore = dto.AiConfidenceScore,
            IsSuccess = true,
            Note = dto.Note
        };

        await _logRepo.InsertAsync(accessLog);

        // Generate VietQR payload if amount > 0
        string? vietQrUrl = null;
        string? qrPayload = null;

        if (amount > 0)
        {
            var encodedInfo = Uri.EscapeDataString($"Thanh toan do xe {dto.LicensePlate}");
            vietQrUrl = $"https://img.vietqr.io/image/970422-0123456789-compact2.png?amount={amount:F0}&addInfo={encodedInfo}";
            qrPayload = $"00020101021238570010A00000072701270006970422011301234567895303704540{amount:F0}5802VN53037046219{encodedInfo}6304";
        }

        return new CheckOutResponseDto
        {
            TicketId = ticket.Id,
            LicensePlate = ticket.LicensePlate,
            CardNumber = ticket.CardNumber,
            VehicleCategory = ticket.VehicleCategory,
            TicketType = ticket.TicketType,
            CheckInTime = ticket.CheckInTime,
            CheckOutTime = checkOutTime,
            TotalMinutes = Math.Round(totalMinutes, 1),
            Amount = amount,
            IsPaid = ticket.IsPaid,
            VietQrUrl = vietQrUrl,
            QrPayload = qrPayload,
            InImageUrl = ticket.InImageUrl,
            OutImageUrl = dto.OutImageUrl,
            IsAllowedExit = true,
            Message = amount > 0 ? $"Vui lòng thu {amount:N0} VNĐ" : "Hợp lệ - Mở barie xe ra"
        };
    }

    public async Task<List<ParkingTicketDto>> GetActiveTicketsAsync(string siteId)
    {
        var tickets = await _ticketRepo.FindAsync(t => t.SiteId == siteId && t.Status == TicketStatus.Active);
        return [.. tickets.Select(MapToDto)];
    }

    public async Task<ParkingTicketDto?> GetTicketByIdAsync(string id)
    {
        var ticket = await _ticketRepo.GetByIdAsync(id);
        return ticket is null ? null : MapToDto(ticket);
    }

    public async Task<List<ParkingTicketDto>> SearchTicketsAsync(string siteId, string? licensePlate, string? cardNumber, bool? isActive)
    {
        var tickets = await _ticketRepo.FindAsync(t =>
            t.SiteId == siteId &&
            (string.IsNullOrEmpty(licensePlate) || t.LicensePlate.Contains(licensePlate)) &&
            (string.IsNullOrEmpty(cardNumber) || t.CardNumber.Contains(cardNumber)) &&
            (!isActive.HasValue || (isActive.Value ? t.Status == TicketStatus.Active : t.Status == TicketStatus.Completed)));

        return [.. tickets.Select(MapToDto)];
    }

    private static ParkingTicketDto MapToDto(ParkingTicket t) => new()
    {
        Id = t.Id,
        SiteId = t.SiteId,
        InGateId = t.InGateId,
        OutGateId = t.OutGateId,
        CardNumber = t.CardNumber,
        LicensePlate = t.LicensePlate,
        VehicleCategory = t.VehicleCategory,
        TicketType = t.TicketType,
        InImageUrl = t.InImageUrl,
        OutImageUrl = t.OutImageUrl,
        Status = t.Status,
        CheckInTime = t.CheckInTime,
        CheckOutTime = t.CheckOutTime,
        Amount = t.Amount,
        IsPaid = t.IsPaid,
        Note = t.Note
    };
}
