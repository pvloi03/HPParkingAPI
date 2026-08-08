using HPParkingAPI.Models.DTOs.Subscription;
using HPParkingAPI.Models.Entities.Parking;
using HPParkingAPI.Repository.Interfaces;

namespace HPParkingAPI.Services.Subscription;

public class MonthlySubscriptionService : IMonthlySubscriptionService
{
    private readonly IRepository<MonthlySubscription> _subscriptionRepo;

    public MonthlySubscriptionService(IRepository<MonthlySubscription> subscriptionRepo)
    {
        _subscriptionRepo = subscriptionRepo;
    }

    public async Task<MonthlySubscriptionResponseDto> CreateSubscriptionAsync(CreateMonthlySubscriptionDto dto)
    {
        var activeExisting = await _subscriptionRepo.FindOneAsync(s =>
            s.SiteId == dto.SiteId &&
            s.LicensePlate == dto.LicensePlate &&
            s.IsActive &&
            s.ExpiryDate >= DateTime.UtcNow);

        if (activeExisting is not null)
        {
            throw new InvalidOperationException($"Biển số '{dto.LicensePlate}' đã có vé tháng đang còn hiệu lực.");
        }

        var subscription = new MonthlySubscription
        {
            SiteId = dto.SiteId,
            LicensePlate = dto.LicensePlate,
            VehicleCategory = dto.VehicleCategory,
            PersonId = dto.PersonId,
            CardNumber = dto.CardNumber,
            PricingPolicyId = dto.PricingPolicyId,
            StartDate = dto.StartDate,
            ExpiryDate = dto.ExpiryDate,
            AmountPaid = dto.AmountPaid,
            IsActive = true,
            Note = dto.Note
        };

        await _subscriptionRepo.InsertAsync(subscription);
        return MapToDto(subscription);
    }

    public async Task<MonthlySubscriptionResponseDto?> ExtendSubscriptionAsync(string id, int extraDays, decimal extraAmount)
    {
        var sub = await _subscriptionRepo.GetByIdAsync(id);
        if (sub is null) return null;

        var baseDate = sub.ExpiryDate > DateTime.UtcNow ? sub.ExpiryDate : DateTime.UtcNow;
        sub.ExpiryDate = baseDate.AddDays(extraDays);
        sub.AmountPaid += extraAmount;
        sub.IsActive = true;
        sub.UpdatedAt = DateTime.UtcNow;

        await _subscriptionRepo.UpdateAsync(sub);
        return MapToDto(sub);
    }

    public async Task<List<MonthlySubscriptionResponseDto>> GetSubscriptionsBySiteAsync(string siteId)
    {
        var subs = await _subscriptionRepo.FindAsync(s => s.SiteId == siteId);
        return [.. subs.Select(MapToDto)];
    }

    public async Task<MonthlySubscriptionResponseDto?> GetSubscriptionByIdAsync(string id)
    {
        var sub = await _subscriptionRepo.GetByIdAsync(id);
        return sub is null ? null : MapToDto(sub);
    }

    public async Task<bool> DeactivateSubscriptionAsync(string id)
    {
        var sub = await _subscriptionRepo.GetByIdAsync(id);
        if (sub is null) return false;

        sub.IsActive = false;
        sub.UpdatedAt = DateTime.UtcNow;
        await _subscriptionRepo.UpdateAsync(sub);
        return true;
    }

    private static MonthlySubscriptionResponseDto MapToDto(MonthlySubscription s) => new()
    {
        Id = s.Id,
        SiteId = s.SiteId,
        PersonId = s.PersonId,
        VehicleId = s.VehicleId,
        LicensePlate = s.LicensePlate,
        CardNumber = s.CardNumber,
        VehicleCategory = s.VehicleCategory,
        PricingPolicyId = s.PricingPolicyId,
        StartDate = s.StartDate,
        ExpiryDate = s.ExpiryDate,
        IsActive = s.IsActive,
        AmountPaid = s.AmountPaid,
        Note = s.Note,
        CreatedAt = s.CreatedAt
    };
}
