using HPParkingAPI.Models.DTOs.Parking;
using HPParkingAPI.Models.Entities.Parking;
using HPParkingAPI.Models.Entities.Vehicles;
using HPParkingAPI.Repository.Interfaces;

namespace HPParkingAPI.Services.Parking;

public class PricingService : IPricingService
{
    private readonly IRepository<PricingPolicy> _policyRepo;

    public PricingService(IRepository<PricingPolicy> policyRepo)
    {
        _policyRepo = policyRepo;
    }

    public async Task<decimal> CalculateParkingFeeAsync(
        string siteId,
        VehicleCategory category,
        TicketType ticketType,
        DateTime inTime,
        DateTime outTime)
    {
        if (ticketType == TicketType.Monthly)
            return 0;

        var policy = await _policyRepo.FindOneAsync(p =>
            p.SiteId == siteId &&
            p.ApplicableCategory == category &&
            p.IsActive);

        if (policy is null)
        {
            // Default fallback pricing if no policy configured yet
            var defaultHours = (int)Math.Ceiling((outTime - inTime).TotalHours);
            if (defaultHours <= 0) defaultHours = 1;

            return category switch
            {
                VehicleCategory.Motorbike => defaultHours * 5000m,
                VehicleCategory.Car => defaultHours * 20000m,
                VehicleCategory.Truck => defaultHours * 35000m,
                VehicleCategory.Container => defaultHours * 50000m,
                _ => defaultHours * 10000m
            };
        }

        var totalMinutes = (outTime - inTime).TotalMinutes;
        if (totalMinutes < 0) totalMinutes = 0;

        if (totalMinutes <= policy.FreeGraceMinutes)
            return 0;

        decimal totalFee = 0;

        if (policy.PricingType == PricingType.Daily || policy.PricingType == PricingType.Monthly)
        {
            totalFee = policy.FlatPrice;
        }
        else
        {
            var totalHours = (int)Math.Ceiling(totalMinutes / 60.0);
            if (totalHours <= policy.FirstBlockHours)
            {
                totalFee = policy.FirstBlockPrice;
            }
            else
            {
                var extraHours = totalHours - policy.FirstBlockHours;
                totalFee = policy.FirstBlockPrice + (extraHours * policy.PricePerHourAfter);
            }

            if (policy.MaxDailyPrice > 0 && totalFee > policy.MaxDailyPrice)
            {
                totalFee = policy.MaxDailyPrice;
            }
        }

        // Add overnight surcharge if parked overnight
        if (inTime.Date != outTime.Date && policy.OvernightPrice > 0)
        {
            var totalDaysOvernight = (outTime.Date - inTime.Date).Days;
            totalFee += totalDaysOvernight * policy.OvernightPrice;
        }

        return totalFee;
    }

    public async Task<PricingPolicyResponseDto> CreatePolicyAsync(CreatePricingPolicyDto dto)
    {
        var policy = new PricingPolicy
        {
            SiteId = dto.SiteId,
            Name = dto.Name,
            ApplicableCategory = dto.ApplicableCategory,
            PricingType = dto.PricingType,
            FreeGraceMinutes = dto.FreeGraceMinutes,
            FirstBlockPrice = dto.FirstBlockPrice,
            FirstBlockHours = dto.FirstBlockHours,
            PricePerHourAfter = dto.PricePerHourAfter,
            FlatPrice = dto.FlatPrice,
            MaxDailyPrice = dto.MaxDailyPrice,
            OvernightPrice = dto.OvernightPrice,
            IsActive = true
        };

        await _policyRepo.InsertAsync(policy);
        return MapToResponse(policy);
    }

    public async Task<List<PricingPolicyResponseDto>> GetPoliciesBySiteAsync(string siteId)
    {
        var policies = await _policyRepo.FindAsync(p => p.SiteId == siteId && p.IsActive);
        return [.. policies.Select(MapToResponse)];
    }

    public async Task<PricingPolicyResponseDto?> GetPolicyByIdAsync(string id)
    {
        var policy = await _policyRepo.GetByIdAsync(id);
        return policy is null ? null : MapToResponse(policy);
    }

    public async Task<bool> DeactivatePolicyAsync(string id)
    {
        var policy = await _policyRepo.GetByIdAsync(id);
        if (policy is null) return false;

        policy.IsActive = false;
        policy.UpdatedAt = DateTime.UtcNow;
        await _policyRepo.UpdateAsync(policy);
        return true;
    }

    private static PricingPolicyResponseDto MapToResponse(PricingPolicy p) => new()
    {
        Id = p.Id,
        SiteId = p.SiteId,
        Name = p.Name,
        ApplicableCategory = p.ApplicableCategory,
        PricingType = p.PricingType,
        FreeGraceMinutes = p.FreeGraceMinutes,
        FirstBlockPrice = p.FirstBlockPrice,
        FirstBlockHours = p.FirstBlockHours,
        PricePerHourAfter = p.PricePerHourAfter,
        FlatPrice = p.FlatPrice,
        MaxDailyPrice = p.MaxDailyPrice,
        OvernightPrice = p.OvernightPrice,
        IsActive = p.IsActive,
        CreatedAt = p.CreatedAt
    };
}
