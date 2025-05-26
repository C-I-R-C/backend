using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class BoxesController : ControllerBase
    {
        private readonly BoxService _boxService;
        public BoxesController(BoxService boxService)
        {
            _boxService = boxService;
        }
        [HttpGet]
        public async Task<IEnumerable<Box>> GetBoxes()
        {
            return await _boxService.GetBoxes();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Box>> GetBox(int id)
        {
            try
            {
                return await _boxService.GetBox(id);
            }
            catch (DivideByZeroException)
            {
                return BadRequest("No such box");
            }
            catch
            {
                return Problem();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBox(int id, Box box)
        {
            try
            {
                await _boxService.PutBox(id, box);
                return Ok();
            }
            catch (DivideByZeroException)
            {
                return BadRequest("No such box");
            }
            catch (ArgumentException)
            {
                return BadRequest("Box id != id");
            }
            catch
            {
                return Problem();
            }
        }

        [HttpPost]
        public async Task <ActionResult<BoxDto>> Create([FromBody] BoxCreateDto dto)
        {
            return await _boxService.Create(dto);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBox(int id)
        {
            try
            {
                await _boxService.DeleteBox(id);
                return Ok();
            }
            catch (DivideByZeroException)
            {
                return BadRequest("No such box");
            }
            catch
            {
                return Problem();
            }
        }
        [HttpPatch("{id}/stock")]
        public async Task<ActionResult<BoxDto>> UpdateBoxStock(
        int id,
        [FromBody] BoxStockUpdateDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _boxService.UpdateBoxStock(id, updateDto);
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
            catch (Exception)
            {
                return StatusCode(500, "Error updating box stock");
            }
        }
        
    }
}
