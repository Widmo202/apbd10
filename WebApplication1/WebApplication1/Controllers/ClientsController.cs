using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ApbdContext _dbContext;

    public ClientsController(IConfiguration configuration, ApbdContext dbContext)
    {
        _configuration = configuration;
        _dbContext = dbContext;
    }

    [HttpDelete("{idClient}")]
    public async Task<IActionResult> DeleteClient(int idClient, CancellationToken token)
    {

        var clientToRemove = new Client()
        {
            IdClient = idClient
        };

        if (_dbContext.ClientTrips.Any(c => c.IdClient == clientToRemove.IdClient))
        {
            return BadRequest("Client is signed for a trip and therefore can not be removed.");
        }

        var entry = _dbContext.Entry(clientToRemove);
        entry.State = EntityState.Deleted;

        await _dbContext.SaveChangesAsync(token);
        
        return Ok("Client has ben deleted \n"+entry);
    }

}