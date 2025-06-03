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
        public async Task<bool> UpdateItemAsync(ItemUpdateDto itemDto)
        {
            var item = await _context.Items.FindAsync(itemDto.Id);
            if (item == null) return false;

            item.Name = itemDto.Name;
            item.BasePrice = itemDto.BasePrice;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<Item> PostItem(ItemCreateDto itemDto)
        {
            if (!await _context.Boxes.AnyAsync(b => b.Id == itemDto.BoxId))
            {
                throw new DivideByZeroException();
            }

            var item = new Item
            {
                Name = itemDto.Name,
                BasePrice = itemDto.BasePrice,
                BoxId = itemDto.BoxId

            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();


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

            var isUsedInOrders = await _context.OrderItems.AnyAsync(oi => oi.ItemId == id);
            if (isUsedInOrders)
            {
                throw new AbandonedMutexException();
            }

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

        }
        public async Task<ItemCostAnalysisDto> CalculateItemCostAnalysis(int itemId)
        {
            var item = await _context.Items
                .Include(i => i.ItemFlowers)
                    .ThenInclude(itemf => itemf.Flower)
                .ThenInclude(f => f.FlowerIngredients)
                    .ThenInclude(fi => fi.Ingredient)
        .Include(i => i.Box)
        .FirstOrDefaultAsync(i => i.Id == itemId);

            if (item == null)
                throw new ArgumentException("Item not found");

            var result = new ItemCostAnalysisDto
            {
                BasePrice = item.BasePrice
            };

            foreach (var itemFlower in item.ItemFlowers)
            {
                var flower = itemFlower.Flower;
                result.FlowersCost += flower.CostPerUnit * itemFlower.Quantity;
                foreach (var flowerIngredient in flower.FlowerIngredients)
                {
                    result.IngredientsCost += flowerIngredient.Ingredient.CostPerUnit *
                                             flowerIngredient.QuantityRequired *
                                             itemFlower.Quantity;
                }
            }
            result.BoxCost = item.Box?.CostPerUnit ?? 0;

            result.TotalComponentsCost = result.FlowersCost + result.BoxCost;
            result.LaborCost = result.FlowersCost - result.IngredientsCost;
            
            result.Profit = item.BasePrice - result.TotalComponentsCost;
            result.Profit = Convert.ToDecimal(0.7) * result.Profit;
            result.ProfitMargin = result.TotalComponentsCost > 0 ?
                                 (result.Profit / result.TotalComponentsCost) : 0;

            return result;
        }
    }
}
