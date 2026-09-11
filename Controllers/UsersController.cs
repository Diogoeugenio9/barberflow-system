using BarberFlow.API.Context;
using BarberFlow.API.DTOs.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("clients")]
    public async Task<ActionResult<List<UserResponseDto>>> GetClients()
    {
        var clients = await _context.Users
            .Where(x => x.Role == "Client")
            .Select(x => new UserResponseDto
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
                Phone = x.Phone,
                Role = x.Role,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return Ok(clients);
    }

    [HttpGet("clients/{id}")]
    public async Task<ActionResult<UserResponseDto>> GetClientById(Guid id)
    {
        var client = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == id && x.Role == "Client");

        if (client == null)
        {
            return NotFound("Client not found.");
        }

        var response = new UserResponseDto
        {
            Id = client.Id,
            Name = client.Name,
            Email = client.Email,
            Phone = client.Phone,
            Role = client.Role,
            CreatedAt = client.CreatedAt
        };

        return Ok(response);
    }

    [HttpPut("clients/{id}")]
    public async Task<ActionResult<UserResponseDto>> UpdateClient(
        Guid id,
        UpdateUserDto request)
    {
        var client = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == id && x.Role == "Client");

        if (client == null)
        {
            return NotFound("Client not found.");
        }

        var emailExists = await _context.Users
            .AnyAsync(x => x.Email == request.Email && x.Id != id);

        if (emailExists)
        {
            return BadRequest("Email already exists.");
        }

        client.Name = request.Name;
        client.Email = request.Email;
        client.Phone = request.Phone;

        await _context.SaveChangesAsync();

        var response = new UserResponseDto
        {
            Id = client.Id,
            Name = client.Name,
            Email = client.Email,
            Phone = client.Phone,
            Role = client.Role,
            CreatedAt = client.CreatedAt
        };

        return Ok(response);
    }

    [HttpDelete("clients/{id}")]
    public async Task<ActionResult> DeleteClient(Guid id)
    {
        var client = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == id && x.Role == "Client");

        if (client == null)
        {
            return NotFound("Client not found.");
        }

        _context.Users.Remove(client);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}