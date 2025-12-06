using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FlowersController : ControllerBase
    {
        private readonly FlowersService _flowersService;
        public FlowersController(FlowersService flowersService)
        {
            _flowersService = flowersService;
        }

        [HttpGet]
        public async Task <ActionResult<PagedResult<FlowerDto>>> GetAll([FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var parameters = new PaginationParameters
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var result = await _flowersService.GetAll(parameters);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async  Task <ActionResult<FlowerWithIngredientsDto>> GetById(int id)
        {
            try
            {
                return await _flowersService.GetById(id);
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

        [HttpPost]
        public async Task <ActionResult<FlowerDto>> Create([FromBody] FlowerCreateDto flowerDto)
        {
            return await _flowersService.Create(flowerDto);
        }

        [HttpPut("{id}")]
        public async Task <IActionResult> Update(int id, [FromBody] FlowerUpdateDto flowerDto)
        {
            try
            {
                await _flowersService.Update(id, flowerDto);
                return NoContent();
            }
            catch (DivideByZeroException)
            {
                return BadRequest("FlowerNotFound");
            }
            catch
            {
                return Problem();
            }
        }

        [HttpDelete("{id}")]
        public async Task <IActionResult> Delete(int id)
        {
            try
            {
                await _flowersService.Delete(id);
                return NoContent();
            }
            catch (DivideByZeroException)
            {
                return BadRequest("FlowerNotFound");
            }
            catch (BadImageFormatException)
            {
                return BadRequest("Cannot delete flower used in order");
            }
            catch
            {
                return Problem();
            }
        }

        [HttpPatch("{id}/quantity")]
        public async Task<ActionResult<FlowerDto>> UpdateQuantity(int id, [FromBody] FlowerQuantityUpdateDto updateDto)
        {
            try
            {
                return await _flowersService.UpdateQuantity(id, updateDto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Flower not found");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return Problem();
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedResult<FlowerDto>>> SearchFlowersByName([FromQuery] string name, [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return BadRequest("Search term cannot be empty");
                }
                var parameters = new PaginationParameters
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
                var result = await _flowersService.GetFlowersByName(name, parameters);


                return Ok(result);
            }
            catch
            {
                return Problem();
            }
        }
    }
}
