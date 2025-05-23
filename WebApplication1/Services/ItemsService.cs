using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class ItemsService
    {
        private readonly ApplicationDbContext _context;

        public ItemsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ItemResponseDto>> GetItems()
        {
            var items = await _context.Items
                .Include(i => i.Box)
                .Include(i => i.ItemFlowers)
                    .ThenInclude(itemf => itemf.Flower)
                        .ToListAsync();

            var itemDtos = items.Select(i => new ItemResponseDto
            {
                Id = i.Id,
                Name = i.Name,
                BasePrice = i.BasePrice,
                Box = i.Box != null ? new BoxDto
                {
                    Id = i.Box.Id,
                    Name = i.Box.Name,
                    InStock = i.Box.InStock
                } : null,
                Flowers = i.ItemFlowers.Select(itemf => new ItemFlowerDetailDto
                {
                    FlowerId = itemf.Flower.Id,
                    FlowerName = itemf.Flower.Name,
                    Quantity = itemf.Quantity
                }).ToList()
            }).ToList();

            return itemDtos;
        }
        public async Task<ItemResponseDto> GetItem(int id)
        {
            var item = await _context.Items
                .Include(i => i.Box) 
                .Include(i => i.ItemFlowers) 
                    .ThenInclude(itemf => itemf.Flower) 
                .ThenInclude(f => f.Color) 
        .Include(i => i.ItemFlowers) 
            .ThenInclude(itemf => itemf.Flower)
                .ThenInclude(f => f.FlowerIngredients)
                    .ThenInclude(fi => fi.Ingredient)
        .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
            {
                throw new DivideByZeroException();
            }

            
            var itemDto = new ItemResponseDto
            {
                Id = item.Id,
                Name = item.Name,
                BasePrice = item.BasePrice,
                Box = item.Box != null ? new BoxDto
                {
                    Id = item.Box.Id,
                    Name = item.Box.Name,
                    InStock = item.Box.InStock
                } : null,
                Flowers = item.ItemFlowers.Select(itemf => new ItemFlowerDetailDto
                {
                    FlowerId = itemf.Flower.Id,
                    FlowerName = itemf.Flower.Name,
                    Quantity = itemf.Quantity,
                    UnitCost = itemf.Flower.CostPerUnit,
                    Color = itemf.Flower.Color?.Name ?? "N/A",
                    Ingredients = itemf.Flower.FlowerIngredients.Select(fi => new FlowerIngredient1Dto
                    {
                        IngredientId = fi.IngredientId,
                        Name = fi.Ingredient.Name,
                        QuantityRequired = fi.QuantityRequired,
                        CostPerUnit = fi.Ingredient.CostPerUnit
                    }).ToList()
                }).ToList()
            };

            return itemDto;
        }
        public async Task PutItem(int id, ItemUpdateDto itemDto)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                throw new DivideByZeroException();
            }

            item.Name = itemDto.Name;
            item.BasePrice = itemDto.BasePrice;

            _context.Entry(item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemExists(id))
                {
                    throw new DivideByZeroException();
                }
                else
                {
                    throw;
                }
            }

            
        }
        public async Task<Item> PostItem(ItemCreateDto itemDto)
        {
            // Check if box exists if BoxId is provided
            if (itemDto.BoxId != null && !await _context.Boxes.AnyAsync(b => b.Id == itemDto.BoxId))
            {
                throw new DivideByZeroException();
            }

            var item = new Item
            {
                Name = itemDto.Name,
                BasePrice = itemDto.BasePrice,
                BoxId = itemDto.BoxId // Only set the foreign key

            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            // Load the box if you need it in the response
            if (item.BoxId.HasValue)
            {
                await _context.Entry(item)
                    .Reference(i => i.Box)
                    .LoadAsync();
            }

            return item;
        }
        public async Task DeleteItem(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                throw new DivideByZeroException();
            }

            // Check if item is used in any orders
            var isUsedInOrders = await _context.OrderItems.AnyAsync(oi => oi.ItemId == id);
            if (isUsedInOrders)
            {
                throw new AbandonedMutexException();
            }

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

        }
        private bool ItemExists(int id)
        {
            return _context.Items.Any(e => e.Id == id);
        }

    }
}
