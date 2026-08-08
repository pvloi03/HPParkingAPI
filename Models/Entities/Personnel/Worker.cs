using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HPParkingAPI.Models.Entities.Personnel;

// ⚠️ DEPRECATED: Su dung Person thay the (Person.cs)
// File nay giu lai de tuong thich, se xoa sau.

[Obsolete("Su dung class Person thay the. File nay se duoc xoa.")]
public enum WorkerRole
{
    Worker = 1,
    Engineer = 2,
    Staff = 3,
    Visitor = 4
}

[Obsolete("Su dung class Person thay the. File nay se duoc xoa.")]
public class Worker : BaseEntity
{
    public string CardNumber { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string? IdentityNumber { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Department { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? ContractorId { get; set; }

    [BsonRepresentation(BsonType.String)]
    public WorkerRole Role { get; set; } = WorkerRole.Worker;

    public string? FaceImageUrl { get; set; }
    public bool IsAllowedEntry { get; set; } = true;
    public bool IsBlacklisted { get; set; } = false;
    public string? BlacklistReason { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? CardExpiryDate { get; set; }
}
