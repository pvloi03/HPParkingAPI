using System.ComponentModel.DataAnnotations;
using HPParkingAPI.Models.Entities.Vehicles;

namespace HPParkingAPI.Models.DTOs.Parking;

public class CheckInRequestDto
{
    [Required(ErrorMessage = "SiteId không được để trống")]
    public string SiteId { get; set; } = null!;

    [Required(ErrorMessage = "InGateId không được để trống")]
    public string InGateId { get; set; } = null!;

    [Required(ErrorMessage = "Mã thẻ quẹt không được để trống")]
    public string CardNumber { get; set; } = null!;

    [Required(ErrorMessage = "Biển số xe không được để trống")]
    public string LicensePlate { get; set; } = null!;

    public VehicleCategory VehicleCategory { get; set; } = VehicleCategory.Motorbike;

    public string? InImageUrl { get; set; }

    public float AiConfidenceScore { get; set; } = 1.0f;

    public string? Note { get; set; }
}
