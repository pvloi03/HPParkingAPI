using HPParkingAPI.Models.DTOs.Devices;
using HPParkingAPI.Models.Entities.Devices;
using HPParkingAPI.Repository.Interfaces;

namespace HPParkingAPI.Services.Devices;

public class DeviceService : IDeviceService
{
    private readonly IRepository<Device> _deviceRepo;

    public DeviceService(IRepository<Device> deviceRepo)
    {
        _deviceRepo = deviceRepo;
    }

    public async Task<DeviceResponseDto> CreateDeviceAsync(CreateDeviceDto dto)
    {
        if (await _deviceRepo.ExistsAsync(d => d.SiteId == dto.SiteId && d.Code == dto.Code))
        {
            throw new InvalidOperationException($"Mã thiết bị '{dto.Code}' đã tồn tại tại công trường này.");
        }

        var device = new Device
        {
            SiteId = dto.SiteId,
            GateId = dto.GateId,
            Code = dto.Code,
            Name = dto.Name,
            DeviceType = dto.DeviceType,
            ConnectionMode = dto.ConnectionMode,
            IpAddress = dto.IpAddress,
            Port = dto.Port,
            RtspUrl = dto.RtspUrl,
            Username = dto.Username,
            Password = dto.Password,
            ParentControllerId = dto.ParentControllerId,
            WiegandFormat = dto.WiegandFormat,
            ControllerPortIndex = dto.ControllerPortIndex,
            UsbDevicePath = dto.UsbDevicePath,
            ComPort = dto.ComPort,
            BaudRate = dto.BaudRate,
            Status = DeviceStatus.Online,
            LastPingAt = DateTime.UtcNow,
            IsActive = true,
            Note = dto.Note
        };

        await _deviceRepo.InsertAsync(device);
        return MapToDto(device);
    }

    public async Task<DeviceResponseDto?> UpdateDeviceAsync(string id, UpdateDeviceDto dto)
    {
        var device = await _deviceRepo.GetByIdAsync(id);
        if (device is null) return null;

        device.GateId = dto.GateId;
        device.Name = dto.Name;
        device.ConnectionMode = dto.ConnectionMode;
        device.IpAddress = dto.IpAddress;
        device.Port = dto.Port;
        device.RtspUrl = dto.RtspUrl;
        device.Username = dto.Username;
        device.Password = dto.Password;
        device.ParentControllerId = dto.ParentControllerId;
        device.WiegandFormat = dto.WiegandFormat;
        device.ControllerPortIndex = dto.ControllerPortIndex;
        device.UsbDevicePath = dto.UsbDevicePath;
        device.ComPort = dto.ComPort;
        device.BaudRate = dto.BaudRate;
        device.IsActive = dto.IsActive;
        device.Note = dto.Note;
        device.UpdatedAt = DateTime.UtcNow;

        await _deviceRepo.UpdateAsync(device);
        return MapToDto(device);
    }

    public async Task<DeviceResponseDto?> GetDeviceByIdAsync(string id)
    {
        var device = await _deviceRepo.GetByIdAsync(id);
        return device is null ? null : MapToDto(device);
    }

    public async Task<List<DeviceResponseDto>> GetDevicesBySiteAsync(string siteId)
    {
        var devices = await _deviceRepo.FindAsync(d => d.SiteId == siteId);
        return [.. devices.Select(MapToDto)];
    }

    public async Task<bool> ProcessHeartbeatAsync(DeviceHeartbeatDto dto)
    {
        var device = await _deviceRepo.GetByIdAsync(dto.DeviceId);
        if (device is null) return false;

        device.Status = dto.Status;
        device.LastPingAt = DateTime.UtcNow;
        if (!string.IsNullOrEmpty(dto.ErrorMessage))
        {
            device.Note = $"[Lỗi {DateTime.UtcNow:HH:mm:ss}] {dto.ErrorMessage}";
        }
        device.UpdatedAt = DateTime.UtcNow;

        await _deviceRepo.UpdateAsync(device);
        return true;
    }

    public async Task<bool> DeleteDeviceAsync(string id)
    {
        return await _deviceRepo.DeleteAsync(id);
    }

    private static DeviceResponseDto MapToDto(Device d) => new()
    {
        Id = d.Id,
        SiteId = d.SiteId,
        GateId = d.GateId,
        Code = d.Code,
        Name = d.Name,
        DeviceType = d.DeviceType,
        ConnectionMode = d.ConnectionMode,
        IpAddress = d.IpAddress,
        Port = d.Port,
        RtspUrl = d.RtspUrl,
        ParentControllerId = d.ParentControllerId,
        WiegandFormat = d.WiegandFormat,
        ControllerPortIndex = d.ControllerPortIndex,
        UsbDevicePath = d.UsbDevicePath,
        ComPort = d.ComPort,
        BaudRate = d.BaudRate,
        Status = d.Status,
        LastPingAt = d.LastPingAt,
        IsActive = d.IsActive,
        Note = d.Note,
        CreatedAt = d.CreatedAt
    };
}
