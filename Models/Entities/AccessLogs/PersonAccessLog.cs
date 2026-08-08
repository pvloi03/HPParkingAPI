using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HPParkingAPI.Models.Entities.AccessLogs;

public enum AccessDirection
{
    In = 1,  // Vào công trường
    Out = 2  // Ra khỏi công trường
}

public enum AuthMethod
{
    Card = 1,       // Quẹt thẻ RFID / QR Code
    FaceId = 2,     // Nhận diện khuôn mặt FaceID
    Manual = 3,     // Bảo vệ cho vào thủ công
    CitizenId = 4   // Quẹt / Đọc CCCD gắn chip
}

public class PersonAccessLog : BaseEntity
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string SiteId { get; set; } = null!;       // ID Công trường

    [BsonRepresentation(BsonType.ObjectId)]
    public string GateId { get; set; } = null!;       // ID Cổng ra vào

    [BsonRepresentation(BsonType.ObjectId)]
    public string? WorkerId { get; set; }             // ID Công nhân/Nhân viên

    public string CardNumber { get; set; } = null!;   // Mã thẻ quẹt
    public string WorkerName { get; set; } = null!;   // Họ tên người ra vào
    public string? IdentityNumber { get; set; }       // Số CCCD / CMND

    [BsonRepresentation(BsonType.String)]
    public AccessDirection Direction { get; set; } = AccessDirection.In;

    [BsonRepresentation(BsonType.String)]
    public AuthMethod Method { get; set; } = AuthMethod.Card;

    public string? SnapshotUrl { get; set; }          // Ảnh chụp khuôn mặt thực tế lúc qua cổng
    public float FaceMatchScore { get; set; } = 1.0f;  // Độ tương đồng nhận diện khuôn mặt (nếu qua FaceID)

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime AccessTime { get; set; } = DateTime.UtcNow; // Thời điểm ra/vào

    public bool IsSuccess { get; set; } = true;       // Hợp lệ hay thất bại
    public string? Note { get; set; }                 // Ghi chú (VD: "Thẻ hết hạn", "Bảo vệ duyệt")
}

