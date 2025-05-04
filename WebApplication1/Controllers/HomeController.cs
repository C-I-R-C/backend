using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
namespace WebApplication1.Controllers
{
    public class ClientWithOrdersDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public int TotalOrdersCount { get; set; }
        public int DiscountLevel { get; set; }

        public List<OrderSummaryDto> Orders { get; set; } = new();
    }

    public class OrderSummaryDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsCurrent { get; set; }
    }
    public class OrderWithClientDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsCurrent { get; set; }
        public string Comment { get; set; }

        public ClientInfoDto Client { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class ClientInfoDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DiscountLevel { get; set; }
    }
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly DataService _data;

        public ClientsController(DataService data)
        {
            _data = data;
        }
        
        public class ClientResponseDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public int TotalOrdersCount { get; set; }
            public int DiscountLevel { get; set; }

            // Optionally include summary order info
            public decimal TotalSpent { get; set; }
            public DateTime? LastOrderDate { get; set; }
        }
        public class ClientCreateDto
        {
            [Required]
            [StringLength(100, MinimumLength = 2)]
            public string Name { get; set; }

            [Phone]
            [StringLength(20)]
            public string PhoneNumber { get; set; }

            [Range(0, 100)]
            public int DiscountLevel { get; set; } = 0;
        }
        public class ClientUpdateDto
        {
            [StringLength(100, MinimumLength = 2)]
            public string Name { get; set; }

            [Phone]
            [StringLength(20)]
            public string PhoneNumber { get; set; }

            [Range(0, 100)]
            public int? DiscountLevel { get; set; }
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

    // Supporting DTOs
    public class OrderCreateDto
    {
        public int ClientId { get; set; }
        public string Comment { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
    }
    public class OrderItemResponseDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public ItemDto Item { get; set; }
    }

    public class ItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal BasePrice { get; set; }
    }
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsCurrent { get; set; }
        public string Comment { get; set; }
        public ClientInfoDto Client { get; set; }
        public List<OrderItemResponseDto> Items { get; set; } = new();
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

    // DTOs for items
    public class ItemCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Range(0.01, 10000)]
        public decimal BasePrice { get; set; }
    }

    public class ItemUpdateDto
    {
        [StringLength(100)]
        public string Name { get; set; }

        [Range(0.01, 10000)]
        public decimal BasePrice { get; set; }
    }
}

