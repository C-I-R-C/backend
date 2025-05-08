using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
        [ApiController]
        [Route("api/[controller]")]
        public class OrdersController : ControllerBase
        {
            private readonly DataService _data;

            public OrdersController(DataService data)
            {
                _data = data;
            }

            // GET api/orders
            [HttpGet]
            public ActionResult<IEnumerable<OrderResponseDto>> GetAll()
            {
                return Ok(_data.Orders.Select(order => MapToOrderResponseDto(order)));
            }

            // GET api/orders/5
            [HttpGet("{id}")]
            public ActionResult<OrderResponseDto> GetById(int id)
            {
                var order = _data.Orders.FirstOrDefault(o => o.Id == id);
                if (order == null) return NotFound();
                return Ok(MapToOrderResponseDto(order));
            }

            // POST api/orders
            [HttpPost]
            public ActionResult<OrderResponseDto> Create([FromBody] OrderCreateDto orderDto)
            {
                // Validate client exists
                var client = _data.Clients.FirstOrDefault(c => c.Id == orderDto.ClientId);
                if (client == null) return BadRequest("Client not found");

                // Validate all items exist
                foreach (var itemDto in orderDto.Items)
                {
                    if (!_data.Items.Any(i => i.Id == itemDto.ItemId))
                        return BadRequest($"Item with ID {itemDto.ItemId} not found");
                }

                var order = new Order
                {
                    Id = _data.NextOrderId,
                    ClientId = orderDto.ClientId,
                    OrderDate = DateTime.UtcNow,
                    Comment = orderDto.Comment,
                    IsCurrent = true,
                    OrderItems = orderDto.Items.Select(itemDto =>
                    {
                        var item = _data.Items.First(i => i.Id == itemDto.ItemId);
                        return new OrderItem
                        {
                            Id = _data.NextOrderItemId,
                            ItemId = item.Id,
                            Quantity = itemDto.Quantity,
                            UnitPrice = item.BasePrice // Store price at time of order
                        };
                    }).ToList()
                };

                // Calculate total with discount
                order.TotalPrice = order.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity)
                                  * (100 - client.DiscountLevel) / 100;

                _data.Orders.Add(order);

                // Update client's order count
                client.TotalOrdersCount = _data.Orders.Count(o => o.ClientId == client.Id);

                return CreatedAtAction(nameof(GetById), new { id = order.Id },
                    MapToOrderResponseDto(order));
            }

            // PUT api/orders/5
            [HttpPut("{id}")]
            public IActionResult Update(int id, [FromBody] OrderUpdateDto orderDto)
            {
                var order = _data.Orders.FirstOrDefault(o => o.Id == id);
                if (order == null) return NotFound();

                // Only allow updating certain fields
                order.Comment = orderDto.Comment ?? order.Comment;
                order.IsCurrent = orderDto.IsCurrent ?? order.IsCurrent;

                return NoContent();
            }

            // DELETE api/orders/5
            [HttpDelete("{id}")]
            public IActionResult Delete(int id)
            {
                var order = _data.Orders.FirstOrDefault(o => o.Id == id);
                if (order == null) return NotFound();

                // In a real system, you might want to soft delete instead
                _data.Orders.Remove(order);

                // Update client's order count
                var client = _data.Clients.FirstOrDefault(c => c.Id == order.ClientId);
                if (client != null)
                {
                    client.TotalOrdersCount = _data.Orders.Count(o => o.ClientId == client.Id);
                }

                return NoContent();
            }

            // PATCH api/orders/5/complete
            [HttpPatch("{id}/complete")]
            public IActionResult MarkAsCompleted(int id)
            {
                var order = _data.Orders.FirstOrDefault(o => o.Id == id);
                if (order == null) return NotFound();

                order.IsCurrent = false;

                // In a real system, you might deduct inventory here
                // DeductFlowerInventory(order);

                return Ok(MapToOrderResponseDto(order));
            }

            private OrderResponseDto MapToOrderResponseDto(Order order)
            {
                var client = _data.Clients.FirstOrDefault(c => c.Id == order.ClientId);

                return new OrderResponseDto
                {
                    Id = order.Id,
                    OrderDate = order.OrderDate,
                    TotalPrice = order.TotalPrice,
                    IsCurrent = order.IsCurrent,
                    Comment = order.Comment,
                    Client = client != null ? new ClientInfoDto
                    {
                        Id = client.Id,
                        Name = client.Name,
                        DiscountLevel = client.DiscountLevel
                    } : null,
                    Items = order.OrderItems.Select(oi => new OrderItemWithDetailsDto
                    {
                        Id = oi.Id,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        Item = _data.Items
                            .Where(i => i.Id == oi.ItemId)
                            .Select(i => new ItemDto
                            {
                                Id = i.Id,
                                Name = i.Name,
                                BasePrice = i.BasePrice
                            })
                            .FirstOrDefault(),
                        Flowers = _data.ItemFlowers
                            .Where(itemf => itemf.ItemId == oi.ItemId)
                        .Select(itemf => new FlowerDetailDto
                        {
                            FlowerId = itemf.FlowerId,
                            FlowerName = _data.Flowers
                                .FirstOrDefault(f => f.Id == itemf.FlowerId)?.Name ?? "Unknown",
                            QuantityPerItem = itemf.Quantity,
                            TotalQuantity = itemf.Quantity * oi.Quantity,
                            UnitCost = _data.Flowers
                                .FirstOrDefault(f => f.Id == itemf.FlowerId)?.CostPerUnit ?? 0,
                            Color = _data.Flowers
                                .FirstOrDefault(f => f.Id == itemf.FlowerId)?.Color?.Name ?? "N/A",
                            Ingredients = _data.FlowerIngredients
                                .Where(fi => fi.FlowerId == itemf.FlowerId)
                                .Select(fi => new IngredientDto
                                {
                                    Id = fi.IngredientId,
                                    Name = _data.Ingredients
                                        .FirstOrDefault(i => i.Id == fi.IngredientId)?.Name ?? "Unknown",
                                    InStock = _data.Ingredients
                                        .FirstOrDefault(i => i.Id == fi.IngredientId)?.InStock ?? 0,
                                    CostPerUnit = _data.Ingredients
                                        .FirstOrDefault(i => i.Id == fi.IngredientId)?.CostPerUnit ?? 0
                                })
                                .ToList()
                        })
                        .ToList()
                    }).ToList()
                };
            }
        }

}
