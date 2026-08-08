using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HPParkingAPI.Models.Entities.Devices;

public enum DeviceType
{
    AnprCamera = 1,       // Camera chụp & đọc biển số xe
    FaceIdCamera = 2,     // Camera nhận diện khuôn mặt công nhân
    AccessController = 3, // Bộ điều khiển Controller kết nối LAN (Mở Barrier / Turnstile)
    CardReader = 4,       // Đầu đọc thẻ từ RFID (Nối Wiegand vào Controller hoặc USB vào PC)
    CitizenIdReader = 5,  // Đầu đọc thẻ CCCD gắn chip (Dây USB cắm trực tiếp máy tính)
    RadarSensor = 6,      // Cảm biến Rada / Loop detector (Nối I/O vào Controller hoặc Camera)
    LedBoard = 7          // Bảng LED hiển thị số tiền / chào mừng
}

public enum ConnectionMode
{
    NetworkLAN = 1,          // Kết nối dây mạng LAN IP (Camera, Controller)
    DirectUSB = 2,           // Cắm dây USB trực tiếp vào máy tính (Đầu đọc CCCD, Đầu đọc USB)
    WiegandToController = 3  // Đấu dây Wiegand (D0/D1) vào Bộ điều khiển Controller
}

public enum DeviceStatus
{
    Online = 1,      // Đang hoạt động bình thường
    Offline = 2,     // Mất kết nối mạng / Tắt nguồn
    Error = 3,       // Gặp lỗi phần cứng / Lỗi SDK
    Maintenance = 4  // Đang bảo trì
}

public class Device : BaseEntity
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string SiteId { get; set; } = null!;     // Công trường thuộc về

    [BsonRepresentation(BsonType.ObjectId)]
    public string? GateId { get; set; }            // Cổng / Làn xe thuộc về (nếu có)

    public string Code { get; set; } = null!;       // Mã thiết bị: "CAM-ANPR-IN-01"
    public string Name { get; set; } = null!;       // Tên thiết bị: "Camera Biển Số Làn Vào Cổng 1"

    [BsonRepresentation(BsonType.String)]
    public DeviceType DeviceType { get; set; } = DeviceType.AnprCamera;

    [BsonRepresentation(BsonType.String)]
    public ConnectionMode ConnectionMode { get; set; } = ConnectionMode.NetworkLAN; // Phương thức kết nối physical

    // 1. Cấu hình mạng LAN (Cho thiết bị IP như Camera, Access Controller)
    public string? IpAddress { get; set; }          // Ví dụ: "192.168.1.100"
    public int Port { get; set; } = 80;             // Cổng kết nối (80, 554, 8000...)
    public string? RtspUrl { get; set; }            // Luồng RTSP Video (như "rtsp://admin:pass@192.168.1.100:554/h264")
    public string? Username { get; set; }          // Tài khoản đăng nhập thiết bị
    public string? Password { get; set; }          // Mật khẩu đăng nhập thiết bị

    // 2. Cấu hình Wiegand (Cho đầu đọc RFID nối vào Access Controller)
    [BsonRepresentation(BsonType.ObjectId)]
    public string? ParentControllerId { get; set; } // ID của Access Controller tiếp nhận tín hiệu Wiegand
    public string? WiegandFormat { get; set; }      // Định dạng: "Wiegand26", "Wiegand34", "Wiegand66"
    public int ControllerPortIndex { get; set; } = 1;// Cổng đọc số 1 hay số 2 trên Controller (Reader 1 / Reader 2)

    // 3. Cấu hình USB / Serial (Cho đầu đọc CCCD cắm dây USB vào Máy tính)
    public string? UsbDevicePath { get; set; }      // Ví dụ: USB VID/PID hoặc tên thiết bị HID
    public string? ComPort { get; set; }            // Ví dụ: "COM3" (nếu là Virtual COM over USB)
    public int BaudRate { get; set; } = 9600;       // Tốc độ truyền (9600, 115200...)

    [BsonRepresentation(BsonType.String)]
    public DeviceStatus Status { get; set; } = DeviceStatus.Online;

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? LastPingAt { get; set; }      // Thời điểm kiểm tra kết nối gần nhất

    public bool IsActive { get; set; } = true;
    public string? Note { get; set; }               // Ghi chú
}

