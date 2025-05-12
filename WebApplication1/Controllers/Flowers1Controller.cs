using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlowersController : ControllerBase
    {
        private readonly ApplicationDbContext _data;

        public FlowersController(ApplicationDbContext context)
        {
            _data = context;
        }

        // GET api/flowers
        [HttpGet]
        public async Task <ActionResult<IEnumerable<FlowerDto>>> GetAll()
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

            return Ok(flowers);
        }

        // GET api/flowers/5
        [HttpGet("{id}")]
        public async  Task <ActionResult<FlowerWithIngredientsDto>> GetById(int id)
        {
            var flower = _data.Flowers.FirstOrDefault(f => f.Id == id);
            if (flower == null) return NotFound();

            return Ok(new FlowerWithIngredientsDto
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
            });
        }

        // POST api/flowers
        [HttpPost]
        public async Task <ActionResult<FlowerDto>> Create([FromBody] FlowerCreateDto flowerDto)
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
            return CreatedAtAction(nameof(GetById), new { id = flower.Id },
                new FlowerDto
                {
                    Id = flower.Id,
                    Name = flower.Name,
                    InStock = flower.InStock,
                    CostPerUnit = flower.CostPerUnit
                });
        }

        // PUT api/flowers/5
        [HttpPut("{id}")]
        public async Task <IActionResult> Update(int id, [FromBody] FlowerUpdateDto flowerDto)
        {
            var flower = _data.Flowers.FirstOrDefault(f => f.Id == id);
            if (flower == null) return NotFound();

            flower.Name = flowerDto.Name;
            flower.InStock = flowerDto.InStock;
            flower.CostPerUnit = flowerDto.CostPerUnit;
            flower.ColorId = flowerDto.ColorId;
            await _data.SaveChangesAsync();
            return NoContent();
        }

        // DELETE api/flowers/5
        [HttpDelete("{id}")]
        public async Task <IActionResult> Delete(int id)
        {
            var flower = _data.Flowers.FirstOrDefault(f => f.Id == id);
            if (flower == null) return NotFound();

            // Check if flower is used in any items
            if (_data.ItemFlowers.Any(itemf => itemf.FlowerId == id))
            {
                return BadRequest("Cannot delete flower used in items");
            }

            _data.Flowers.Remove(flower);
            await _data.SaveChangesAsync();
            return NoContent();
        }
    }
}
