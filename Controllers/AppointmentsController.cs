using BarberFlow.API.Context;
using BarberFlow.API.DTOs.Appointments;
using BarberFlow.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BarberFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AppointmentsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentResponseDto>> Create(
    CreateAppointmentDto request)
    {
        var barber = await _context.Barbers
            .FirstOrDefaultAsync(x => x.Id == request.BarberId);

        if (barber == null)
        {
            return BadRequest("Barber not found.");
        }

        var service = await _context.Services
            .FirstOrDefaultAsync(x => x.Id == request.ServiceId);

        if (service == null)
        {
            return BadRequest("Serviço não encontrado.");
        }

        if (request.AppointmentDate < DateTime.Now)
        {
            return BadRequest("A data e o horário do agendamento não podem estar no passado.");
        }

        var hour = request.AppointmentDate.Hour;

        if (hour < 9 || hour >= 18)
        {
            return BadRequest("O horário está fora do horário de funcionamento.");
        }

        var minute = request.AppointmentDate.Minute;

        if (minute % 10 != 0)
        {
            return BadRequest("O horário deve ser marcado de 10 em 10 minutos.");
        }

        var appointmentEnd = request.AppointmentDate.AddMinutes(30);

        var appointmentExists = await _context.Appointments
            .AnyAsync(x =>
                x.BarberId == request.BarberId &&
                x.Status == "Agendado" &&
                x.AppointmentDate < appointmentEnd &&
                x.AppointmentDate.AddMinutes(30) > request.AppointmentDate);

        if (appointmentExists)
        {
            return BadRequest("Este horário está ocupado.");
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Unauthorized();
        }

        var appointment = new Appointment
        {
            ClientName = request.ClientName,
            AppointmentDate = request.AppointmentDate,
            BarberId = request.BarberId,
            ServiceId = request.ServiceId,
            UserId = Guid.Parse(userId),
            Status = "Agendado"
        };

        _context.Appointments.Add(appointment);

        await _context.SaveChangesAsync();

        var response = new AppointmentResponseDto
        {
            Id = appointment.Id,
            ClientName = appointment.ClientName,
            AppointmentDate = appointment.AppointmentDate,
            BarberName = barber.Name,
            ServiceName = service.Name,
            CreatedAt = appointment.CreatedAt
        };

        return Ok(response);
    }

    [HttpGet("available")]
    public async Task<ActionResult> GetAvailable(
    Guid barberId,
    DateTime date)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        var appointments = await _context.Appointments
            .Where(x =>
                x.BarberId == barberId &&
                x.AppointmentDate >= startOfDay &&
                x.AppointmentDate < endOfDay &&
                x.Status == "Agendado")
            .ToListAsync();

        var availableTimes = new List<TimeSpan>();

        for (
            var time = TimeSpan.FromHours(9);
            time <= TimeSpan.FromHours(17).Add(TimeSpan.FromMinutes(30));
            time = time.Add(TimeSpan.FromMinutes(10)))
        {
            var appointmentDateTime = date.Date.Add(time);
            var appointmentEnd = appointmentDateTime.AddMinutes(30);

            var ocupado = appointments.Any(x =>
                x.AppointmentDate < appointmentEnd &&
                x.AppointmentDate.AddMinutes(30) > appointmentDateTime);

            if (!ocupado)
            {
                availableTimes.Add(time);
            }
        }

        return Ok(availableTimes);
    }

    [HttpGet]
    public async Task<ActionResult<List<AppointmentResponseDto>>> GetAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Unauthorized();
        }

        var appointments = await _context.Appointments
            .Where(x => x.UserId == Guid.Parse(userId))
            .Include(x => x.Barber)
            .Include(x => x.Service)
            .Select(appointment => new AppointmentResponseDto
            {
                Id = appointment.Id,
                ClientName = appointment.ClientName,
                AppointmentDate = appointment.AppointmentDate,
                BarberName = appointment.Barber.Name,
                ServiceName = appointment.Service.Name,
                CreatedAt = appointment.CreatedAt,
                Status = appointment.Status
            })
            .OrderBy(x => x.AppointmentDate)
            .ToListAsync();

        return Ok(appointments);
    }

    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<AppointmentResponseDto>>> GetAllForAdmin()
    {
        var appointments = await _context.Appointments
            .Include(x => x.Barber)
            .Include(x => x.Service)
            .Select(appointment => new AppointmentResponseDto
            {
                Id = appointment.Id,
                ClientName = appointment.ClientName,
                AppointmentDate = appointment.AppointmentDate,
                BarberName = appointment.Barber.Name,
                ServiceName = appointment.Service.Name,
                CreatedAt = appointment.CreatedAt,
                Status = appointment.Status
            })
            .OrderBy(x => x.AppointmentDate)
            .ToListAsync();

        return Ok(appointments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentResponseDto>> GetById(Guid id)
    {
        var appointment = await _context.Appointments
            .Include(x => x.Barber)
            .Include(x => x.Service)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (appointment == null)
        {
            return NotFound("Appointment not found.");
        }

        var response = new AppointmentResponseDto
        {
            Id = appointment.Id,
            ClientName = appointment.ClientName,
            AppointmentDate = appointment.AppointmentDate,
            BarberName = appointment.Barber.Name,
            ServiceName = appointment.Service.Name,
            CreatedAt = appointment.CreatedAt
        };

        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AppointmentResponseDto>> Update(
    Guid id,
    CreateAppointmentDto request)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(x => x.Id == id);

        if (appointment == null)
        {
            return NotFound("Appointment not found.");
        }

        var barber = await _context.Barbers
            .FirstOrDefaultAsync(x => x.Id == request.BarberId);

        if (barber == null)
        {
            return BadRequest("Barber not found.");
        }

        var service = await _context.Services
            .FirstOrDefaultAsync(x => x.Id == request.ServiceId);

        if (service == null)
        {
            return BadRequest("Service not found.");
        }

        if (request.AppointmentDate < DateTime.UtcNow)
        {
            return BadRequest("Appointment date cannot be in the past.");
        }

        var hour = request.AppointmentDate.Hour;

        if (hour < 9 || hour >= 18)
        {
            return BadRequest("Appointment outside business hours.");
        }

        var appointmentExists = await _context.Appointments
            .AnyAsync(x =>
                x.BarberId == request.BarberId &&
                x.Status == "Agendado" &&
                x.AppointmentDate == request.AppointmentDate &&
                x.Id != id);

        if (appointmentExists)
        {
            return BadRequest("This time is already booked.");
        }


        appointment.ClientName = request.ClientName;
        appointment.AppointmentDate = request.AppointmentDate;
        appointment.BarberId = request.BarberId;
        appointment.ServiceId = request.ServiceId;

        await _context.SaveChangesAsync();

        var response = new AppointmentResponseDto
        {
            Id = appointment.Id,
            ClientName = appointment.ClientName,
            AppointmentDate = appointment.AppointmentDate,
            BarberName = barber.Name,
            ServiceName = service.Name,
            CreatedAt = appointment.CreatedAt
        };

        return Ok(response);
    }

    [HttpPut("{id}/cancel")]
    public async Task<ActionResult> Cancel(Guid id)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(x => x.Id == id);

        if (appointment == null)
        {
            return NotFound("Appointment not found.");
        }

        if (appointment.Status == "Cancelado")
        {
            return BadRequest("Este agendamento já está cancelado.");
        }

        appointment.Status = "Cancelado";

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Agendamento cancelado com sucesso."
        });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(x => x.Id == id);

        if (appointment == null)
        {
            return NotFound("Appointment not found.");
        }

        _context.Appointments.Remove(appointment);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}

