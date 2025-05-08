using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WebApplication1;
namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly DataService _data;

        public ClientsController(DataService data)
        {
            _data = data;
        }

        // GET api/clients
        [HttpGet]
        public ActionResult<IEnumerable<ClientResponseDto>> GetAll()
        {
            var clients = _data.Clients.Select(c => new ClientResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                PhoneNumber = c.PhoneNumber,
                TotalOrdersCount = c.TotalOrdersCount,
                DiscountLevel = c.DiscountLevel,
                TotalSpent = CalculateTotalSpent(c.Id),
                LastOrderDate = GetLastOrderDate(c.Id)
            });

            return Ok(clients);
        }
        [HttpGet("{id}/orders")]
        public ActionResult<ClientWithOrdersDto> GetClientWithOrders(int id)
        {
            var client = _data.Clients.FirstOrDefault(c => c.Id == id);
            if (client == null) return NotFound();

            var result = new ClientWithOrdersDto
            {
                Id = client.Id,
                Name = client.Name,
                PhoneNumber = client.PhoneNumber,
                TotalOrdersCount = client.TotalOrdersCount,
                DiscountLevel = client.DiscountLevel,
                Orders = _data.Orders 
                    .Where(o => o.ClientId == id)
                    .Select(o => new OrderSummaryDto
                    {
                        Id = o.Id,
                        OrderDate = o.OrderDate,
                        TotalPrice = o.TotalPrice,
                        IsCurrent = o.IsCurrent
                    })
                    .ToList()
            };

            return Ok(result);
        }
        [HttpGet("{id}")]
        public ActionResult<ClientResponseDto> GetById(int id)
        {
            var client = _data.Clients.FirstOrDefault(c => c.Id == id);
            if (client == null) return NotFound();

            return Ok(new ClientResponseDto
            {
                Id = client.Id,
                Name = client.Name,
                PhoneNumber = client.PhoneNumber,
                TotalOrdersCount = client.TotalOrdersCount,
                DiscountLevel = client.DiscountLevel,
                TotalSpent = CalculateTotalSpent(client.Id),
                LastOrderDate = GetLastOrderDate(client.Id)
            });
        }

        [HttpPost]
        public ActionResult<ClientResponseDto> Create([FromBody] ClientCreateDto clientDto)
        {
            var client = new Client
            {
                Id = _data.NextClientId+1,
                Name = clientDto.Name,
                PhoneNumber = clientDto.PhoneNumber,
                DiscountLevel = clientDto.DiscountLevel,
                TotalOrdersCount = 0
            };

            _data.Clients.Add(client);

            return CreatedAtAction(nameof(GetById), new { id = client.Id },
                new ClientResponseDto
                {
                    Id = client.Id,
                    Name = client.Name,
                    PhoneNumber = client.PhoneNumber,
                    TotalOrdersCount = 0,
                    DiscountLevel = client.DiscountLevel
                });
        }

        // PUT api/clients/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ClientUpdateDto clientDto)
        {
            var client = _data.Clients.FirstOrDefault(c => c.Id == id);
            if (client == null) return NotFound();

            if (!string.IsNullOrEmpty(clientDto.Name))
                client.Name = clientDto.Name;

            if (!string.IsNullOrEmpty(clientDto.PhoneNumber))
                client.PhoneNumber = clientDto.PhoneNumber;

            if (clientDto.DiscountLevel.HasValue)
                client.DiscountLevel = clientDto.DiscountLevel.Value;

            return NoContent();
        }

        // DELETE api/clients/5 (unchanged)
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var client = _data.Clients.FirstOrDefault(c => c.Id == id);
            if (client == null) return NotFound();

            _data.Clients.Remove(client);
            return NoContent();
        }

        private decimal CalculateTotalSpent(int clientId)
        {
            // Implement logic to calculate total spent by client
            // You'll need access to orders data
            return 0m; // Placeholder
        }

        private DateTime? GetLastOrderDate(int clientId)
        {
            // Implement logic to get last order date
            // You'll need access to orders data
            return null; // Placeholder
        }
    }
   
   
}


