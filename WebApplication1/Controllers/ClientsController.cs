using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // DTO classes

        // GET: api/Clients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientResponseDto>>> GetClients()
        {
            var clients = await _context.Clients
                .Select(c => new ClientResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    PhoneNumber = c.PhoneNumber,
                    TotalOrdersCount = c.TotalOrdersCount,
                    DiscountLevel = c.DiscountLevel,
                    TotalSpent = _context.Orders
                        .Where(o => o.ClientId == c.Id)
                        .Sum(o => o.TotalPrice),
                    LastOrderDate = _context.Orders
                        .Where(o => o.ClientId == c.Id)
                        .Max(o => (DateTime?)o.OrderDate)
                })
                .ToListAsync();

            return Ok(clients);
        }

        // GET: api/Clients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientResponseDto>> GetClient(int id)
        {
            var client = await _context.Clients
                .Where(c => c.Id == id)
                .Select(c => new ClientResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    PhoneNumber = c.PhoneNumber,
                    TotalOrdersCount = c.TotalOrdersCount,
                    DiscountLevel = c.DiscountLevel,
                    TotalSpent = _context.Orders
                        .Where(o => o.ClientId == c.Id)
                        .Sum(o => o.TotalPrice),
                    LastOrderDate = _context.Orders
                        .Where(o => o.ClientId == c.Id)
                        .Max(o => (DateTime?)o.OrderDate)
                })
                .FirstOrDefaultAsync();

            if (client == null)
            {
                return NotFound();
            }

            return client;
        }

        // GET: api/Clients/5/orders
        [HttpGet("{id}/orders")]
        public async Task<ActionResult<ClientWithOrdersDto>> GetClientWithOrders(int id)
        {
            var client = await _context.Clients
                .Where(c => c.Id == id)
                .Select(c => new ClientWithOrdersDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    PhoneNumber = c.PhoneNumber,
                    TotalOrdersCount = c.TotalOrdersCount,
                    DiscountLevel = c.DiscountLevel,
                    Orders = _context.Orders
                        .Where(o => o.ClientId == c.Id)
                        .Select(o => new OrderSummaryDto
                        {
                            Id = o.Id,
                            OrderDate = o.OrderDate,
                            TotalPrice = o.TotalPrice,
                            IsCurrent = o.IsCurrent
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (client == null)
            {
                return NotFound();
            }

            return client;
        }

        // PUT: api/Clients/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClient(int id, ClientUpdateDto clientDto)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(clientDto.Name))
                client.Name = clientDto.Name;

            if (!string.IsNullOrEmpty(clientDto.PhoneNumber))
                client.PhoneNumber = clientDto.PhoneNumber;

            if (clientDto.DiscountLevel.HasValue)
                client.DiscountLevel = clientDto.DiscountLevel.Value;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Clients
        [HttpPost]
        public async Task <ActionResult<ClientResponseDto>> Create([FromBody] ClientCreateDto clientDto)
        {
            var client = new Client
            {
                Name = clientDto.Name,
                PhoneNumber = clientDto.PhoneNumber,
                DiscountLevel = clientDto.DiscountLevel,
                TotalOrdersCount = 0
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetClient), new { id = client.Id },
                new ClientResponseDto
                {
                    Id = client.Id,
                    Name = client.Name,
                    PhoneNumber = client.PhoneNumber,
                    TotalOrdersCount = 0,
                    DiscountLevel = client.DiscountLevel
                });
        }
        [HttpGet("{clientId}/allOrders")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetClientOrders(int clientId)
        {
            // Check if client exists
            if (!await _context.Clients.AnyAsync(c => c.Id == clientId))
            {
                return NotFound("Client not found");
            }

            var orders = await _context.Orders
                .Where(o => o.ClientId == clientId)
                .Include(o => o.Client)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item)
                        .ThenInclude(i => i.ItemFlowers)
                            .ThenInclude(itemf => itemf.Flower)
        .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Item)
                .ThenInclude(i => i.Box)
        .OrderByDescending(o => o.OrderDate)
        .ToListAsync();

            var orderDtos = orders.Select(o => MapToOrderResponseDto(o)).ToList();
            return Ok(orderDtos);
        }
        // DELETE: api/Clients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }
        private OrderResponseDto MapToOrderResponseDto(Order order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                IsCurrent = order.IsCurrent,
                Comment = order.Comment,
                Client = order.Client != null ? new ClientInfoDto
                {
                    Id = order.Client.Id,
                    Name = order.Client.Name,
                    DiscountLevel = order.Client.DiscountLevel
                } : null,
                Items = order.OrderItems?.Select(oi => new OrderItemWithDetailsDto
                {
                    Id = oi.Id,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    Item = oi.Item != null ? new ItemDto
                    {
                        Id = oi.Item.Id,
                        Name = oi.Item.Name,
                        BasePrice = oi.Item.BasePrice
                    } : null,
                    Flowers = _context.ItemFlowers
                    .Where(itemf => itemf.ItemId == oi.ItemId)
                    .Include(itemf => itemf.Flower)
                        .ThenInclude(f => f.Color)
                    .Include(itemf => itemf.Flower)
                        .ThenInclude(f => f.FlowerIngredients)
                            .ThenInclude(fi => fi.Ingredient)
                    .Select(itemf => new FlowerDetailDto
                    {
                        FlowerId = itemf.FlowerId,
                        FlowerName = itemf.Flower != null ? itemf.Flower.Name : "Unknown",
                        QuantityPerItem = itemf.Quantity,
                        TotalQuantity = itemf.Quantity * oi.Quantity,
                        UnitCost = itemf.Flower != null ? itemf.Flower.CostPerUnit : 0,
                        Color = itemf.Flower != null && itemf.Flower.Color != null
                            ? itemf.Flower.Color.Name
                            : "N/A",
                        Ingredients = itemf.Flower != null && itemf.Flower.FlowerIngredients != null
                            ? itemf.Flower.FlowerIngredients
                                .Select(fi => new IngredientDto
                                {
                                    Id = fi.IngredientId,
                                    Name = fi.Ingredient != null ? fi.Ingredient.Name : "Unknown",
                                    InStock = fi.Ingredient != null ? fi.Ingredient.InStock : 0,
                                    CostPerUnit = fi.Ingredient != null ? fi.Ingredient.CostPerUnit : 0
                                })
                                .ToList()
                            : new List<IngredientDto>()
                    })
                    .ToList()
                }).ToList() ?? new List<OrderItemWithDetailsDto>()
            };
        }
    }
}
