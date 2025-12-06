using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClientsController : ControllerBase
    {
        private readonly ClientsService _clientService;
        public ClientsController(ClientsService clientsService)
        {
            _clientService = clientsService;
        }
        [HttpGet]
        public async Task<ActionResult<PagedResult<ClientResponseDto>>> GetClients([FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var parameters = new PaginationParameters
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            return await _clientService.GetClients(parameters);
        }

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
            try
            {
                await _clientService.PutClient(id, clientDto);
                return Ok(); 
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound($"Client with ID {id} no longer exists");

            }
            catch (DivideByZeroException)
            {
                return BadRequest("Client not found");
            }
            catch (AbandonedMutexException)
            {
                return BadRequest("Discount must be between 1 and 100");
            }
            catch (ArgumentException)
            {
                return BadRequest("ID mismatch");
            }
            catch
            {
                return StatusCode(500, "An error occurred while updating the client");
            }
        }

        [HttpPost]
        public async Task <ActionResult<ClientResponseDto>> Create([FromBody] ClientCreateDto clientDto)
        {
            return await _clientService.Create(clientDto);
        }
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
