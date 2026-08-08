using HPParkingAPI.Models.DTOs.Subscription;

namespace HPParkingAPI.Services.Subscription;

public interface IMonthlySubscriptionService
{
    Task<MonthlySubscriptionResponseDto> CreateSubscriptionAsync(CreateMonthlySubscriptionDto dto);
    Task<MonthlySubscriptionResponseDto?> ExtendSubscriptionAsync(string id, int extraDays, decimal extraAmount);
    Task<List<MonthlySubscriptionResponseDto>> GetSubscriptionsBySiteAsync(string siteId);
    Task<MonthlySubscriptionResponseDto?> GetSubscriptionByIdAsync(string id);
    Task<bool> DeactivateSubscriptionAsync(string id);
}
