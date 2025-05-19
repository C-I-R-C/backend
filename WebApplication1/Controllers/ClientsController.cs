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


        // PUT: api/Clients/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClient(int id, ClientUpdateDto clientDto)
        {
            try
            {
                await PutClient(id, clientDto);
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

                // Pagination
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
