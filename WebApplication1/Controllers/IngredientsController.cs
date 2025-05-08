using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientsController : ControllerBase
    {
        private readonly DataService _data;

        public IngredientsController(DataService data)
        {
            _data = data;
        }

        // GET api/ingredients
        [HttpGet]
        public ActionResult<IEnumerable<IngredientDto>> GetAll()
        {
            return Ok(_data.Ingredients.Select(i => new IngredientDto
            {
                Id = i.Id,
                Name = i.Name,
                InStock = i.InStock,
                CostPerUnit = i.CostPerUnit
            }));
        }

        // GET api/ingredients/5
        [HttpGet("{id}")]
        public ActionResult<IngredientDto> GetById(int id)
        {
            var ingredient = _data.Ingredients.FirstOrDefault(i => i.Id == id);
            if (ingredient == null) return NotFound();

            return Ok(new IngredientDto
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                InStock = ingredient.InStock,
                CostPerUnit = ingredient.CostPerUnit
            });
        }

        // POST api/ingredients
        [HttpPost]
        public ActionResult<IngredientDto> Create([FromBody] IngredientCreateDto dto)
        {
            var ingredient = new Ingredient
            {
                Id = _data.NextIngredientId,
                Name = dto.Name,
                InStock = dto.InStock,
                CostPerUnit = dto.CostPerUnit
            };

            _data.Ingredients.Add(ingredient);
            return CreatedAtAction(nameof(GetById), new { id = ingredient.Id },
                new IngredientDto
                {
                    Id = ingredient.Id,
                    Name = ingredient.Name,
                    InStock = ingredient.InStock,
                    CostPerUnit = ingredient.CostPerUnit
                });
        }

        // PUT api/ingredients/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] IngredientCreateDto dto)
        {
            var ingredient = _data.Ingredients.FirstOrDefault(i => i.Id == id);
            if (ingredient == null) return NotFound();

            ingredient.Name = dto.Name;
            ingredient.InStock = dto.InStock;
            ingredient.CostPerUnit = dto.CostPerUnit;

            return NoContent();
        }

        // DELETE api/ingredients/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var ingredient = _data.Ingredients.FirstOrDefault(i => i.Id == id);
            if (ingredient == null) return NotFound();

            if (_data.FlowerIngredients.Any(fi => fi.IngredientId == id))
            {
                return BadRequest("Cannot delete ingredient used in flower recipes");
            }

            _data.Ingredients.Remove(ingredient);
            return NoContent();
        }
    }
}
