using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LearnApi.Models.Entities.Personnel;

public enum WorkerRole
{
    Worker = 1,       // Công nhân nhà thầu
    Engineer = 2,     // Kỹ sư / Giám sát
    Staff = 3,        // Nhân viên nội bộ
    Visitor = 4       // Khách tham quan
}

public class Worker : BaseEntity
{
    public string CardNumber { get; set; } = null!;     // Mã thẻ RFID / QR Code
    public string FullName { get; set; } = null!;       // Họ và tên
    public string? IdentityNumber { get; set; }         // Số CCCD / CMND
    public string? PhoneNumber { get; set; }           // Số điện thoại

    [BsonRepresentation(BsonType.ObjectId)]
    public string? ContractorId { get; set; }           // ID Nhà thầu (nếu thuộc nhà thầu phụ)

    [BsonRepresentation(BsonType.String)]
    public WorkerRole Role { get; set; } = WorkerRole.Worker;

    public string? FaceImageUrl { get; set; }           // Ảnh khuôn mặt để điểm danh/FaceID
    public bool IsAllowedEntry { get; set; } = true;    // Có được phép vào công trường không

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? CardExpiryDate { get; set; }        // Hạn sử dụng thẻ
}
