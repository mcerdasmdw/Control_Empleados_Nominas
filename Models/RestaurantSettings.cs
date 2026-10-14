namespace Planilla_RRHH.Models;

public class RestaurantSettings
{
    public int Id { get; set; } = 1;
    public string? CommercialName { get; set; }
    public string? LegalId { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string Currency { get; set; } = "CRC";
    public string? LogoPath { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}