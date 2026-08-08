using System.ComponentModel.DataAnnotations;
using HPParkingAPI.Models.Entities.Parking;
using HPParkingAPI.Models.Entities.Vehicles;

namespace HPParkingAPI.Models.DTOs.Parking;

public class CreatePricingPolicyDto
{
    [Required(ErrorMessage = "SiteId không được để trống")]
    public string SiteId { get; set; } = null!;

    [Required(ErrorMessage = "Tên bảng giá không được để trống")]
    public string Name { get; set; } = null!;

    public VehicleCategory ApplicableCategory { get; set; } = VehicleCategory.Motorbike;
    public PricingType PricingType { get; set; } = PricingType.Hourly;

    public int FreeGraceMinutes { get; set; } = 0;
    public decimal FirstBlockPrice { get; set; } = 0;
    public int FirstBlockHours { get; set; } = 1;
    public decimal PricePerHourAfter { get; set; } = 0;

    public decimal FlatPrice { get; set; } = 0;
    public decimal MaxDailyPrice { get; set; } = 0;
    public decimal OvernightPrice { get; set; } = 0;
}

public class PricingPolicyResponseDto
{
    public string Id { get; set; } = null!;
    public string SiteId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public VehicleCategory ApplicableCategory { get; set; }
    public PricingType PricingType { get; set; }

    public int FreeGraceMinutes { get; set; }
    public decimal FirstBlockPrice { get; set; }
    public int FirstBlockHours { get; set; }
    public decimal PricePerHourAfter { get; set; }

    public decimal FlatPrice { get; set; }
    public decimal MaxDailyPrice { get; set; }
    public decimal OvernightPrice { get; set; }

    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
