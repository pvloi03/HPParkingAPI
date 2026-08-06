namespace LearnApi.Models.Entities.Sites;

public class Site : BaseEntity
{
    public string Code { get; set; } = null!; // Ví dụ: "CT-HP-01"
    public string Name { get; set; } = null!; // Ví dụ: "Công trường Hải Phòng 1"
    public string Address { get; set; } = null!; // Địa chỉ công trường
    public bool IsActive { get; set; } = true;
}
