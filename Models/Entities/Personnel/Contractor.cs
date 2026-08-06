namespace LearnApi.Models.Entities.Personnel;

public class Contractor : BaseEntity
{
    public string Code { get; set; } = null!;        // Mã nhà thầu: "NT-HOABINH"
    public string Name { get; set; } = null!;        // Tên nhà thầu: "Công ty Cổ phần Tập đoàn Xây dựng Hòa Bình"
    public string? ContactPerson { get; set; }      // Người đại diện liên hệ
    public string? PhoneNumber { get; set; }        // Số điện thoại
    public bool IsActive { get; set; } = true;
}
