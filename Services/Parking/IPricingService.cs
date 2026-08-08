using HPParkingAPI.Models.DTOs.Parking;
using HPParkingAPI.Models.Entities.Parking;
using HPParkingAPI.Models.Entities.Vehicles;

namespace HPParkingAPI.Services.Parking;

public interface IPricingService
{
    Task<decimal> CalculateParkingFeeAsync(
        string siteId,
        VehicleCategory category,
        TicketType ticketType,
        DateTime inTime,
        DateTime outTime);

    Task<PricingPolicyResponseDto> CreatePolicyAsync(CreatePricingPolicyDto dto);
    Task<List<PricingPolicyResponseDto>> GetPoliciesBySiteAsync(string siteId);
    Task<PricingPolicyResponseDto?> GetPolicyByIdAsync(string id);
    Task<bool> DeactivatePolicyAsync(string id);
}
