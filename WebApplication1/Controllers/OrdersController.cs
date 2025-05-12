using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item)
                .ToListAsync();

            return orders.Select(MapToOrderResponseDto).ToList();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponseDto>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return MapToOrderResponseDto(order);
        }

        // POST: api/Orders
        [HttpPost]
        public async Task <ActionResult<OrderResponseDto>> Create([FromBody] OrderCreateDto orderDto)
        {
            // Validate client exists
            var client = _context.Clients.FirstOrDefault(c => c.Id == orderDto.ClientId);
            if (client == null) return BadRequest("Client not found");

            // Validate all items exist
            foreach (var itemDto in orderDto.Items)
            {
                if (!_context.Items.Any(i => i.Id == itemDto.ItemId))
                    return BadRequest($"Item with ID {itemDto.ItemId} not found");
            }

            var order = new Order
            {
                ClientId = orderDto.ClientId,
                OrderDate = DateTime.UtcNow,
                Comment = orderDto.Comment,
                IsCurrent = true,
                OrderItems = orderDto.Items.Select(itemDto =>
                {
                    var item = _context.Items.First(i => i.Id == itemDto.ItemId);
                    return new OrderItem
                    {
                        ItemId = item.Id,
                        Quantity = itemDto.Quantity,
                        UnitPrice = item.BasePrice // Store price at time of order
                    };
                }).ToList()
            };

            // Calculate total with discount
            order.TotalPrice = order.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity)
                              * (100 - client.DiscountLevel) / 100;

            _context.Orders.Add(order);

            // Update client's order count
            client.TotalOrdersCount = _context.Orders.Count(o => o.ClientId == client.Id);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id },
                MapToOrderResponseDto(order));
        }

        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, OrderUpdateDto orderDto)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            // Only allow updating certain fields
            order.Comment = orderDto.Comment ?? order.Comment;
            order.IsCurrent = orderDto.IsCurrent ?? order.IsCurrent;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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

        // PATCH: api/Orders/5/complete
        [HttpPatch("{id}/complete")]
        public async Task<IActionResult> MarkAsCompleted(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.IsCurrent = false;
            await _context.SaveChangesAsync();

            return Ok(MapToOrderResponseDto(order));
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            // Update client's order count if client still exists
            var client = await _context.Clients.FindAsync(order.ClientId);
            if (client != null)
            {
                client.TotalOrdersCount = await _context.Orders.CountAsync(o => o.ClientId == client.Id);
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
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

    // DTO classes
}