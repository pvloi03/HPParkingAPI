using HPParkingAPI.Models.Entities.Vehicles;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HPParkingAPI.Models.Entities.AccessLogs;

public class VehicleAccessLog : BaseEntity
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string SiteId { get; set; } = null!;           // ID Công trường

    [BsonRepresentation(BsonType.ObjectId)]
    public string GateId { get; set; } = null!;           // ID Cổng ra vào

    [BsonRepresentation(BsonType.ObjectId)]
    public string? VehicleId { get; set; }                // ID Xe (nếu xe đã đăng ký trước)

    public string LicensePlate { get; set; } = null!;     // Biển số xe ghi nhận

    [BsonRepresentation(BsonType.String)]
    public VehicleCategory Category { get; set; } = VehicleCategory.Motorbike; // Phân loại xe

    [BsonRepresentation(BsonType.String)]
    public AccessDirection Direction { get; set; } = AccessDirection.In;

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime AccessTime { get; set; } = DateTime.UtcNow; // Thời điểm xe ra/vào

    public string? FullImageUrl { get; set; }             // Đường dẫn ảnh toàn cảnh xe từ Camera
    public string? PlateCropImageUrl { get; set; }        // Đường dẫn ảnh cắt khung biển số
    public float ConfidenceScore { get; set; } = 1.0f;    // Độ tin cậy nhận diện của AI (0.0 - 1.0)

    public string? DriverName { get; set; }               // Tên tài xế (nếu có)
    public bool IsSuccess { get; set; } = true;           // Cho phép qua barrier hay không
    public string? Note { get; set; }                     // Ghi chú (VD: "Chở cát 10 tấn", "Xe chưa đăng ký")
}

