using System.Security.AccessControl;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class ItemFlowersService
    {
        private readonly ApplicationDbContext _data;

        public ItemFlowersService(ApplicationDbContext context)
        {
            _data = context;
        }

        public async Task<PagedResult<ItemFlowerDto>> GetFlowersForItem(
    int itemId,
    PaginationParameters parameters)
        {
            // Используем AnyAsync для асинхронной проверки
            bool itemExists = await _data.Items.AnyAsync(i => i.Id == itemId);
            if (!itemExists)
            {
                throw new DivideByZeroException($"Item with ID {itemId} not found");
            }

            var query = from itemf in _data.ItemFlowers
                        join f in _data.Flowers on itemf.FlowerId equals f.Id
                        join c in _data.Colors on f.ColorId equals c.Id into flowerColor
                        from color in flowerColor.DefaultIfEmpty()
                        where itemf.ItemId == itemId
                        select new ItemFlowerDto
                        {
                            ItemId = itemf.ItemId,
                            FlowerId = itemf.FlowerId,
                            Quantity = itemf.Quantity,
                            Flower = new FlowerDto
                            {
                                Id = f.Id,
                                Name = f.Name,
                                InStock = f.InStock,
                                CostPerUnit = f.CostPerUnit,
                                Color = color != null ? new ColorDto
                                {
                                    Id = color.Id,
                                    Name = color.Name,
                                    IsNatural = color.IsNatural
                                } : null
                            }
                        };
            query = query.OrderBy(dto => dto.Flower.Name);

            var totalCount = await query.CountAsync();

            var result = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return new PagedResult<ItemFlowerDto>
            {
                Items = result,
                TotalCount = totalCount,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize
            };
        }
        public async Task<ItemFlowerDto> AddFlowerToItem(int itemId, [FromBody] AddFlowerToItemDto dto)
        {
            if (!_data.Items.Any(i => i.Id == itemId))
                throw new DivideByZeroException();

            if (!_data.Flowers.Any(f => f.Id == dto.FlowerId))
                throw new DivideByZeroException();

            var existing = _data.ItemFlowers
                .FirstOrDefault(itemf => itemf.ItemId == itemId && itemf.FlowerId == dto.FlowerId);

            if (existing != null)
            {
                existing.Quantity += dto.Quantity;
            }
            else
            {
                _data.ItemFlowers.Add(new ItemFlower
                {
                    ItemId = itemId,
                    FlowerId = dto.FlowerId,
                    Quantity = dto.Quantity
                });
            }

            var flower = _data.Flowers.First(f => f.Id == dto.FlowerId);

            await _data.SaveChangesAsync();
            return
                new ItemFlowerDto
                {
                    ItemId = itemId,
                    FlowerId = dto.FlowerId,
                    Quantity = dto.Quantity,
                    Flower = new FlowerDto
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
                        } : null
                    }
                };
        }

        public async Task UpdateFlowerQuantity(int itemId, int flowerId, [FromBody] UpdateFlowerQuantityDto dto)
        {
            var itemFlower = _data.ItemFlowers
                .FirstOrDefault(itemf => itemf.ItemId == itemId && itemf.FlowerId == flowerId);

            if (itemFlower == null)
                throw new DivideByZeroException();

            itemFlower.Quantity = dto.Quantity;
            await _data.SaveChangesAsync();
        }

        public async Task RemoveFlowerFromItem(int itemId, int flowerId)
        {
            using var transaction = await _data.Database.BeginTransactionAsync();

            try
            {
                var itemFlower = await _data.ItemFlowers
                    .FirstOrDefaultAsync(itemf => itemf.ItemId == itemId && itemf.FlowerId == flowerId);

                if (itemFlower == null)
                {
                    throw new KeyNotFoundException($"Flower {flowerId} not found in item {itemId}");
                }

                _data.ItemFlowers.Remove(itemFlower);
                await _data.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw; 
            }
        }
    }
}
