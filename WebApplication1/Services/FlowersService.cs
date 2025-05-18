using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class FlowersService
    {
        private readonly ApplicationDbContext _data;

        public FlowersService(ApplicationDbContext context)
        {
            _data = context;
        }

        public async Task<List<FlowerDto>> GetAll()
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

            return flowers.ToList();
        }
        public async Task<FlowerWithIngredientsDto> GetById(int id)
        {
            var flower = _data.Flowers.FirstOrDefault(f => f.Id == id);
            if (flower == null)
            {
                throw new DivideByZeroException();
            }

            return new FlowerWithIngredientsDto
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
            };
        }
        public async Task<FlowerDto> Create([FromBody] FlowerCreateDto flowerDto)
        {
            var flower = new Flower
            {
                Name = flowerDto.Name,
                InStock = flowerDto.InStock,
                CostPerUnit = flowerDto.CostPerUnit,
                ColorId = flowerDto.ColorId
            };

            _data.Flowers.Add(flower);
            await _data.SaveChangesAsync();
            return new FlowerDto
                {
                    Id = flower.Id,
                    Name = flower.Name,
                    InStock = flower.InStock,
                    CostPerUnit = flower.CostPerUnit
                };
        }
        public async Task Update(int id, [FromBody] FlowerUpdateDto flowerDto)
        {
            var flower = _data.Flowers.FirstOrDefault(f => f.Id == id);
            if (flower == null)
            {
                throw new DivideByZeroException();
            }
            flower.Name = flowerDto.Name;
            flower.InStock = flowerDto.InStock;
            flower.CostPerUnit = flowerDto.CostPerUnit;
            flower.ColorId = flowerDto.ColorId;
            await _data.SaveChangesAsync();
        }
        public async Task Delete(int id)
        {
            var flower = _data.Flowers.FirstOrDefault(f => f.Id == id);
            if (flower == null){
            throw new DivideByZeroException();
            }

            // Check if flower is used in any items
            if (_data.ItemFlowers.Any(itemf => itemf.FlowerId == id))
            {
                throw new BadImageFormatException();
            }

            _data.Flowers.Remove(flower);
            await _data.SaveChangesAsync();
        }
    }
}
