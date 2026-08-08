using System.ComponentModel.DataAnnotations;

namespace HPParkingAPI.Models.DTOs.Parking;

public class CheckOutRequestDto
{
    [Required(ErrorMessage = "SiteId không được để trống")]
    public string SiteId { get; set; } = null!;

    [Required(ErrorMessage = "OutGateId không được để trống")]
    public string OutGateId { get; set; } = null!;

    public string? CardNumber { get; set; }

    [Required(ErrorMessage = "Biển số xe không được để trống")]
    public string LicensePlate { get; set; } = null!;

    public string? OutImageUrl { get; set; }

    public float AiConfidenceScore { get; set; } = 1.0f;

    public string? ReceivedBy { get; set; }

    public string? Note { get; set; }
}
