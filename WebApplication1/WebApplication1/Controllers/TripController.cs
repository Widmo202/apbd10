using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ApbdContext _dbContext;

    public TripController(IConfiguration configuration, ApbdContext dbContext)
    {
        _configuration = configuration;
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetTrips(int page = 1, int pageSize = 10)
    {
        var totalTrips = await _dbContext.Trips.CountAsync();
        var allPages = (totalTrips + pageSize - 1) / pageSize;

        var trips = await _dbContext.Trips
            .Include(t => t.IdCountries)
            .Include(t => t.ClientTrips)
            .OrderByDescending(t => t.DateFrom)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var clientIds = trips.SelectMany(t => t.ClientTrips.Select(ct => ct.IdClient)).Distinct();
        var clients = await _dbContext.Clients.Where(c => clientIds.Contains(c.IdClient)).ToDictionaryAsync(c => c.IdClient);

        var response = new 
        {
            pageNum = page,
            pageSize = pageSize,
            allPages = allPages,
            trips = trips.Select(t => new 
            {
                t.Name,
                t.Description,
                t.DateFrom,
                t.DateTo,
                t.MaxPeople,
                Countries = t.IdCountries.Select(c => new { c.Name }),
                Clients = t.ClientTrips.Select(ct => new { FirstName = clients[ct.IdClient].FirstName, LastName = clients[ct.IdClient].LastName })
            })
        };

        return Ok(response);
    }
}