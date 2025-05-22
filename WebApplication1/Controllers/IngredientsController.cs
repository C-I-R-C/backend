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

        // GET: api/Ingredients1
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ingredient>>> GetIngredients()
        {
            return await _context.Ingredients.ToListAsync();
        }

        // GET: api/Ingredients1/5
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

        // PUT: api/Ingredients1/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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

        // POST: api/Ingredients1
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task <ActionResult<IngredientDto>> Create([FromBody] IngredientCreateDto dto)
        {
            return await _ingredientsService.Create(dto);
        }


        // DELETE: api/Ingredients1/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIngredient(int id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }

            _context.Ingredients.Remove(ingredient);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpGet("low-stock")]
        public async Task<ActionResult<List<IngredientStockDto>>> GetLowStockIngredients(
    [FromQuery] int count = 5)
        {
            try
            {
                // Validate input
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
