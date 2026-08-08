using System.ComponentModel.DataAnnotations;
using HPParkingAPI.Models.Entities.Devices;

namespace HPParkingAPI.Models.DTOs.Devices;

public class CreateDeviceDto
{
    [Required(ErrorMessage = "SiteId không được để trống")]
    public string SiteId { get; set; } = null!;

    public string? GateId { get; set; }

    [Required(ErrorMessage = "Mã thiết bị không được để trống")]
    public string Code { get; set; } = null!;

    [Required(ErrorMessage = "Tên thiết bị không được để trống")]
    public string Name { get; set; } = null!;

    public DeviceType DeviceType { get; set; } = DeviceType.AnprCamera;
    public ConnectionMode ConnectionMode { get; set; } = ConnectionMode.NetworkLAN;

    // Cấu hình mạng LAN IP
    public string? IpAddress { get; set; }
    public int Port { get; set; } = 80;
    public string? RtspUrl { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }

    // Cấu hình Wiegand
    public string? ParentControllerId { get; set; }
    public string? WiegandFormat { get; set; }
    public int ControllerPortIndex { get; set; } = 1;

    // Cấu hình USB / Serial
    public string? UsbDevicePath { get; set; }
    public string? ComPort { get; set; }
    public int BaudRate { get; set; } = 9600;

    public string? Note { get; set; }
}

public class UpdateDeviceDto
{
    public string? GateId { get; set; }
    public string Name { get; set; } = null!;
    public ConnectionMode ConnectionMode { get; set; }

    public string? IpAddress { get; set; }
    public int Port { get; set; }
    public string? RtspUrl { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }

    public string? ParentControllerId { get; set; }
    public string? WiegandFormat { get; set; }
    public int ControllerPortIndex { get; set; }

    public string? UsbDevicePath { get; set; }
    public string? ComPort { get; set; }
    public int BaudRate { get; set; }
    public bool IsActive { get; set; }
    public string? Note { get; set; }
}

public class DeviceResponseDto
{
    public string Id { get; set; } = null!;
    public string SiteId { get; set; } = null!;
    public string? GateId { get; set; }

    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DeviceType DeviceType { get; set; }
    public ConnectionMode ConnectionMode { get; set; }

    public string? IpAddress { get; set; }
    public int Port { get; set; }
    public string? RtspUrl { get; set; }

    public string? ParentControllerId { get; set; }
    public string? WiegandFormat { get; set; }
    public int ControllerPortIndex { get; set; }

    public string? UsbDevicePath { get; set; }
    public string? ComPort { get; set; }
    public int BaudRate { get; set; }

    public DeviceStatus Status { get; set; }
    public DateTime? LastPingAt { get; set; }
    public bool IsActive { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class DeviceHeartbeatDto
{
    [Required(ErrorMessage = "DeviceId không được để trống")]
    public string DeviceId { get; set; } = null!;

    public DeviceStatus Status { get; set; } = DeviceStatus.Online;
    public string? ErrorMessage { get; set; }
}

