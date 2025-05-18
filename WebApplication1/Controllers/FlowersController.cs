using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class FlowersController : ControllerBase
    {
        private readonly ApplicationDbContext _data;
        private readonly FlowersService _flowersService;
        public FlowersController(ApplicationDbContext context, FlowersService flowersService)
        {
            _data = context;
            _flowersService = flowersService;
        }

        // GET api/flowers
        [HttpGet]
        public async Task <ActionResult<IEnumerable<FlowerDto>>> GetAll()
        {
            return await _flowersService.GetAll();
        }

        // GET api/flowers/5
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

        // POST api/flowers
        [HttpPost]
        public async Task <ActionResult<FlowerDto>> Create([FromBody] FlowerCreateDto flowerDto)
        {
            return await _flowersService.Create(flowerDto);
        }

        // PUT api/flowers/5
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

        // DELETE api/flowers/5
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
    }
}
