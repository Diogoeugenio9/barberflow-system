namespace BarberFlow.API.Entities;

public class Appointment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string ClientName { get; set; } = string.Empty;

    public DateTime AppointmentDate { get; set; }

    public Guid BarberId { get; set; }

    public Barber Barber { get; set; } = null!;

    public Guid ServiceId { get; set; }

    public Service Service { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}