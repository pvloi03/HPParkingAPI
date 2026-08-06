using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LearnApi.Models.Entities.Vehicles;

public enum VehicleCategory
{
    Motorbike = 1,  // Xe máy công nhân / nhân viên
    Car = 2,        // Ô tô con / Xe sếp / Khách
    Truck = 3,      // Xe tải chở vật tư / nguyên vật liệu
    Container = 4,  // Xe container / Xe công trình nặng
    Special = 5     // Xe cẩu, xe máy xúc, xe chuyên dụng
}

public class Vehicle : BaseEntity
{
    public string LicensePlate { get; set; } = null!;   // Biển số xe: "15A-12345"

    [BsonRepresentation(BsonType.String)]
    public VehicleCategory Category { get; set; } = VehicleCategory.Motorbike;

    public string? CardNumber { get; set; }             // Mã thẻ RFID quẹt xe / dán kính

    [BsonRepresentation(BsonType.ObjectId)]
    public string? ContractorId { get; set; }           // Nhà thầu sở hữu / phụ trách xe

    public string? OwnerName { get; set; }              // Tên tài xế / chủ xe
    public string? PhoneNumber { get; set; }            // SĐT tài xế
    public bool IsAllowedEntry { get; set; } = true;    // Trạng thái được phép vào
}
