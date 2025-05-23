using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
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
    public class FlowerIngredientsController : ControllerBase
    {
        private readonly FlowerIngredientsService _flowerIngredientsService;
        private readonly ApplicationDbContext _context;

        public FlowerIngredientsController(ApplicationDbContext context, FlowerIngredientsService flowerIngredientsService)
        {
            _context = context;
            _flowerIngredientsService = flowerIngredientsService;
        }

        // GET: api/FlowerIngredients1
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlowerIngredientDto>>> GetIngredientsForFlower(int flowerId)
        {
            try
            {
                return await _flowerIngredientsService.GetIngredientsForFlower(flowerId);
            }
            catch (DivideByZeroException)
            {
                return NotFound("flower not found");
            }
            catch
            {
                return Problem();
            }
        }

        // GET: api/FlowerIngredients1/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FlowerIngredient>> GetFlowerIngredient(int id)
        {
            try
            {
                return await _flowerIngredientsService.GetFlowerIngredient(id);
            }
            catch (DivideByZeroException)
            {
                return NotFound();
            }
            catch
            {
                return Problem();
            }
        }

        // PUT: api/FlowerIngredients1/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlowerIngredient(int id, FlowerIngredient flowerIngredient)
        {
            try
            {
                await _flowerIngredientsService.PutFlowerIngredient(id, flowerIngredient);
                return Ok();
            }
            catch (DivideByZeroException)
            {
                return NotFound();
            }
            catch (BadImageFormatException)
            {
                return NotFound("No such flower");
            }
            catch
            {
                return Problem();
            }
        }
        [HttpPost]
        public async Task<ActionResult<FlowerIngredientDto>> AddIngredientToFlower(
    int flowerId, [FromBody] AddIngredientToFlowerDto dto)
        {
            // Validate input
            if (dto == null)
                return BadRequest("Ingredient data is required");

            if (dto.QuantityRequired <= 0)
                return BadRequest("Quantity must be greater than zero");

            // Check if flower exists
            var flower = await _context.Flowers
                .Include(f => f.FlowerIngredients)
                .ThenInclude(fi => fi.Ingredient)
                .FirstOrDefaultAsync(f => f.Id == flowerId);

            if (flower == null)
                return NotFound($"Flower with ID {flowerId} not found");

            // Check if ingredient exists
            var ingredient = await _context.Ingredients
                .FirstOrDefaultAsync(i => i.Id == dto.IngredientId);

            if (ingredient == null)
                return NotFound($"Ingredient with ID {dto.IngredientId} not found");

            try
            {
                // Find existing relationship or create new
                var existing = flower.FlowerIngredients
                    .FirstOrDefault(fi => fi.IngredientId == dto.IngredientId);

                decimal costChange = 0;

                if (existing != null)
                {
                    // Calculate cost difference for existing ingredient
                    costChange = ingredient.CostPerUnit *
                                (dto.QuantityRequired - existing.QuantityRequired);
                    existing.QuantityRequired = dto.QuantityRequired;
                }
                else
                {
                    // Calculate cost for new ingredient
                    costChange = ingredient.CostPerUnit * dto.QuantityRequired;

                    var newFlowerIngredient = new FlowerIngredient
                    {
                        FlowerId = flowerId,
                        IngredientId = dto.IngredientId,
                        QuantityRequired = dto.QuantityRequired
                    };

                    _context.FlowerIngredients.Add(newFlowerIngredient);
                    flower.FlowerIngredients.Add(newFlowerIngredient);
                }

                // Update flower cost
                flower.CostPerUnit += costChange;

                await _context.SaveChangesAsync();

                // Return the DTO
                return Ok(new FlowerIngredientDto
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
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlowerIngredient(int id)
        {
            try
            {
                await _flowerIngredientsService.DeleteFlowerIngredient(id);
                return Ok();
            }
            catch (DivideByZeroException)
            {
                return NotFound();
            }
            catch (BadImageFormatException)
            {
                return NotFound("No such flower");
            }
            catch
            {
                return Problem();
            }
        }
        [HttpDelete("{flowerId}/ingredients/{ingredientId}")]
        public async Task<IActionResult> RemoveIngredientFromFlower(
    int flowerId,
    int ingredientId)
        { 
            try
            {
                await _flowerIngredientsService.RemoveIngredientFromFlower(flowerId, ingredientId);
                return Ok();
            }
            catch (DivideByZeroException)
            {
                return NotFound();
            }
            catch (BadImageFormatException)
            {
                return NotFound("No such flower");
            }
            catch
            {
                return Problem();
            }
        }
    }
}
