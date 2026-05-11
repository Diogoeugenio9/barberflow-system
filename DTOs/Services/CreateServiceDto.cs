namespace BarberFlow.API.DTOs.Services
{
    public class CreateServiceDto
    {
        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int DurationInMinutes { get; set; }
    }
}
