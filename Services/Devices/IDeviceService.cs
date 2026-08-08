using HPParkingAPI.Models.DTOs.Devices;

namespace HPParkingAPI.Services.Devices;

public interface IDeviceService
{
    Task<DeviceResponseDto> CreateDeviceAsync(CreateDeviceDto dto);
    Task<DeviceResponseDto?> UpdateDeviceAsync(string id, UpdateDeviceDto dto);
    Task<DeviceResponseDto?> GetDeviceByIdAsync(string id);
    Task<List<DeviceResponseDto>> GetDevicesBySiteAsync(string siteId);
    Task<bool> ProcessHeartbeatAsync(DeviceHeartbeatDto dto);
    Task<bool> DeleteDeviceAsync(string id);
}
