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
        public async Task <ActionResult<IEnumerable<FlowerIngredientDto>>> GetIngredientsForFlower(int flowerId)
        {
            return await _flowerIngredientsService.GetIngredientsForFlower(flowerId);
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
        public async Task <ActionResult<FlowerIngredientDto>> AddIngredientToFlower(
            int flowerId, [FromBody] AddIngredientToFlowerDto dto)
        {
            try
            {
                return await _flowerIngredientsService.AddIngredientToFlower(flowerId, dto);
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
    }
}
