using System.ComponentModel.DataAnnotations;
using HPParkingAPI.Models.Entities.Personnel;

namespace HPParkingAPI.Models.DTOs.Personnel;

// ===================== PERSON DTOs =====================

public class CreatePersonDto
{
    [Required(ErrorMessage = "Ma the khong duoc de trong")]
    public string CardNumber { get; set; } = null!;

    [Required(ErrorMessage = "Ho ten khong duoc de trong")]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "SiteId khong duoc de trong")]
    public string SiteId { get; set; } = null!;

    public PersonRole Role { get; set; } = PersonRole.Worker;

    public string? IdentityNumber { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }

    // Cong truong
    public string? Department { get; set; }
    public string? ContractorId { get; set; }

    // Chung cu
    public string? ApartmentNumber { get; set; }

    public string? FaceImageUrl { get; set; }
    public bool IsAllowedEntry { get; set; } = true;
    public DateTime? CardExpiryDate { get; set; }
}

public class UpdatePersonDto
{
    public string FullName { get; set; } = null!;
    public PersonRole Role { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Department { get; set; }
    public string? ContractorId { get; set; }
    public string? ApartmentNumber { get; set; }
    public string? FaceImageUrl { get; set; }
    public bool IsAllowedEntry { get; set; }
    public bool IsBlacklisted { get; set; }
    public string? BlacklistReason { get; set; }
    public DateTime? CardExpiryDate { get; set; }
}

public class PersonResponseDto
{
    public string Id { get; set; } = null!;
    public string CardNumber { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string SiteId { get; set; } = null!;
    public PersonRole Role { get; set; }
    public string? IdentityNumber { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Department { get; set; }
    public string? ContractorId { get; set; }
    public string? ApartmentNumber { get; set; }
    public string? FaceImageUrl { get; set; }
    public bool IsAllowedEntry { get; set; }
    public bool IsBlacklisted { get; set; }
    public string? BlacklistReason { get; set; }
    public DateTime? CardExpiryDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

// ===================== CONTRACTOR DTOs =====================

public class ContractorDto
{
    public string Id { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? ContactPerson { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
}

public class CreateContractorDto
{
    [Required(ErrorMessage = "Ma nha thau khong duoc de trong")]
    public string Code { get; set; } = null!;

    [Required(ErrorMessage = "Ten nha thau khong duoc de trong")]
    public string Name { get; set; } = null!;

    public string? ContactPerson { get; set; }
    public string? PhoneNumber { get; set; }
}
