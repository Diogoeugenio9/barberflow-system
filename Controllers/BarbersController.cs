using BarberFlow.API.Context;
using BarberFlow.API.DTOs.Barbers;
using BarberFlow.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BarbersController : ControllerBase
{
    private readonly AppDbContext _context;

    public BarbersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<BarberResponseDto>> Create(
    CreateBarberDto request)
    {
        var barber = new Barber
        {
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            Specialty = request.Specialty
        };

        _context.Barbers.Add(barber);

        await _context.SaveChangesAsync();

        var response = new BarberResponseDto
        {
            Id = barber.Id,
            Name = barber.Name,
            Email = barber.Email,
            Phone = barber.Phone,
            Specialty = barber.Specialty,
            CreatedAt = barber.CreatedAt
        };

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<List<BarberResponseDto>>> GetAll()
    {
        var barbers = await _context.Barbers
            .Select(barber => new BarberResponseDto
            {
                Id = barber.Id,
                Name = barber.Name,
                Email = barber.Email,
                Phone = barber.Phone,
                Specialty = barber.Specialty,
                CreatedAt = barber.CreatedAt
            })
            .ToListAsync();

        return Ok(barbers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BarberResponseDto>> GetById(Guid id)
    {
        var barber = await _context.Barbers
            .FirstOrDefaultAsync(x => x.Id == id);

        if (barber == null)
        {
            return NotFound("Barber not found.");
        }

        var response = new BarberResponseDto
        {
            Id = barber.Id,
            Name = barber.Name,
            Email = barber.Email,
            Phone = barber.Phone,
            Specialty = barber.Specialty,
            CreatedAt = barber.CreatedAt
        };

        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BarberResponseDto>> Update(
    Guid id,
    UpdateBarberDto request)
    {
        var barber = await _context.Barbers
            .FirstOrDefaultAsync(x => x.Id == id);

        if (barber == null)
        {
            return NotFound("Barber not found.");
        }

        barber.Name = request.Name;
        barber.Email = request.Email;
        barber.Phone = request.Phone;
        barber.Specialty = request.Specialty;

        await _context.SaveChangesAsync();

        var response = new BarberResponseDto
        {
            Id = barber.Id,
            Name = barber.Name,
            Email = barber.Email,
            Phone = barber.Phone,
            Specialty = barber.Specialty,
            CreatedAt = barber.CreatedAt
        };

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var barber = await _context.Barbers
            .FirstOrDefaultAsync(x => x.Id == id);

        if (barber == null)
        {
            return NotFound("Barber not found.");
        }

        _context.Barbers.Remove(barber);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}




    

