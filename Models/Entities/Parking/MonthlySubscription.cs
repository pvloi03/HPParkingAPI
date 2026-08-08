using HPParkingAPI.Models.Entities.Vehicles;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HPParkingAPI.Models.Entities.Parking;

public class MonthlySubscription : BaseEntity
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string SiteId { get; set; } = null!;           // Bai xe ap dung

    [BsonRepresentation(BsonType.ObjectId)]
    public string? PersonId { get; set; }                  // Cu dan / Nhan vien dang ky (neu co)

    [BsonRepresentation(BsonType.ObjectId)]
    public string? VehicleId { get; set; }                 // Xe dang ky (neu biet truoc bien so)

    public string LicensePlate { get; set; } = null!;      // Bien so xe (bat buoc)
    public string? CardNumber { get; set; }                // The RFID / QR (neu dung the)

    [BsonRepresentation(BsonType.String)]
    public VehicleCategory VehicleCategory { get; set; } = VehicleCategory.Motorbike;

    [BsonRepresentation(BsonType.ObjectId)]
    public string? PricingPolicyId { get; set; }           // Bang gia ve thang ap dung

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime StartDate { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime ExpiryDate { get; set; }

    public bool IsActive { get; set; } = true;

    public decimal AmountPaid { get; set; } = 0;           // So tien da tra (0 = mien phi vd cong truong)
    public string? Note { get; set; }
}
