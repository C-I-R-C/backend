using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class IngredientsService
    {
        private readonly ApplicationDbContext _context;

        public IngredientsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task PutIngredient(int id, Ingredient ingredient)
        {
            if (id != ingredient.Id)
            {
                throw new ArgumentException("NoIngre");
            }

            _context.Entry(ingredient).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IngredientExists(id))
                {
                    throw new ArgumentException("NoIngre");
                }
                else
                {
                    throw;
                }
            }

        }
        public async Task<IngredientDto> Create([FromBody] IngredientCreateDto dto)
        {
            var ingredient = new Ingredient
            {
                Name = dto.Name,
                InStock = dto.InStock,
                CostPerUnit = dto.CostPerUnit
            };

            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();
            return
                new IngredientDto
                {
                    Id = ingredient.Id,
                    Name = ingredient.Name,
                    InStock = ingredient.InStock,
                    CostPerUnit = ingredient.CostPerUnit
                };
        }
        private bool IngredientExists(int id)
        {
            return _context.Ingredients.Any(e => e.Id == id);
        }
    }
}
