using HPParkingAPI.Models.DTOs.Personnel;

namespace HPParkingAPI.Services.Personnel;

public interface IPersonnelService
{
    Task<PersonResponseDto> CreatePersonAsync(CreatePersonDto dto);
    Task<PersonResponseDto?> UpdatePersonAsync(string id, UpdatePersonDto dto);
    Task<PersonResponseDto?> GetPersonByIdAsync(string id);
    Task<PersonResponseDto?> GetPersonByCardNumberAsync(string cardNumber);
    Task<List<PersonResponseDto>> GetPersonsBySiteAsync(string siteId);
    Task<bool> DeletePersonAsync(string id);

    Task<ContractorDto> CreateContractorAsync(CreateContractorDto dto);
    Task<List<ContractorDto>> GetAllContractorsAsync();
}
