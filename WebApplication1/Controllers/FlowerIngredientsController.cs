using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class FlowerIngredientsController : ControllerBase
    {
        private readonly FlowerIngredientsService _flowerIngredientsService;

        public FlowerIngredientsController(FlowerIngredientsService flowerIngredientsService)
        {
            _flowerIngredientsService = flowerIngredientsService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<FlowerIngredientDto>>> GetIngredientsForFlower(int flowerId, [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var parameters = new PaginationParameters
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            try
            {
                var result = await _flowerIngredientsService.GetIngredientsForFlower(flowerId, parameters);
                return Ok(result);
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
            if (dto == null)
                return BadRequest("Ingredient data is required");
            try
            {
                return await _flowerIngredientsService.AddIngredientToFlower(flowerId, dto);
            }
            catch (DivideByZeroException)
            {
                return BadRequest("Ingredient or flowert doesn't exist");
            }
            catch (ArgumentException)
            {
                return BadRequest("Quantity must be greater than 0");
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
