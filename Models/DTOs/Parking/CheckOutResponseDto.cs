using HPParkingAPI.Models.Entities.Parking;
using HPParkingAPI.Models.Entities.Vehicles;

namespace HPParkingAPI.Models.DTOs.Parking;

public class CheckOutResponseDto
{
    public string TicketId { get; set; } = null!;
    public string LicensePlate { get; set; } = null!;
    public string CardNumber { get; set; } = null!;
    public VehicleCategory VehicleCategory { get; set; }
    public TicketType TicketType { get; set; }

    public DateTime CheckInTime { get; set; }
    public DateTime CheckOutTime { get; set; }
    public double TotalMinutes { get; set; }

    public decimal Amount { get; set; }
    public bool IsPaid { get; set; }

    public string? VietQrUrl { get; set; }
    public string? QrPayload { get; set; }

    public string? InImageUrl { get; set; }
    public string? OutImageUrl { get; set; }

    public bool IsAllowedExit { get; set; } = true;
    public string Message { get; set; } = "Khớp vé thành công";
}
