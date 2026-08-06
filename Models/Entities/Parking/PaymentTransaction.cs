using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LearnApi.Models.Entities.Parking;

public enum PaymentMethod
{
    Cash = 1,      // Tiền mặt
    QRCode = 2,    // Quét mã QR (VietQR / MoMo / ZaloPay)
    Card = 3       // Thẻ ATM / Thẻ tín dụng
}

public class PaymentTransaction : BaseEntity
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string ParkingTicketId { get; set; } = null!; // Lượt đỗ xe liên kết

    public decimal Amount { get; set; } = 0;             // Số tiền thanh toán

    [BsonRepresentation(BsonType.String)]
    public PaymentMethod Method { get; set; } = PaymentMethod.Cash;

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime PaidAt { get; set; } = DateTime.UtcNow; // Thời điểm thanh toán

    public string? ReceivedBy { get; set; }              // Tên bảo vệ / thu ngân xác nhận
    public string? Note { get; set; }                    // Ghi chú
}
