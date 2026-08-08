using Asp.Versioning;
using HPParkingAPI.Models.DTOs.Personnel;
using HPParkingAPI.Services.Personnel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HPParkingAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/personnel")]
[Produces("application/json")]
[Authorize]
public class PersonnelController : ControllerBase
{
    private readonly IPersonnelService _personnelService;

    public PersonnelController(IPersonnelService personnelService)
    {
        _personnelService = personnelService;
    }

    /// <summary>Đăng ký nhân sự mới (Công nhân / Kỹ sư / Cư dân)</summary>
    [HttpPost("persons")]
    [ProducesResponseType(typeof(PersonResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreatePerson([FromBody] CreatePersonDto dto)
    {
        try
        {
            var result = await _personnelService.CreatePersonAsync(dto);
            return CreatedAtAction(nameof(GetPersonById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>Cập nhật thông tin nhân sự</summary>
    [HttpPut("persons/{id}")]
    [ProducesResponseType(typeof(PersonResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePerson(string id, [FromBody] UpdatePersonDto dto)
    {
        var result = await _personnelService.UpdatePersonAsync(id, dto);
        if (result is null) return NotFound(new { message = "Không tìm thấy thông tin nhân sự." });
        return Ok(result);
    }

    /// <summary>Lấy danh sách nhân sự theo SiteId</summary>
    [HttpGet("persons")]
    [ProducesResponseType(typeof(List<PersonResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPersonsBySite([FromQuery] string siteId)
    {
        var result = await _personnelService.GetPersonsBySiteAsync(siteId);
        return Ok(result);
    }

    /// <summary>Lấy thông tin nhân sự theo ID</summary>
    [HttpGet("persons/{id}")]
    [ProducesResponseType(typeof(PersonResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPersonById(string id)
    {
        var result = await _personnelService.GetPersonByIdAsync(id);
        if (result is null) return NotFound(new { message = "Không tìm thấy thông tin nhân sự." });
        return Ok(result);
    }

    /// <summary>Tra cứu nhân sự theo Mã thẻ RFID</summary>
    [HttpGet("persons/card/{cardNumber}")]
    [ProducesResponseType(typeof(PersonResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPersonByCardNumber(string cardNumber)
    {
        var result = await _personnelService.GetPersonByCardNumberAsync(cardNumber);
        if (result is null) return NotFound(new { message = "Thẻ không có trong hệ thống." });
        return Ok(result);
    }

    /// <summary>Xóa thông tin nhân sự</summary>
    [HttpDelete("persons/{id}")]
    [Authorize(Roles = "SuperAdmin,SiteAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePerson(string id)
    {
        var success = await _personnelService.DeletePersonAsync(id);
        if (!success) return NotFound(new { message = "Không tìm thấy thông tin nhân sự." });
        return NoContent();
    }

    /// <summary>Tạo nhà thầu mới</summary>
    [HttpPost("contractors")]
    [Authorize(Roles = "SuperAdmin,SiteAdmin")]
    [ProducesResponseType(typeof(ContractorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateContractor([FromBody] CreateContractorDto dto)
    {
        try
        {
            var result = await _personnelService.CreateContractorAsync(dto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>Lấy danh sách tất cả nhà thầu</summary>
    [HttpGet("contractors")]
    [ProducesResponseType(typeof(List<ContractorDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllContractors()
    {
        var result = await _personnelService.GetAllContractorsAsync();
        return Ok(result);
    }
}
