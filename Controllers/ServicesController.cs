using BarberFlow.API.Context;
using BarberFlow.API.DTOs.Services;
using BarberFlow.API.Entities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ServicesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<ServiceResponseDto>> Create(
    CreateServiceDto request)
    {
        var service = new Service
        {
            Name = request.Name,
            Price = request.Price,
            DurationInMinutes = request.DurationInMinutes
        };

        _context.Services.Add(service);

        await _context.SaveChangesAsync();

        var response = new ServiceResponseDto
        {
            Id = service.Id,
            Name = service.Name,
            Price = service.Price,
            DurationInMinutes = service.DurationInMinutes,
            CreatedAt = service.CreatedAt
        };

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<List<ServiceResponseDto>>> GetAll()
    {
        var services = await _context.Services
            .Select(service => new ServiceResponseDto
            {
                Id = service.Id,
                Name = service.Name,
                Price = service.Price,
                DurationInMinutes = service.DurationInMinutes,
                CreatedAt = service.CreatedAt
            })
            .ToListAsync();

        return Ok(services);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResponseDto>> GetById(Guid id)
    {
        var service = await _context.Services
            .FirstOrDefaultAsync(x => x.Id == id);

        if (service == null)
        {
            return NotFound("Service not found.");
        }

        var response = new ServiceResponseDto
        {
            Id = service.Id,
            Name = service.Name,
            Price = service.Price,
            DurationInMinutes = service.DurationInMinutes,
            CreatedAt = service.CreatedAt
        };

        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ServiceResponseDto>> Update(
    Guid id,
    UpdateServiceDto request)
    {
        var service = await _context.Services
            .FirstOrDefaultAsync(x => x.Id == id);

        if (service == null)
        {
            return NotFound("Service not found.");
        }

        service.Name = request.Name;
        service.Price = request.Price;
        service.DurationInMinutes = request.DurationInMinutes;

        await _context.SaveChangesAsync();

        var response = new ServiceResponseDto
        {
            Id = service.Id,
            Name = service.Name,
            Price = service.Price,
            DurationInMinutes = service.DurationInMinutes,
            CreatedAt = service.CreatedAt
        };

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var service = await _context.Services
            .FirstOrDefaultAsync(x => x.Id == id);

        if (service == null)
        {
            return NotFound("Service not found.");
        }

        _context.Services.Remove(service);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}