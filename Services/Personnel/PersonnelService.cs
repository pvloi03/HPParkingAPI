using HPParkingAPI.Models.DTOs.Personnel;
using HPParkingAPI.Models.Entities.Personnel;
using HPParkingAPI.Repository.Interfaces;

namespace HPParkingAPI.Services.Personnel;

public class PersonnelService : IPersonnelService
{
    private readonly IRepository<Person> _personRepo;
    private readonly IRepository<Contractor> _contractorRepo;

    public PersonnelService(IRepository<Person> personRepo, IRepository<Contractor> contractorRepo)
    {
        _personRepo = personRepo;
        _contractorRepo = contractorRepo;
    }

    public async Task<PersonResponseDto> CreatePersonAsync(CreatePersonDto dto)
    {
        if (await _personRepo.ExistsAsync(p => p.SiteId == dto.SiteId && p.CardNumber == dto.CardNumber))
        {
            throw new InvalidOperationException($"Mã thẻ '{dto.CardNumber}' đã tồn tại trong địa điểm này.");
        }

        var person = new Person
        {
            CardNumber = dto.CardNumber,
            FullName = dto.FullName,
            SiteId = dto.SiteId,
            Role = dto.Role,
            IdentityNumber = dto.IdentityNumber,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            Department = dto.Department,
            ContractorId = dto.ContractorId,
            ApartmentNumber = dto.ApartmentNumber,
            FaceImageUrl = dto.FaceImageUrl,
            IsAllowedEntry = dto.IsAllowedEntry,
            CardExpiryDate = dto.CardExpiryDate
        };

        await _personRepo.InsertAsync(person);
        return MapToPersonDto(person);
    }

    public async Task<PersonResponseDto?> UpdatePersonAsync(string id, UpdatePersonDto dto)
    {
        var person = await _personRepo.GetByIdAsync(id);
        if (person is null) return null;

        person.FullName = dto.FullName;
        person.Role = dto.Role;
        person.PhoneNumber = dto.PhoneNumber;
        person.Email = dto.Email;
        person.Department = dto.Department;
        person.ContractorId = dto.ContractorId;
        person.ApartmentNumber = dto.ApartmentNumber;
        person.FaceImageUrl = dto.FaceImageUrl;
        person.IsAllowedEntry = dto.IsAllowedEntry;
        person.IsBlacklisted = dto.IsBlacklisted;
        person.BlacklistReason = dto.BlacklistReason;
        person.CardExpiryDate = dto.CardExpiryDate;
        person.UpdatedAt = DateTime.UtcNow;

        await _personRepo.UpdateAsync(person);
        return MapToPersonDto(person);
    }

    public async Task<PersonResponseDto?> GetPersonByIdAsync(string id)
    {
        var person = await _personRepo.GetByIdAsync(id);
        return person is null ? null : MapToPersonDto(person);
    }

    public async Task<PersonResponseDto?> GetPersonByCardNumberAsync(string cardNumber)
    {
        var person = await _personRepo.FindOneAsync(p => p.CardNumber == cardNumber);
        return person is null ? null : MapToPersonDto(person);
    }

    public async Task<List<PersonResponseDto>> GetPersonsBySiteAsync(string siteId)
    {
        var persons = await _personRepo.FindAsync(p => p.SiteId == siteId);
        return [.. persons.Select(MapToPersonDto)];
    }

    public async Task<bool> DeletePersonAsync(string id)
    {
        return await _personRepo.DeleteAsync(id);
    }

    public async Task<ContractorDto> CreateContractorAsync(CreateContractorDto dto)
    {
        if (await _contractorRepo.ExistsAsync(c => c.Code == dto.Code))
        {
            throw new InvalidOperationException($"Mã nhà thầu '{dto.Code}' đã tồn tại.");
        }

        var contractor = new Contractor
        {
            Code = dto.Code,
            Name = dto.Name,
            ContactPerson = dto.ContactPerson,
            PhoneNumber = dto.PhoneNumber,
            IsActive = true
        };

        await _contractorRepo.InsertAsync(contractor);
        return MapToContractorDto(contractor);
    }

    public async Task<List<ContractorDto>> GetAllContractorsAsync()
    {
        var contractors = await _contractorRepo.GetAllAsync();
        return [.. contractors.Select(MapToContractorDto)];
    }

    private static PersonResponseDto MapToPersonDto(Person p) => new()
    {
        Id = p.Id,
        CardNumber = p.CardNumber,
        FullName = p.FullName,
        SiteId = p.SiteId,
        Role = p.Role,
        IdentityNumber = p.IdentityNumber,
        PhoneNumber = p.PhoneNumber,
        Email = p.Email,
        Department = p.Department,
        ContractorId = p.ContractorId,
        ApartmentNumber = p.ApartmentNumber,
        FaceImageUrl = p.FaceImageUrl,
        IsAllowedEntry = p.IsAllowedEntry,
        IsBlacklisted = p.IsBlacklisted,
        BlacklistReason = p.BlacklistReason,
        CardExpiryDate = p.CardExpiryDate,
        CreatedAt = p.CreatedAt
    };

    private static ContractorDto MapToContractorDto(Contractor c) => new()
    {
        Id = c.Id,
        Code = c.Code,
        Name = c.Name,
        ContactPerson = c.ContactPerson,
        PhoneNumber = c.PhoneNumber,
        IsActive = c.IsActive
    };
}
