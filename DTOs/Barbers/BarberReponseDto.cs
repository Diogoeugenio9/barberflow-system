namespace BarberFlow.API.DTOs.Barbers;

public class BarberResponseDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Specialty { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}