using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DTOs;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class OrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrderResponseDto>> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item)
                .ToListAsync();

            return orders.Select(MapToOrderResponseDto).ToList();
        }
        public async Task<OrderResponseDto> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                throw new DivideByZeroException();
            }

            return MapToOrderResponseDto(order);
        }
        public async Task<OrderResponseDto> Create([FromBody] OrderCreateDto orderDto)
        {
            // Validate client exists
            var client = _context.Clients.FirstOrDefault(c => c.Id == orderDto.ClientId);
            if (client == null) throw new ArgumentException();

            // Validate all items exist
            foreach (var itemDto in orderDto.Items)
            {
                if (!_context.Items.Any(i => i.Id == itemDto.ItemId))
                    throw new AbandonedMutexException();
            }

            var order = new Order
            {
                ClientId = orderDto.ClientId,
                OrderDate = DateTime.UtcNow,
                OrderCompleteDate = orderDto.OrderCompleteDate ?? DateTime.MinValue,
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
            return MapToOrderResponseDto(order);
        }
        public async Task PutOrder(int id, OrderUpdateDto orderDto)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                throw new DivideByZeroException();
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
                    throw new DivideByZeroException();
                }
                else
                {
                    throw;
                }
            }
        }
        public async Task<OrderResponseDto> MarkAsCompleted(int id)
        {
            // Load the order with its items
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {id} not found");
            }

            if (!order.IsCurrent)
            {
                throw new InvalidOperationException("Order is already completed");
            }
            var itemIds = order.OrderItems.Select(oi => oi.ItemId).ToList();

            var itemsWithFlowers = await _context.Items
                .Where(i => itemIds.Contains(i.Id))
                .Include(i => i.ItemFlowers)
                    .ThenInclude(itemf => itemf.Flower)
        .ToListAsync();

            // Create a dictionary for quick lookup
            var itemsDictionary = itemsWithFlowers.ToDictionary(i => i.Id);
            // Process each order item
            foreach (var orderItem in order.OrderItems)
            {
                if (!itemsDictionary.TryGetValue(orderItem.ItemId, out var item))
                {
                    throw new KeyNotFoundException($"Item with ID {orderItem.ItemId} not found");
                }

                foreach (var itemFlower in item.ItemFlowers)
                {
                    var flower = itemFlower.Flower;
                    if (flower == null) continue;

                    var quantityToDeduct = itemFlower.Quantity * orderItem.Quantity;

                    if (flower.InStock < quantityToDeduct)
                    {
                        throw new InvalidOperationException(
                            $"Not enough stock for flower {flower.Name}. " +
                            $"Required: {quantityToDeduct}, Available: {flower.InStock}");
                    }

                    flower.InStock -= quantityToDeduct;

                }
                if (item.BoxId.HasValue)
                {
                    var box = await _context.Boxes.FindAsync(item.BoxId.Value);
                    if (box != null)
                    {
                        box.InStock -= 1;
                    }
                }
            }

            order.IsCurrent = false;
            order.OrderCompleteDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return MapToOrderResponseDto(order);
        }
        public async Task DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                throw new DivideByZeroException();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            var client = await _context.Clients.FindAsync(order.ClientId);
            if (client != null)
            {
                client.TotalOrdersCount = await _context.Orders.CountAsync(o => o.ClientId == client.Id);
                await _context.SaveChangesAsync();
            }
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
                OrderCompleteDate = order.OrderCompleteDate,
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
        public async Task<PagedResponse<OrderResponseDto>> QueryOrders(
    DateTime? orderDate,
    DateTime? completionDate,
    bool? isCompleted,
    int pageNumber = 1,
    int pageSize = 20)
        {
            // 1. Prepare the base query with includes
            var baseQuery = _context.Orders
                .Include(o => o.Client)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item)
                .AsQueryable();

            // 2. Convert input dates to UTC dates (date-only)
            var utcOrderDate = orderDate.HasValue
                ? orderDate.Value.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(orderDate.Value.Date, DateTimeKind.Utc)
                    : orderDate.Value.Date.ToUniversalTime()
                : (DateTime?)null;

            var utcCompletionDate = completionDate.HasValue
                ? completionDate.Value.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(completionDate.Value.Date, DateTimeKind.Utc)
                    : completionDate.Value.Date.ToUniversalTime()
                : (DateTime?)null;

            // 3. Apply filters with PostgreSQL-compatible date comparison
            if (utcOrderDate.HasValue)
            {
                baseQuery = baseQuery.Where(o =>
                    o.OrderDate >= utcOrderDate.Value &&
                    o.OrderDate < utcOrderDate.Value.AddDays(1));
            }

            if (utcCompletionDate.HasValue)
            {
                baseQuery = baseQuery.Where(o =>
                    o.OrderCompleteDate >= utcCompletionDate.Value &&
                    o.OrderCompleteDate < utcCompletionDate.Value.AddDays(1));
            }

            if (isCompleted.HasValue)
            {
                baseQuery = baseQuery.Where(o => o.IsCurrent == !isCompleted.Value);
            }

            // 4. Get total count (before pagination)
            var totalCount = await baseQuery.CountAsync();

            // 5. Apply pagination with ordering
            var orders = await baseQuery
    .OrderByDescending(o => o.OrderDate)
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .Select(o => new OrderResponseDto
    {
        Id = o.Id,
        OrderDate = o.OrderDate,
        OrderCompleteDate = o.OrderCompleteDate,
        TotalPrice = o.TotalPrice,
        IsCurrent = o.IsCurrent,
        Comment = o.Comment,
        Client = o.Client != null ? new ClientInfoDto
        {
            Id = o.Client.Id,
            Name = o.Client.Name,
            DiscountLevel = o.Client.DiscountLevel
        } : null,
        Items = o.OrderItems.Select(oi => new OrderItemWithDetailsDto
        {
            Id = oi.Id,
            Quantity = oi.Quantity,
            UnitPrice = oi.UnitPrice,
            Item = oi.Item != null ? new ItemDto
            {
                Id = oi.Item.Id,
                Name = oi.Item.Name,
                BasePrice = oi.Item.BasePrice
            } : null
            // Include other OrderItemWithDetailsDto properties as needed
        }).ToList()
    })
    .ToListAsync();
            // 6. Return the paged result
            return new PagedResponse<OrderResponseDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = orders
            };
        }
        public async Task<List<UrgentOrderDto>> GetMostUrgentOrders(int count = 5)
        {
            var now = DateTime.UtcNow;

            return await _context.Orders
                .Where(o => o.IsCurrent) // Only unfinished orders
                .Where(o => o.OrderCompleteDate > now) // Not yet past due
                .OrderBy(o => o.OrderCompleteDate) // Orders with closest dates first
                .Take(count) // Limit results
                .Select(o => new UrgentOrderDto
                {
                    OrderId = o.Id,
                    ClientName = o.Client.Name,
                    CompletionDate = o.OrderCompleteDate,
                    TimeUntilDue = o.OrderCompleteDate - now,
                    ItemsCount = o.OrderItems.Count,
                    TotalPrice = o.TotalPrice
                })
                .ToListAsync();
        }
        public async Task<OrderProfitDto> CalculateOrderProfit(int orderId)
        {
            // Get the order with all related data
            var order = await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item)
                        .ThenInclude(i => i.ItemFlowers)
                            .ThenInclude(itemf => itemf.Flower)
                        .ThenInclude(f => f.FlowerIngredients)
                            .ThenInclude(fi => fi.Ingredient)
        .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Item)
                .ThenInclude(i => i.Box)
        .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new KeyNotFoundException("Order not found");

            var result = new OrderProfitDto
            {
                OrderId = orderId,
                TotalSellingPrice = order.TotalPrice
            };

            // Calculate total actual cost
            foreach (var orderItem in order.OrderItems)
            {
                var item = orderItem.Item;
                decimal itemCost = 0;

                // Calculate flowers cost
                foreach (var itemFlower in item.ItemFlowers)
                {
                    var flower = itemFlower.Flower;
                    itemCost += flower.CostPerUnit * itemFlower.Quantity * orderItem.Quantity;

                    // Calculate ingredients cost for each flower
                    foreach (var flowerIngredient in flower.FlowerIngredients)
                    {
                        itemCost += flowerIngredient.Ingredient.CostPerUnit *
                                   flowerIngredient.QuantityRequired *
                                   itemFlower.Quantity *
                                   orderItem.Quantity;
                    }
                }

                if (item.Box != null)
                {
                    itemCost += item.Box.CostPerUnit * orderItem.Quantity;
                }

                result.TotalActualCost += itemCost;
            }

            decimal discountRate = order.Client?.DiscountLevel / 100m ?? 0;
            result.DiscountAmount = result.TotalSellingPrice * discountRate;

            result.ProfitBeforeDiscount = result.TotalSellingPrice - result.TotalActualCost;
            result.FinalProfit = result.ProfitBeforeDiscount - result.DiscountAmount;
            result.FinalProfit = result.FinalProfit * Convert.ToDecimal(0.7);
            result.ProfitMargin = result.TotalActualCost > 0 ?
                (result.FinalProfit / result.TotalActualCost) * 100 : 0;

            return result;
        }
    }
}
