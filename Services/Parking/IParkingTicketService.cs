using HPParkingAPI.Models.DTOs.Parking;

namespace HPParkingAPI.Services.Parking;

public interface IParkingTicketService
{
    Task<ParkingTicketDto> CheckInAsync(CheckInRequestDto dto);
    Task<CheckOutResponseDto> CheckOutAsync(CheckOutRequestDto dto);
    Task<List<ParkingTicketDto>> GetActiveTicketsAsync(string siteId);
    Task<ParkingTicketDto?> GetTicketByIdAsync(string id);
    Task<List<ParkingTicketDto>> SearchTicketsAsync(string siteId, string? licensePlate, string? cardNumber, bool? isActive);
}
