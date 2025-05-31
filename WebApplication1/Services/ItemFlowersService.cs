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

        public async Task<List<ItemFlowerDto>> GetFlowersForItem(int itemId)
        {
            if (!_data.Items.Any(i => i.Id == itemId))
                throw new DivideByZeroException();

            var itemFlowers = _data.ItemFlowers
                .Where(itemf => itemf.ItemId == itemId)
            .Select(itemf => new ItemFlowerDto
            {
                ItemId = itemf.ItemId,
                FlowerId = itemf.FlowerId,
                Quantity = itemf.Quantity,
                Flower = _data.Flowers
                    .Where(f => f.Id == itemf.FlowerId)
                    .Select(f => new FlowerDto
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
                    })
                    .FirstOrDefault()
            });

            return await itemFlowers.ToListAsync();
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
                // Find the relationship
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
