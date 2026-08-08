using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HPParkingAPI.Models.Entities.Parking;

public enum PaymentMethod
{
    Cash = 1,      // Tiền mặt
    QRCode = 2,    // Quét mã QR (VietQR / MoMo / ZaloPay)
    Card = 3       // Thẻ ATM / Thẻ tín dụng
}

public enum PaymentStatus
{
    Pending = 1,   // Đang chờ thanh toán (Đang hiển thị VietQR)
    Success = 2,   // Thanh toán thành công
    Failed = 3,    // Thất bại / Hết hạn
    Refunded = 4   // Đã hoàn tiền
}

public class PaymentTransaction : BaseEntity
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string ParkingTicketId { get; set; } = null!; // Lượt đỗ xe liên kết

    public string? TransactionCode { get; set; }         // Mã giao dịch ngân hàng / VietQR

    public decimal Amount { get; set; } = 0;             // Số tiền thanh toán

    [BsonRepresentation(BsonType.String)]
    public PaymentMethod Method { get; set; } = PaymentMethod.Cash;

    [BsonRepresentation(BsonType.String)]
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending; // Trạng thái giao dịch

    public string? QrPayload { get; set; }               // Chuỗi mã QR động tạo ra từ VietQR API

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime PaidAt { get; set; } = DateTime.UtcNow; // Thời điểm thanh toán

    public string? ReceivedBy { get; set; }              // Tên bảo vệ / thu ngân xác nhận
    public string? Note { get; set; }                    // Ghi chú
}

