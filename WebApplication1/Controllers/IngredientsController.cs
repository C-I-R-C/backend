using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IngredientsService _ingredientsService;

        public IngredientsController(ApplicationDbContext context, IngredientsService ingredientsService)
        {
            _context = context;
            _ingredientsService = ingredientsService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ingredient>>> GetIngredients()
        {
            return await _context.Ingredients.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Ingredient>> GetIngredient(int id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);

            if (ingredient == null)
            {
                return NotFound();
            }

            return ingredient;
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIngredient(int id, Ingredient ingredient)
        {
            try
            {
                await _ingredientsService.PutIngredient(id, ingredient);
                return Ok();
            }
            catch (ArgumentException)
            {
                return NoContent();
            }
            catch {
                return Problem();
            }
        }

        [HttpPost]
        public async Task <ActionResult<IngredientDto>> Create([FromBody] IngredientCreateDto dto)
        {
            return await _ingredientsService.Create(dto);
        }


        // DELETE: api/Ingredients1/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIngredient(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Check if ingredient exists
                var ingredient = await _context.Ingredients.FindAsync(id);
                if (ingredient == null)
                {
                    return NotFound($"Ingredient with ID {id} not found");
                }
                var isUsedInFlowers = await _context.FlowerIngredients
                    .AnyAsync(fi => fi.IngredientId == id);

                if (isUsedInFlowers)
                {
                    var flowerNames = await _context.FlowerIngredients
                        .Where(fi => fi.IngredientId == id)
                        .Include(fi => fi.Flower)
                        .Select(fi => fi.Flower.Name)
                        .ToListAsync();

                    return BadRequest(new
                    {
                        Message = "Cannot delete ingredient used in flowers",
                        Flowers = flowerNames,
                        Count = flowerNames.Count
                    });
                }

                // Safe to delete
                _context.Ingredients.Remove(ingredient);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "An error occurred while deleting ingredient");
            }
        }
        [HttpGet("low-stock")]
        public async Task<ActionResult<List<IngredientStockDto>>> GetLowStockIngredients(
    [FromQuery] int count = 5)
        {
            try
            {
                if (count < 1 || count > 50)
                {
                    return BadRequest("Count must be between 1 and 50");
                }

                var lowStockIngredients = await _ingredientsService.GetLowestStockIngredients(count);
                return Ok(lowStockIngredients);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving low stock ingredients");
            }
        }
        [HttpPatch("{id}/stock")]
        public async Task<ActionResult<IngredientDto>> UpdateIngredientStock(
    int id,
    [FromBody] UpdateIngredientStockDto updateDto)
        {
            try
            {
                var result = await _ingredientsService.UpdateIngredientStock(id, updateDto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error updating ingredient stock");
            }
        }
    }
}
