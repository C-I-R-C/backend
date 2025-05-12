using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlowerIngredients1Controller : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FlowerIngredients1Controller(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/FlowerIngredients1
        [HttpGet]
        public async Task <ActionResult<IEnumerable<FlowerIngredientDto>>> GetIngredientsForFlower(int flowerId)
        {
            if (!_context.Flowers.Any(f => f.Id == flowerId))
                return NotFound("Flower not found");

            return Ok(_context.FlowerIngredients
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
                }));
        }

        // GET: api/FlowerIngredients1/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FlowerIngredient>> GetFlowerIngredient(int id)
        {
            var flowerIngredient = await _context.FlowerIngredients.FindAsync(id);

            if (flowerIngredient == null)
            {
                return NotFound();
            }

            return flowerIngredient;
        }

        // PUT: api/FlowerIngredients1/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlowerIngredient(int id, FlowerIngredient flowerIngredient)
        {
            if (id != flowerIngredient.FlowerId)
            {
                return BadRequest();
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
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/FlowerIngredients1
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task <ActionResult<FlowerIngredientDto>> AddIngredientToFlower(
            int flowerId, [FromBody] AddIngredientToFlowerDto dto)
        {
            if (!_context.Flowers.Any(f => f.Id == flowerId))
                return NotFound("Flower not found");

            if (!_context.Ingredients.Any(i => i.Id == dto.IngredientId))
                return NotFound("Ingredient not found");

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
            return CreatedAtAction(
                nameof(GetIngredientsForFlower),
                new { flowerId },
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
                });
        }
        // DELETE: api/FlowerIngredients1/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlowerIngredient(int id)
        {
            var flowerIngredient = await _context.FlowerIngredients.FindAsync(id);
            if (flowerIngredient == null)
            {
                return NotFound();
            }

            _context.FlowerIngredients.Remove(flowerIngredient);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FlowerIngredientExists(int id)
        {
            return _context.FlowerIngredients.Any(e => e.FlowerId == id);
        }
    }
    public class AddIngredientToFlowerDto
    {
        [Range(1, int.MaxValue)]
        public int IngredientId { get; set; }

        [Range(1, 100)]
        public int QuantityRequired { get; set; }
    }
}
