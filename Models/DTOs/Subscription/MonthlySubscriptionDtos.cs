using System.ComponentModel.DataAnnotations;
using HPParkingAPI.Models.Entities.Vehicles;

namespace HPParkingAPI.Models.DTOs.Subscription;

public class CreateMonthlySubscriptionDto
{
    [Required(ErrorMessage = "SiteId khong duoc de trong")]
    public string SiteId { get; set; } = null!;

    [Required(ErrorMessage = "Bien so xe khong duoc de trong")]
    public string LicensePlate { get; set; } = null!;

    public VehicleCategory VehicleCategory { get; set; } = VehicleCategory.Motorbike;

    public string? PersonId { get; set; }        // Lien ket voi cu dan / nhan vien (neu co)
    public string? CardNumber { get; set; }      // The RFID neu co
    public string? PricingPolicyId { get; set; } // Bang gia ap dung

    [Required(ErrorMessage = "Ngay bat dau khong duoc de trong")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "Ngay het han khong duoc de trong")]
    public DateTime ExpiryDate { get; set; }

    public decimal AmountPaid { get; set; } = 0; // 0 = mien phi (cong truong noi bo)
    public string? Note { get; set; }
}

public class MonthlySubscriptionResponseDto
{
    public string Id { get; set; } = null!;
    public string SiteId { get; set; } = null!;
    public string? PersonId { get; set; }
    public string? VehicleId { get; set; }
    public string LicensePlate { get; set; } = null!;
    public string? CardNumber { get; set; }
    public VehicleCategory VehicleCategory { get; set; }
    public string? PricingPolicyId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsExpired => DateTime.UtcNow > ExpiryDate;
    public decimal AmountPaid { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
}
