using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/flowers/{flowerId}/ingredients")]
    public class FlowerIngredientsController : ControllerBase
    {
        private readonly DataService _data;

        public FlowerIngredientsController(DataService data)
        {
            _data = data;
        }

        // GET api/flowers/1/ingredients
        [HttpGet]
        public ActionResult<IEnumerable<FlowerIngredientDto>> GetIngredientsForFlower(int flowerId)
        {
            if (!_data.Flowers.Any(f => f.Id == flowerId))
                return NotFound("Flower not found");

            return Ok(_data.FlowerIngredients
                .Where(fi => fi.FlowerId == flowerId)
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
                }));
        }

        // POST api/flowers/1/ingredients
        [HttpPost]
        public ActionResult<FlowerIngredientDto> AddIngredientToFlower(
            int flowerId, [FromBody] AddIngredientToFlowerDto dto)
        {
            if (!_data.Flowers.Any(f => f.Id == flowerId))
                return NotFound("Flower not found");

            if (!_data.Ingredients.Any(i => i.Id == dto.IngredientId))
                return NotFound("Ingredient not found");

            var existing = _data.FlowerIngredients
                .FirstOrDefault(fi => fi.FlowerId == flowerId && fi.IngredientId == dto.IngredientId);

            if (existing != null)
            {
                existing.QuantityRequired = dto.QuantityRequired;
            }
            else
            {
                _data.FlowerIngredients.Add(new FlowerIngredient
                {
                    FlowerId = flowerId,
                    IngredientId = dto.IngredientId,
                    QuantityRequired = dto.QuantityRequired
                });
            }

            var ingredient = _data.Ingredients.First(i => i.Id == dto.IngredientId);
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

        // DELETE api/flowers/1/ingredients/2
        [HttpDelete("{ingredientId}")]
        public IActionResult RemoveIngredientFromFlower(int flowerId, int ingredientId)
        {
            var flowerIngredient = _data.FlowerIngredients
                .FirstOrDefault(fi => fi.FlowerId == flowerId && fi.IngredientId == ingredientId);

            if (flowerIngredient == null)
                return NotFound("Ingredient not found in this flower recipe");

            _data.FlowerIngredients.Remove(flowerIngredient);
            return NoContent();
        }
    }

    // Supporting DTOs
    public class AddIngredientToFlowerDto
    {
        [Range(1, int.MaxValue)]
        public int IngredientId { get; set; }

        [Range(1, 100)]
        public int QuantityRequired { get; set; }
    }
}
