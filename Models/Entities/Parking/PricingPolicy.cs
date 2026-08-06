using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LearnApi.Models.Entities.Parking;

public enum PricingType
{
    Hourly = 1,   // Tính theo giờ (xe vãng lai)
    Daily = 2,    // Vé ngày (VD: gói cả ngày 50k)
    Monthly = 3   // Vé tháng (Nhân viên thuê chỗ cố định)
}

public class PricingPolicy : BaseEntity
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string SiteId { get; set; } = null!;     // Áp dụng cho công trường / bãi xe nào

    public string Name { get; set; } = null!;       // Ví dụ: "Bảng giá xe máy 2025"

    [BsonRepresentation(BsonType.String)]
    public PricingType PricingType { get; set; } = PricingType.Hourly;

    // Tính theo giờ (Hourly)
    public decimal FirstBlockPrice { get; set; } = 0;      // Giá N giờ đầu (VD: 2 giờ đầu 10.000đ)
    public int FirstBlockHours { get; set; } = 1;          // Số giờ tính giá đầu
    public decimal PricePerHourAfter { get; set; } = 0;    // Giá mỗi giờ tiếp theo

    // Vé ngày / vé tháng (Daily / Monthly)
    public decimal FlatPrice { get; set; } = 0;            // Giá cố định (vé ngày / vé tháng)

    public decimal MaxDailyPrice { get; set; } = 0;        // Mức trần tối đa trong ngày (nếu có)

    public bool IsActive { get; set; } = true;
}
