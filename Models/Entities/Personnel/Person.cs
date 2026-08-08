using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HPParkingAPI.Models.Entities.Personnel;

public enum PersonRole
{
    // --- Cong truong xay dung ---
    Worker = 1,         // Cong nhan nha thau
    Engineer = 2,       // Ky su / Giam sat cong trinh

    // --- Chung cu / Van phong ---
    Resident = 10,      // Cu dan chung cu
    Tenant = 11,        // Nguoi thue van phong / mat bang

    // --- Chung cho moi loai bai ---
    Staff = 20,         // Nhan vien van hanh (bao ve, thu phi...)
    Visitor = 21,       // Khach tham quan (duoc cap the tam)
}

public class Person : BaseEntity
{
    public string CardNumber { get; set; } = null!;

    public string FullName { get; set; } = null!;
    public string? IdentityNumber { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string SiteId { get; set; } = null!;

    [BsonRepresentation(BsonType.String)]
    public PersonRole Role { get; set; } = PersonRole.Worker;

    // Cong truong
    public string? Department { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? ContractorId { get; set; }

    // Chung cu
    public string? ApartmentNumber { get; set; }

    // Nhan dien
    public string? FaceImageUrl { get; set; }

    // Kiem soat ra vao
    public bool IsAllowedEntry { get; set; } = true;
    public bool IsBlacklisted { get; set; } = false;
    public string? BlacklistReason { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? CardExpiryDate { get; set; }
}
