using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LearnApi.Models.Entities.Sites;

public enum GateType
{
    Person = 1,   // Cổng công nhân / người đi bộ
    Vehicle = 2,  // Cổng xe máy / ô tô / xe tải
    Combined = 3  // Cổng hỗn hợp
}

public enum GateOperatingMode
{
    AccessControlOnly = 1, // Chỉ kiểm soát an ninh nội bộ (Công nhân / Kỹ sư)
    PaidParkingOnly = 2,   // Chỉ thu phí đỗ xe thương mại (Khách vãng lai)
    Hybrid = 3             // Hỗn hợp: Nội bộ miễn phí, Khách ngoài tính tiền
}

public class Gate : BaseEntity
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string SiteId { get; set; } = null!;  // Công trường thuộc về

    public string Code { get; set; } = null!;    // Mã cổng: "GATE-01"
    public string Name { get; set; } = null!;    // Tên cổng: "Cổng chính công nhân"

    [BsonRepresentation(BsonType.String)]
    public GateType GateType { get; set; } = GateType.Combined;

    [BsonRepresentation(BsonType.String)]
    public GateOperatingMode OperatingMode { get; set; } = GateOperatingMode.AccessControlOnly;

    public bool IsActive { get; set; } = true;
}
