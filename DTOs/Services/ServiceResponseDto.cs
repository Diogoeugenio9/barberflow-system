namespace BarberFlow.API.DTOs.Services
{
    public class ServiceResponseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int DurationInMinutes { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
