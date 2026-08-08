using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HPParkingAPI.Models.Entities.Auth;

public enum UserRole
{
    SuperAdmin = 1,     // Quan tri toan he thong (tat ca cac site)
    SiteAdmin = 2,      // Quan ly mot bai xe cu the
    Operator = 3,       // Nhan vien van hanh (thu phi, mo cong)
    Viewer = 4          // Chi xem bao cao / dashboard
}

public class AppUser : BaseEntity
{
    public string Username { get; set; } = null!;        // Ten dang nhap
    public string PasswordHash { get; set; } = null!;    // Mat khau da ma hoa (BCrypt)

    public string FullName { get; set; } = null!;        // Ho va ten
    public string? Email { get; set; }                   // Email
    public string? PhoneNumber { get; set; }             // So dien thoai

    [BsonRepresentation(BsonType.String)]
    public UserRole Role { get; set; } = UserRole.Operator;

    [BsonRepresentation(BsonType.ObjectId)]
    public string? SiteId { get; set; }                  // null = co quyen moi site (SuperAdmin)

    public bool IsActive { get; set; } = true;

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? LastLoginAt { get; set; }           // Lan dang nhap cuoi
}
