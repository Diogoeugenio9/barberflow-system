namespace BarberFlow.API.DTOs.Appointments
{
    public class CreateAppointmentDto
    {
        public string ClientName { get; set; } = string.Empty;

        public DateTime AppointmentDate { get; set; }

        public Guid BarberId { get; set; }

        public Guid ServiceId { get; set; }
    }
}
