using HPParkingAPI.Models.Entities.AccessLogs;

namespace HPParkingAPI.Models.DTOs.Realtime;

public class GateAccessEventDto
{
    public string EventId { get; set; } = Guid.NewGuid().ToString();
    public string EventType { get; set; } = null!; // "VEHICLE_ACCESS" | "PERSON_ACCESS"
    public string SiteId { get; set; } = null!;
    public string GateId { get; set; } = null!;
    public AccessDirection Direction { get; set; }

    public string Identifier { get; set; } = null!; // Biển số xe hoặc Mã thẻ công nhân
    public string DisplayName { get; set; } = null!; // Tên chủ xe hoặc tên công nhân
    public string? ImageUrl { get; set; }            // Ảnh ANPR hoặc ảnh FaceID

    public bool IsAllowed { get; set; }
    public string? Reason { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
