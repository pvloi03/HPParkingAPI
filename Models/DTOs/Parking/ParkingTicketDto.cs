using HPParkingAPI.Models.Entities.Parking;
using HPParkingAPI.Models.Entities.Vehicles;

namespace HPParkingAPI.Models.DTOs.Parking;

public class ParkingTicketDto
{
    public string Id { get; set; } = null!;
    public string SiteId { get; set; } = null!;
    public string InGateId { get; set; } = null!;
    public string? OutGateId { get; set; }

    public string CardNumber { get; set; } = null!;
    public string LicensePlate { get; set; } = null!;
    public VehicleCategory VehicleCategory { get; set; }
    public TicketType TicketType { get; set; }

    public string? InImageUrl { get; set; }
    public string? OutImageUrl { get; set; }

    public TicketStatus Status { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }

    public decimal Amount { get; set; }
    public bool IsPaid { get; set; }
    public string? Note { get; set; }
}
