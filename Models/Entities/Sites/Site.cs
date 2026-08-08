namespace HPParkingAPI.Models.Entities.Sites;

public enum SiteType
{
    ConstructionSite = 1,   // Công trường xây dựng
    Residential = 2,        // Chung cư / Khu dân cư
    Commercial = 3,         // Bãi xe thương mại / Trung tâm thương mại
    Office = 4              // Tòa nhà văn phòng
}

public class Site : BaseEntity
{
    public string Code { get; set; } = null!;    // Ví dụ: "CT-HP-01"
    public string Name { get; set; } = null!;    // Ví dụ: "Công trường Hải Phòng 1"
    public string Address { get; set; } = null!; // Địa chỉ

    public SiteType SiteType { get; set; } = SiteType.ConstructionSite;

    public int? TotalSlots { get; set; }          // Tổng số chỗ đỗ
    public int? MotorbikeSlots { get; set; }      // Số chỗ xe máy
    public int? CarSlots { get; set; }            // Số chỗ ô tô

    public bool IsActive { get; set; } = true;
}
