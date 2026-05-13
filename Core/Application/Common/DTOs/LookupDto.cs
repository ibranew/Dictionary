

namespace Dictionary.Application.Common.DTOs;

/// <summary>
/// Dropdown / lookup verileri için kullanılan basit DTO.
/// </summary>
public class LookupDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}