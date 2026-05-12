namespace BarberFlow.API.DTOs.Appointments;

public class AppointmentResponseDto
{
    public Guid Id { get; set; }

    public string ClientName { get; set; } = string.Empty;

    public DateTime AppointmentDate { get; set; }

    public string BarberName { get; set; } = string.Empty;

    public string ServiceName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}