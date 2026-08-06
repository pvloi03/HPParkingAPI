using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LearnApi.Models.Entities.Parking;

public enum TicketStatus
{
    Active = 1,     // Đang đỗ trong bãi
    Completed = 2,  // Đã ra + Đã thanh toán
    Cancelled = 3   // Huỷ (VD: xe nội bộ không tính tiền)
}

public class ParkingTicket : BaseEntity
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string SiteId { get; set; } = null!;       // Công trường / Bãi xe

    [BsonRepresentation(BsonType.ObjectId)]
    public string InGateId { get; set; } = null!;     // Cổng vào

    [BsonRepresentation(BsonType.ObjectId)]
    public string? OutGateId { get; set; }            // Cổng ra

    [BsonRepresentation(BsonType.ObjectId)]
    public string? PricingPolicyId { get; set; }      // Bảng giá áp dụng

    public string CardNumber { get; set; } = null!;   // Mã thẻ quẹt khi vào
    public string LicensePlate { get; set; } = null!; // Biển số xe

    [BsonRepresentation(BsonType.String)]
    public TicketStatus Status { get; set; } = TicketStatus.Active;

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CheckInTime { get; set; } = DateTime.UtcNow;

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? CheckOutTime { get; set; }

    public decimal Amount { get; set; } = 0;          // Số tiền phải trả
    public bool IsPaid { get; set; } = false;         // Đã thanh toán chưa
    public string? Note { get; set; }                 // Ghi chú
}
