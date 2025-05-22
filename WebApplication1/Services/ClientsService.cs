using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class ClientsService
    {
        private readonly ApplicationDbContext _context;

        public ClientsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ClientResponseDto>> GetClients()
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

            return clients;
        }
        public async Task<ClientResponseDto> GetClient(int id)
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
                throw new DivideByZeroException();
            }

            return client;
            
        }
        public async Task<ClientWithOrdersDto> GetClientWithOrders(int id)
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
                throw new DivideByZeroException();
            }

            return client;
        }
        public async Task PutClient(int id, ClientUpdateDto clientDto)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                throw new DivideByZeroException();
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
                    throw new DivideByZeroException();
                }
                else
                {
                    throw;
                }
            }
        }
        public async Task<ClientResponseDto> Create([FromBody] ClientCreateDto clientDto)
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
            return 
                new ClientResponseDto
                {
                    Id = client.Id,
                    Name = client.Name,
                    PhoneNumber = client.PhoneNumber,
                    TotalOrdersCount = 0,
                    DiscountLevel = client.DiscountLevel
                };
        }
        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }
        public async Task DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                throw new DivideByZeroException();
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

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
        public async Task<List<ClientWithDetailedOrdersDto>> SearchClients(
        string searchTerm,
        bool? onlyCompletedOrders = null)
        {
            var query = _context.Clients
                .Include(c => c.Orders)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.Item)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c =>
                    c.Name.ToLower().Contains(searchTerm.ToLower()) ||
                    c.PhoneNumber.Contains(searchTerm));
            }

            var clients = await query.ToListAsync();

            return clients.Select(c => new ClientWithDetailedOrdersDto
            {
                Id = c.Id,
                Name = c.Name,
                PhoneNumber = c.PhoneNumber,
                TotalOrdersCount = c.TotalOrdersCount,
                DiscountLevel = c.DiscountLevel,
                Orders = c.Orders
                    .Where(o => onlyCompletedOrders == null ||
                               o.IsCurrent == !onlyCompletedOrders.Value)
                    .Select(o => MapToOrderResponseDto(o))
                    .ToList()
            }).ToList();
        }
    }

}

