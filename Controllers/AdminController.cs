using BarberFlow.API.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult> Dashboard()
    {
        var hoje = DateTime.Today;
        var amanha = hoje.AddDays(1);
        var agora = DateTime.Now;

        var agendamentosHoje = await _context.Appointments
            .CountAsync(x =>
                x.AppointmentDate >= hoje &&
                x.AppointmentDate < amanha &&
                x.Status == "Agendado");

        var proximosAgendamentos = await _context.Appointments
            .Where(x =>
                x.AppointmentDate >= agora &&
                x.Status == "Agendado")
            .Include(x => x.Barber)
            .Include(x => x.Service)
            .OrderBy(x => x.AppointmentDate)
            .Take(5)
            .Select(x => new
            {
                x.Id,
                x.ClientName,
                x.AppointmentDate,
                BarberName = x.Barber.Name,
                ServiceName = x.Service.Name
            })
            .ToListAsync();

        var totalClientes = await _context.Users
            .CountAsync(x => x.Role == "Client");

        var totalBarbeiros = await _context.Barbers
            .CountAsync();

        var totalServicos = await _context.Services
            .CountAsync();

        return Ok(new
        {
            agendamentosHoje,
            totalClientes,
            totalBarbeiros,
            totalServicos,
            proximosAgendamentos
        });
    }
}