using System.ComponentModel.DataAnnotations;
using HPParkingAPI.Models.Entities.Parking;

namespace HPParkingAPI.Models.DTOs.Payment;

public class CreatePaymentRequestDto
{
    [Required(ErrorMessage = "ParkingTicketId không được để trống")]
    public string ParkingTicketId { get; set; } = null!;

    public decimal Amount { get; set; }

    public PaymentMethod Method { get; set; } = PaymentMethod.QRCode;

    public string? ReceivedBy { get; set; }

    public string? Note { get; set; }
}

public class PaymentResponseDto
{
    public string Id { get; set; } = null!;
    public string ParkingTicketId { get; set; } = null!;
    public string? TransactionCode { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; }
    public string? QrPayload { get; set; }
    public DateTime PaidAt { get; set; }
    public string? ReceivedBy { get; set; }
    public string? Note { get; set; }
}

public class VietQrGenerateDto
{
    public string AccountNo { get; set; } = null!;
    public string AccountName { get; set; } = null!;
    public string BankBin { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Description { get; set; } = null!;
    public string QrImageUrl { get; set; } = null!;
}
