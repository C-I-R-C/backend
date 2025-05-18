using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class FlowerIngredientsService
    {
        private readonly ApplicationDbContext _context;

        public FlowerIngredientsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<FlowerIngredientDto>> GetIngredientsForFlower(int flowerId)
        {
            if (!_context.Flowers.Any(f => f.Id == flowerId))
                throw new DivideByZeroException();

            return _context.FlowerIngredients
                .Where(fi => fi.FlowerId == flowerId)
                .Select(fi => new FlowerIngredientDto
                {
                    FlowerId = fi.FlowerId,
                    IngredientId = fi.IngredientId,
                    QuantityRequired = fi.QuantityRequired,
                    Ingredient = _context.Ingredients
                        .Where(i => i.Id == fi.IngredientId)
                        .Select(i => new IngredientDto
                        {
                            Id = i.Id,
                            Name = i.Name,
                            InStock = i.InStock,
                            CostPerUnit = i.CostPerUnit
                        })
                        .FirstOrDefault()
                }).ToList();
        }

        public async Task<ActionResult<FlowerIngredient>> GetFlowerIngredient(int id)
        {
            var flowerIngredient = await _context.FlowerIngredients.FindAsync(id);

            if (flowerIngredient == null)
            {
                throw new DivideByZeroException();
            }

            return flowerIngredient;
        }

        public async Task PutFlowerIngredient(int id, FlowerIngredient flowerIngredient)
        {
            if (id != flowerIngredient.FlowerId)
            {
                throw new BadImageFormatException();
            }

            _context.Entry(flowerIngredient).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlowerIngredientExists(id))
                {
                    throw new DivideByZeroException();
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<ActionResult<FlowerIngredientDto>> AddIngredientToFlower(
            int flowerId, [FromBody] AddIngredientToFlowerDto dto)
        {
            if (!_context.Flowers.Any(f => f.Id == flowerId))
                throw new DivideByZeroException();

            if (!_context.Ingredients.Any(i => i.Id == dto.IngredientId))
                throw new DivideByZeroException();

            var existing = _context.FlowerIngredients
                .FirstOrDefault(fi => fi.FlowerId == flowerId && fi.IngredientId == dto.IngredientId);

            if (existing != null)
            {
                existing.QuantityRequired = dto.QuantityRequired;
            }
            else
            {
                _context.FlowerIngredients.Add(new FlowerIngredient
                {
                    FlowerId = flowerId,
                    IngredientId = dto.IngredientId,
                    QuantityRequired = dto.QuantityRequired
                });
            }

            var ingredient = _context.Ingredients.First(i => i.Id == dto.IngredientId);
            await _context.SaveChangesAsync();
            return 
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
                };
        }

        public async Task DeleteFlowerIngredient(int id)
        {
            var flowerIngredient = await _context.FlowerIngredients.FindAsync(id);
            if (flowerIngredient == null)
            {
                throw new DivideByZeroException();
            }

            _context.FlowerIngredients.Remove(flowerIngredient);
            await _context.SaveChangesAsync();
        }

        private bool FlowerIngredientExists(int id)
        {
            return _context.FlowerIngredients.Any(e => e.FlowerId == id);
        }
    }
    
}

