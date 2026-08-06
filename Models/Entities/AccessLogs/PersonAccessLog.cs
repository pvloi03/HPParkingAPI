using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LearnApi.Models.Entities.AccessLogs;

public enum AccessDirection
{
    In = 1,  // Vào công trường
    Out = 2  // Ra khỏi công trường
}

public enum AuthMethod
{
    Card = 1,    // Quẹt thẻ RFID / QR Code
    FaceId = 2,  // Nhận diện khuôn mặt FaceID
    Manual = 3   // Bảo vệ cho vào thủ công
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

    [BsonRepresentation(BsonType.String)]
    public AccessDirection Direction { get; set; } = AccessDirection.In;

    [BsonRepresentation(BsonType.String)]
    public AuthMethod Method { get; set; } = AuthMethod.Card;

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime AccessTime { get; set; } = DateTime.UtcNow; // Thời điểm ra/vào

    public bool IsSuccess { get; set; } = true;       // Hợp lệ hay thất bại
    public string? Note { get; set; }                 // Ghi chú (VD: "Thẻ hết hạn", "Bảo vệ duyệt")
}
