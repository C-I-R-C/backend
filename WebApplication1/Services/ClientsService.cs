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


        public async Task<PagedResult<ClientResponseDto>> 
            GetClients(
                PaginationParameters parameters )

        {

            var query = from client in _context.Clients
                        join order in _context.Orders
                            on client.Id equals order.ClientId into clientOrders
                        from co in clientOrders.DefaultIfEmpty()
                        group co by new
                        {
                            client.Id,
                            client.Name,
                            client.PhoneNumber,
                            client.TotalOrdersCount,
                            client.DiscountLevel
                        } into g
                        select new ClientResponseDto
                        {
                            Id = g.Key.Id,
                            Name = g.Key.Name,
                            PhoneNumber = g.Key.PhoneNumber,
                            TotalOrdersCount = g.Key.TotalOrdersCount,
                            DiscountLevel = g.Key.DiscountLevel,
                            TotalSpent = g.Sum(o => o != null ? o.TotalPrice : 0),
                            LastOrderDate = g.Max(o => o != null ? o.OrderDate : (DateTime?)null)
                        };


            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(c => c.Name) // или другой порядок
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();


            return new PagedResult<ClientResponseDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize
            };

        }


        public async Task<ClientResponseDto> 
            GetClient(
                int id )

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
        
        public async Task<ClientResponseDto> 
            Create(
                [FromBody] ClientCreateDto clientDto )

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


        public async Task 
            DeleteClient(
                int id )

        {

            var client = await _context.Clients.FindAsync(id);


            if (client == null)
            {

                throw new DivideByZeroException();

            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

        }


        public async Task 
            PutClient(
                int id, 
                [FromBody] ClientUpdateDto clientDto )

        {


            if (id != clientDto.Id)
            {

                throw new ArgumentException();
            
            }


            var client = await _context.Clients.FindAsync(id) ?? throw new DivideByZeroException();


            if (!string.IsNullOrWhiteSpace(clientDto.Name))
            {
             
                client.Name = clientDto.Name.Trim();
            
            }


            if (!string.IsNullOrWhiteSpace(clientDto.PhoneNumber))
            {
             
                client.PhoneNumber = clientDto.PhoneNumber.Trim();

            }


            if (clientDto.DiscountLevel.HasValue)
            {

                if (clientDto.DiscountLevel < 0 || clientDto.DiscountLevel > 100)
                {
                 
                    throw new AbandonedMutexException();
                
                }


                client.DiscountLevel = clientDto.DiscountLevel.Value;
            
            }

            await _context.SaveChangesAsync();

        }


        private OrderResponseDto 
            MapToOrderResponseDto(
                Order order)

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

                    Flowers = [.. _context.ItemFlowers
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

                    })]
                }).ToList() ?? []

            };

        }


        public async Task<List<ClientWithDetailedOrdersDto>> 
            SearchClients(
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


            return [.. clients.Select(c => new ClientWithDetailedOrdersDto
            {
                Id = c.Id,
                Name = c.Name,
                PhoneNumber = c.PhoneNumber,
                TotalOrdersCount = c.TotalOrdersCount,
                DiscountLevel = c.DiscountLevel,
                Orders = [.. c.Orders
                    .Where(o => onlyCompletedOrders == null ||
                               o.IsCurrent == !onlyCompletedOrders.Value)
                    .Select(o => MapToOrderResponseDto(o))]
            })];

        }


    }



}

