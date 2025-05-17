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

        // GET: api/Clients/5/orders
        [HttpGet("{id}/orders")]
        public async Task<ActionResult<ClientWithOrdersDto>> GetClientWithOrders(int id)
        {
            try
            {
                return await _clientService.GetClientWithOrders(id);
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
        [HttpGet("{clientId}/allOrders")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetClientOrders(int clientId)
        {
            try
            {
                return await GetClientOrders(clientId);
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

        
    }
}
