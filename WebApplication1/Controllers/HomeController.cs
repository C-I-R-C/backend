using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
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
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly DataService _data;

        public OrdersController(DataService data)
        {
            _data = data;
        }


        private static List<Item> _items = new List<Item>
        {
            new Item { Id = 1, Name = "Rose Bouquet", BasePrice = 25.99m },
            new Item { Id = 2, Name = "Tulip Arrangement", BasePrice = 19.99m },
            new Item { Id = 3, Name = "Wedding Flowers", BasePrice = 199.99m }
        };

        // GET api/orders
        [HttpGet]
        public ActionResult<IEnumerable<OrderResponseDto>> GetAll()
        {
            return Ok(_data.Orders.Select(order => new OrderResponseDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                IsCurrent = order.IsCurrent,
                Comment = order.Comment,
                Client = _data.Clients
                    .Where(c => c.Id == order.ClientId)
                    .Select(c => new ClientInfoDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        DiscountLevel = c.DiscountLevel
                    })
                    .FirstOrDefault(),
                Items = order.OrderItems.Select(oi => new OrderItemResponseDto
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
                        .FirstOrDefault()
                }).ToList()
            }));
        }
        [HttpGet("with-clients")]
        public ActionResult<IEnumerable<OrderWithClientDto>> GetAllWithClients()
        {
            var result = _data.Orders.Select(o => new OrderWithClientDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalPrice = o.TotalPrice,
                IsCurrent = o.IsCurrent,
                Comment = o.Comment,
                Client = _data.Clients
                    .Where(c => c.Id == o.ClientId)
                    .Select(c => new ClientInfoDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        DiscountLevel = c.DiscountLevel
                    })
                    .FirstOrDefault(),
                Items = o.OrderItems.Select(oi => new OrderItemDto
                {
                    ItemId = oi.ItemId,
                    Quantity = oi.Quantity,
                }).ToList()
            });

            return Ok(result);
        }
        [HttpPost]
        public ActionResult<OrderResponseDto> Create([FromBody] OrderCreateDto orderDto)
        {
            var client = _data.Clients.FirstOrDefault(c => c.Id == orderDto.ClientId);
            if (client == null) return BadRequest("Client not found");

            var order = new Order
            {
                Id = _data.NextOrderId,
                ClientId = orderDto.ClientId,
                OrderDate = DateTime.UtcNow,
                Comment = orderDto.Comment,
                IsCurrent = true,
                OrderItems = orderDto.Items.Select(i =>
                {
                    var item = _data.Items.FirstOrDefault(it => it.Id == i.ItemId);
                    if (item == null)
                        throw new ArgumentException($"Item with ID {i.ItemId} not found");

                    return new OrderItem
                    {
                        Id = _data.NextOrderItemId,
                        ItemId = i.ItemId,
                        Quantity = i.Quantity,
                        UnitPrice = item.BasePrice // Use current price
                    };
                }).ToList()
            };

            order.TotalPrice = order.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity)
                               * (100 - client.DiscountLevel) / 100;

            _data.Orders.Add(order);
            client.TotalOrdersCount = _data.Orders.Count(o => o.ClientId == client.Id);

            return CreatedAtAction(nameof(GetById), new { id = order.Id },
                MapToOrderResponseDto(order, client));
        }

        private OrderResponseDto MapToOrderResponseDto(Order order, Client client)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                IsCurrent = order.IsCurrent,
                Comment = order.Comment,
                Client = new ClientInfoDto
                {
                    Id = client.Id,
                    Name = client.Name,
                    DiscountLevel = client.DiscountLevel
                },
                Items = order.OrderItems.Select(oi => new OrderItemResponseDto
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
                        .FirstOrDefault()
                }).ToList()
            };
        }

        private decimal CalculateTotal(List<OrderItemDto> items, int discountLevel)
        {
            // Implement your pricing logic here
            return 0;
        }

        private OrderWithClientDto MapToOrderWithClientDto(Order order, Client client)
        {
            return new OrderWithClientDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                IsCurrent = order.IsCurrent,
                Comment = order.Comment,
                Client = new ClientInfoDto
                {
                    Id = client.Id,
                    Name = client.Name,
                    DiscountLevel = client.DiscountLevel
                }
            };
        }

        // GET api/orders/5
        [HttpGet("{id}")]
        public ActionResult<OrderResponseDto> GetById(int id)
        {
            var order = _data.Orders.FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();

            var client = _data.Clients.FirstOrDefault(c => c.Id == order.ClientId);

            return Ok(new OrderResponseDto
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
                Items = order.OrderItems.Select(oi => new OrderItemResponseDto
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
                        .FirstOrDefault()
                }).ToList()
            });
        }

        // Helper method
        private decimal CalculateOrderTotal(List<OrderItemDto> items)
        {
            decimal total = 0;
            foreach (var item in items)
            {
                var product = _items.FirstOrDefault(p => p.Id == item.ItemId);
                if (product != null)
                {
                    total += product.BasePrice * item.Quantity;
                }
            }
            return total;
        }
    }
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly DataService _data;

        public ItemsController(DataService data)
        {
            _data = data;
        }

        // GET api/items
        [HttpGet]
        public ActionResult<IEnumerable<Item>> GetAll()
        {
            return Ok(_data.Items);
        }

        // GET api/items/5
        [HttpGet("{id}")]
        public ActionResult<Item> GetById(int id)
        {
            var item = _data.Items.FirstOrDefault(i => i.Id == id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        // POST api/items
        [HttpPost]
        public ActionResult<Item> Create([FromBody] ItemCreateDto itemDto)
        {
            var item = new Item
            {
                Id = _data.NextItemId,
                Name = itemDto.Name,
                BasePrice = itemDto.BasePrice
            };

            _data.Items.Add(item);
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        // PUT api/items/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ItemUpdateDto itemDto)
        {
            var item = _data.Items.FirstOrDefault(i => i.Id == id);
            if (item == null) return NotFound();

            item.Name = itemDto.Name;
            item.BasePrice = itemDto.BasePrice;

            return NoContent();
        }

        // DELETE api/items/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var item = _data.Items.FirstOrDefault(i => i.Id == id);
            if (item == null) return NotFound();

            // Check if item is used in any orders
            if (_data.Orders.Any(o => o.OrderItems.Any(oi => oi.ItemId == id)))
            {
                return BadRequest("Cannot delete item referenced in existing orders");
            }

            _data.Items.Remove(item);
            return NoContent();
        }
    }
    [ApiController]
    [Route("api/orders/{orderId}/flowers")]
    public class OrderFlowersController : ControllerBase
    {
        private readonly DataService _data;

        public OrderFlowersController(DataService data)
        {
            _data = data;
        }

        // GET api/orders/5/flowers
        [HttpGet]
        public ActionResult<OrderFlowersResponseDto> GetFlowersForOrder(int orderId)
        {
            var order = _data.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null) return NotFound("Order not found");

            var result = new OrderFlowersResponseDto
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                ClientName = _data.Clients.FirstOrDefault(c => c.Id == order.ClientId)?.Name,
                Flowers = order.OrderItems
                    .SelectMany(oi => _data.ItemFlowers
                        .Where(itemf => itemf.ItemId == oi.ItemId)
                    .Select(itemf => new FlowerUsageDto
                    {
                        FlowerId = itemf.FlowerId,
                        FlowerName = _data.Flowers.FirstOrDefault(f => f.Id == itemf.FlowerId)?.Name ?? "Unknown",
                        QuantityUsed = itemf.Quantity * oi.Quantity, // Total across all items
                        UnitCost = _data.Flowers.FirstOrDefault(f => f.Id == itemf.FlowerId)?.CostPerUnit ?? 0,
                        Color = _data.Flowers
                            .FirstOrDefault(f => f.Id == itemf.FlowerId)?
                            .Color?.Name ?? "N/A",
                        Items = new List<ItemFlowerUsageDto>
                        {
                            new ItemFlowerUsageDto
                            {
                                ItemId = oi.ItemId,
                                ItemName = _data.Items.FirstOrDefault(i => i.Id == oi.ItemId)?.Name,
                                QuantityInItem = itemf.Quantity,
                                ItemQuantity = oi.Quantity
                                }
    }
                    }))
                .GroupBy(f => f.FlowerId)
                .Select(g => new FlowerUsageDto
                {
                    FlowerId = g.Key,
                    FlowerName = g.First().FlowerName,
                    QuantityUsed = g.Sum(x => x.QuantityUsed),
                    UnitCost = g.First().UnitCost,
                    Color = g.First().Color,
                    Items = g.SelectMany(x => x.Items).ToList()
                })
                .ToList()
            };

            return Ok(result);
        }
    }
    [ApiController]
    [Route("api/[controller]")]
    public class FlowersController : ControllerBase
    {
        private readonly DataService _data;

        public FlowersController(DataService data)
        {
            _data = data;
        }

        // GET api/flowers
        [HttpGet]
        public ActionResult<IEnumerable<FlowerDto>> GetAll()
        {
            var flowers = _data.Flowers.Select(f => new FlowerDto
            {
                Id = f.Id,
                Name = f.Name,
                InStock = f.InStock,
                CostPerUnit = f.CostPerUnit,
                Color = f.Color != null ? new ColorDto
                {
                    Id = f.Color.Id,
                    Name = f.Color.Name,
                    IsNatural = f.Color.IsNatural
                } : null
            });

            return Ok(flowers);
        }

        // GET api/flowers/5
        [HttpGet("{id}")]
        public ActionResult<FlowerWithIngredientsDto> GetById(int id)
        {
            var flower = _data.Flowers.FirstOrDefault(f => f.Id == id);
            if (flower == null) return NotFound();

            return Ok(new FlowerWithIngredientsDto
            {
                Id = flower.Id,
                Name = flower.Name,
                InStock = flower.InStock,
                CostPerUnit = flower.CostPerUnit,
                Color = flower.Color != null ? new ColorDto
                {
                    Id = flower.Color.Id,
                    Name = flower.Color.Name,
                    IsNatural = flower.Color.IsNatural
                } : null,
                Ingredients = _data.FlowerIngredients
                    .Where(fi => fi.FlowerId == id)
                    .Select(fi => new FlowerIngredientDto
                    {
                        FlowerId = fi.FlowerId,
                        IngredientId = fi.IngredientId,
                        QuantityRequired = fi.QuantityRequired,
                        Ingredient = _data.Ingredients
                            .Where(i => i.Id == fi.IngredientId)
                            .Select(i => new IngredientDto
                            {
                                Id = i.Id,
                                Name = i.Name,
                                InStock = i.InStock,
                                CostPerUnit = i.CostPerUnit
                            })
                            .FirstOrDefault()
                    })
                    .ToList()
            });
        }

        // POST api/flowers
        [HttpPost]
        public ActionResult<FlowerDto> Create([FromBody] FlowerCreateDto flowerDto)
        {
            var flower = new Flower
            {
                Id = _data.NextFlowerId,
                Name = flowerDto.Name,
                InStock = flowerDto.InStock,
                CostPerUnit = flowerDto.CostPerUnit,
                ColorId = flowerDto.ColorId
            };

            _data.Flowers.Add(flower);
            return CreatedAtAction(nameof(GetById), new { id = flower.Id },
                new FlowerDto
                {
                    Id = flower.Id,
                    Name = flower.Name,
                    InStock = flower.InStock,
                    CostPerUnit = flower.CostPerUnit
                });
        }

        // PUT api/flowers/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] FlowerUpdateDto flowerDto)
        {
            var flower = _data.Flowers.FirstOrDefault(f => f.Id == id);
            if (flower == null) return NotFound();

            flower.Name = flowerDto.Name;
            flower.InStock = flowerDto.InStock;
            flower.CostPerUnit = flowerDto.CostPerUnit;
            flower.ColorId = flowerDto.ColorId;

            return NoContent();
        }

        // DELETE api/flowers/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var flower = _data.Flowers.FirstOrDefault(f => f.Id == id);
            if (flower == null) return NotFound();

            // Check if flower is used in any items
            if (_data.ItemFlowers.Any(itemf => itemf.FlowerId == id))
        {
                return BadRequest("Cannot delete flower used in items");
            }

            _data.Flowers.Remove(flower);
            return NoContent();
        }
    }

    [ApiController]
    [Route("api/orders/{orderId}/items/{itemId}/flowers")]
    public class OrderItemFlowersController : ControllerBase
    {
        private readonly DataService _data;

        public OrderItemFlowersController(DataService data)
        {
            _data = data;
        }

        // GET api/orders/5/items/1/flowers
        [HttpGet]
        public ActionResult<OrderItemFlowersResponseDto> GetFlowersForOrderItem(
            int orderId, int itemId)
        {
            var order = _data.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null) return NotFound("Order not found");

            var orderItem = order.OrderItems.FirstOrDefault(oi => oi.ItemId == itemId);
            if (orderItem == null) return NotFound("Item not found in order");

            var item = _data.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null) return NotFound("Item not found");

            var result = new OrderItemFlowersResponseDto
            {
                OrderId = order.Id,
                ItemId = item.Id,
                ItemName = item.Name,
                ItemQuantity = orderItem.Quantity,
                Flowers = _data.ItemFlowers
                    .Where(itemf => itemf.ItemId == itemId)
                .Select(itemf => new FlowerDetailDto
                {
                    FlowerId = itemf.FlowerId,
                    FlowerName = _data.Flowers.FirstOrDefault(f => f.Id == itemf.FlowerId)?.Name ?? "Unknown",
                    QuantityPerItem = itemf.Quantity,
                    TotalQuantity = itemf.Quantity * orderItem.Quantity,
                    UnitCost = _data.Flowers.FirstOrDefault(f => f.Id == itemf.FlowerId)?.CostPerUnit ?? 0,
                    Color = _data.Flowers
                        .FirstOrDefault(f => f.Id == itemf.FlowerId)?
                        .Color?.Name ?? "N/A"
                })
                .ToList()
            };

            return Ok(result);
        }
    }
        [ApiController]
        [Route("api/[controller]")]
        public class ColorsController : ControllerBase
        {
            private readonly DataService _data;

            public ColorsController(DataService data)
            {
                _data = data;
            }

            // GET api/colors
            [HttpGet]
            public ActionResult<IEnumerable<ColorDto>> GetAll()
            {
                var colors = _data.Colors.Select(c => new ColorDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    IsNatural = c.IsNatural
                });

                return Ok(colors);
            }

            // GET api/colors/5
            [HttpGet("{id}")]
            public ActionResult<ColorDto> GetById(int id)
            {
                var color = _data.Colors.FirstOrDefault(c => c.Id == id);
                if (color == null) return NotFound();

                return Ok(new ColorDto
                {
                    Id = color.Id,
                    Name = color.Name,
                    IsNatural = color.IsNatural
                });
            }

            // POST api/colors
            [HttpPost]
            public ActionResult<ColorDto> Create([FromBody] ColorCreateDto colorDto)
            {
                var color = new Color
                {
                    Id = _data.NextColorId,
                    Name = colorDto.Name,
                    IsNatural = colorDto.IsNatural
                };

                _data.Colors.Add(color);

                return CreatedAtAction(nameof(GetById), new { id = color.Id },
                    new ColorDto
                    {
                        Id = color.Id,
                        Name = color.Name,
                        IsNatural = color.IsNatural
                    });
            }

            // PUT api/colors/5
            [HttpPut("{id}")]
            public IActionResult Update(int id, [FromBody] ColorUpdateDto colorDto)
            {
                var color = _data.Colors.FirstOrDefault(c => c.Id == id);
                if (color == null) return NotFound();

                if (!string.IsNullOrEmpty(colorDto.Name))
                    color.Name = colorDto.Name;

                if (colorDto.IsNatural.HasValue)
                    color.IsNatural = colorDto.IsNatural.Value;

                return NoContent();
            }

            // DELETE api/colors/5
            [HttpDelete("{id}")]
            public IActionResult Delete(int id)
            {
                var color = _data.Colors.FirstOrDefault(c => c.Id == id);
                if (color == null) return NotFound();

                // Check if color is used by any flowers
                if (_data.Flowers.Any(f => f.ColorId == id))
                {
                    return BadRequest("Cannot delete color assigned to flowers");
                }

                _data.Colors.Remove(color);
                return NoContent();
            }
        }
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientsController : ControllerBase
    {
        private readonly DataService _data;

        public IngredientsController(DataService data)
        {
            _data = data;
        }

        // GET api/ingredients
        [HttpGet]
        public ActionResult<IEnumerable<IngredientDto>> GetAll()
        {
            return Ok(_data.Ingredients.Select(i => new IngredientDto
            {
                Id = i.Id,
                Name = i.Name,
                InStock = i.InStock,
                CostPerUnit = i.CostPerUnit
            }));
        }

        // GET api/ingredients/5
        [HttpGet("{id}")]
        public ActionResult<IngredientDto> GetById(int id)
        {
            var ingredient = _data.Ingredients.FirstOrDefault(i => i.Id == id);
            if (ingredient == null) return NotFound();

            return Ok(new IngredientDto
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                InStock = ingredient.InStock,
                CostPerUnit = ingredient.CostPerUnit
            });
        }

        // POST api/ingredients
        [HttpPost]
        public ActionResult<IngredientDto> Create([FromBody] IngredientCreateDto dto)
        {
            var ingredient = new Ingredient
            {
                Id = _data.NextIngredientId,
                Name = dto.Name,
                InStock = dto.InStock,
                CostPerUnit = dto.CostPerUnit
            };

            _data.Ingredients.Add(ingredient);
            return CreatedAtAction(nameof(GetById), new { id = ingredient.Id },
                new IngredientDto
                {
                    Id = ingredient.Id,
                    Name = ingredient.Name,
                    InStock = ingredient.InStock,
                    CostPerUnit = ingredient.CostPerUnit
                });
        }

        // PUT api/ingredients/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] IngredientCreateDto dto)
        {
            var ingredient = _data.Ingredients.FirstOrDefault(i => i.Id == id);
            if (ingredient == null) return NotFound();

            ingredient.Name = dto.Name;
            ingredient.InStock = dto.InStock;
            ingredient.CostPerUnit = dto.CostPerUnit;

            return NoContent();
        }

        // DELETE api/ingredients/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var ingredient = _data.Ingredients.FirstOrDefault(i => i.Id == id);
            if (ingredient == null) return NotFound();

            if (_data.FlowerIngredients.Any(fi => fi.IngredientId == id))
            {
                return BadRequest("Cannot delete ingredient used in flower recipes");
            }

            _data.Ingredients.Remove(ingredient);
            return NoContent();
        }
    }
    [ApiController]
    [Route("api/flowers/{flowerId}/ingredients")]
    public class FlowerIngredientsController : ControllerBase
    {
        private readonly DataService _data;

        public FlowerIngredientsController(DataService data)
        {
            _data = data;
        }

        // GET api/flowers/1/ingredients
        [HttpGet]
        public ActionResult<IEnumerable<FlowerIngredientDto>> GetIngredientsForFlower(int flowerId)
        {
            if (!_data.Flowers.Any(f => f.Id == flowerId))
                return NotFound("Flower not found");

            return Ok(_data.FlowerIngredients
                .Where(fi => fi.FlowerId == flowerId)
                .Select(fi => new FlowerIngredientDto
                {
                    FlowerId = fi.FlowerId,
                    IngredientId = fi.IngredientId,
                    QuantityRequired = fi.QuantityRequired,
                    Ingredient = _data.Ingredients
                        .Where(i => i.Id == fi.IngredientId)
                        .Select(i => new IngredientDto
                        {
                            Id = i.Id,
                            Name = i.Name,
                            InStock = i.InStock,
                            CostPerUnit = i.CostPerUnit
                        })
                        .FirstOrDefault()
                }));
        }

        // POST api/flowers/1/ingredients
        [HttpPost]
        public ActionResult<FlowerIngredientDto> AddIngredientToFlower(
            int flowerId, [FromBody] AddIngredientToFlowerDto dto)
        {
            if (!_data.Flowers.Any(f => f.Id == flowerId))
                return NotFound("Flower not found");

            if (!_data.Ingredients.Any(i => i.Id == dto.IngredientId))
                return NotFound("Ingredient not found");

            var existing = _data.FlowerIngredients
                .FirstOrDefault(fi => fi.FlowerId == flowerId && fi.IngredientId == dto.IngredientId);

            if (existing != null)
            {
                existing.QuantityRequired = dto.QuantityRequired;
            }
            else
            {
                _data.FlowerIngredients.Add(new FlowerIngredient
                {
                    FlowerId = flowerId,
                    IngredientId = dto.IngredientId,
                    QuantityRequired = dto.QuantityRequired
                });
            }

            var ingredient = _data.Ingredients.First(i => i.Id == dto.IngredientId);
            return CreatedAtAction(
                nameof(GetIngredientsForFlower),
                new { flowerId },
                new FlowerIngredientDto
                {
                    FlowerId = flowerId,
                    IngredientId = dto.IngredientId,
                    QuantityRequired = dto.QuantityRequired,
                    Ingredient = new IngredientDto
                    {
                        Id = ingredient.Id,
                        Name = ingredient.Name,
                        InStock = ingredient.InStock,
                        CostPerUnit = ingredient.CostPerUnit
                    }
                });
        }

        // DELETE api/flowers/1/ingredients/2
        [HttpDelete("{ingredientId}")]
        public IActionResult RemoveIngredientFromFlower(int flowerId, int ingredientId)
        {
            var flowerIngredient = _data.FlowerIngredients
                .FirstOrDefault(fi => fi.FlowerId == flowerId && fi.IngredientId == ingredientId);

            if (flowerIngredient == null)
                return NotFound("Ingredient not found in this flower recipe");

            _data.FlowerIngredients.Remove(flowerIngredient);
            return NoContent();
        }
    }

    // Supporting DTOs
    public class AddIngredientToFlowerDto
    {
        [Range(1, int.MaxValue)]
        public int IngredientId { get; set; }

        [Range(1, 100)]
        public int QuantityRequired { get; set; }
    }
}


