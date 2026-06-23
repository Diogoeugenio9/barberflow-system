using BarberFlow.API.Context;
using BarberFlow.API.DTOs.Appointments;
using BarberFlow.API.Entities;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        x.AppointmentDate == request.AppointmentDate);

        if (appointmentExists)
        {
            return BadRequest("This time is already booked.");
        }

        var appointment = new Appointment
        {
            ClientName = request.ClientName,
            AppointmentDate = request.AppointmentDate,
            BarberId = request.BarberId,
            ServiceId = request.ServiceId
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

    [HttpGet]
    public async Task<ActionResult<List<AppointmentResponseDto>>> GetAll()
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
                CreatedAt = appointment.CreatedAt
            })
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

