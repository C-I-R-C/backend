using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ClientsService _clientService;
        public ClientsController(ApplicationDbContext context, ClientsService clientsService)
        {
            _context = context;
            _clientService = clientsService;
        }

        // DTO classes

        // GET: api/Clients
        [HttpGet]
        public async Task<ActionResult<List<ClientResponseDto>>> GetClients()
        {
            return await _clientService.GetClients();
        }

        // GET: api/Clients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientResponseDto>> GetClient(int id, bool isWithOrders)
        {
            try
            {
                return await _clientService.GetClient(id);
            }
            catch (DivideByZeroException) {
                return NotFound();
            }
            catch
            {
                return Problem();
            }

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutClient(int id, [FromBody] ClientUpdateDto clientDto)
        {
            // Validate input
            if (clientDto == null)
                return BadRequest("Client data is required");

            if (id != clientDto.Id)
                return BadRequest("ID mismatch");

            // Find existing client
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
                return NotFound($"Client with ID {id} not found");

            // Update fields if provided
            if (!string.IsNullOrWhiteSpace(clientDto.Name))
                client.Name = clientDto.Name.Trim();

            if (!string.IsNullOrWhiteSpace(clientDto.PhoneNumber))
                client.PhoneNumber = clientDto.PhoneNumber.Trim();

            if (clientDto.DiscountLevel.HasValue)
            {
                // Validate discount range (0-100)
                if (clientDto.DiscountLevel < 0 || clientDto.DiscountLevel > 100)
                    return BadRequest("Discount must be between 0 and 100");

                client.DiscountLevel = clientDto.DiscountLevel.Value;
            }

            try
            {
                await _context.SaveChangesAsync();
                return NoContent(); // Standard response for successful PUT
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ClientExists(id))
                    return NotFound($"Client with ID {id} no longer exists");

                return Conflict("The client was modified by another user. Please refresh and try again.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the client");
            }
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }
        // POST: api/Clients
        [HttpPost]
        public async Task <ActionResult<ClientResponseDto>> Create([FromBody] ClientCreateDto clientDto)
        {
            return await _clientService.Create(clientDto);
        }
        // DELETE: api/Clients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            try
            {
                await _clientService.DeleteClient(id);
                return Ok();
            }
            catch (DivideByZeroException)
            {
                return NotFound();
            }
            catch
            {
                return Problem();
            }
        }
        [HttpGet("search")]
        public async Task<ActionResult<List<ClientWithDetailedOrdersDto>>> SearchClients(
        [FromQuery] string searchTerm,
        [FromQuery] bool? completedOrders = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest("Search term cannot be empty");
                }

                var clients = await _clientService.SearchClients(searchTerm, completedOrders);
                var pagedClients = clients
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Ok(new
                {
                    TotalCount = clients.Count,
                    Page = page,
                    PageSize = pageSize,
                    Results = pagedClients
                });
            }
            catch
            {
                return Problem();
            }
        }

    }
}
