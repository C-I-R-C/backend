using System;
using System.Collections.Generic;
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
    public class ItemFlowersController : ControllerBase
    {
        private readonly ApplicationDbContext _data;
        private readonly ItemFlowersService _itemFlowersService;
        public ItemFlowersController(ApplicationDbContext context, ItemFlowersService itemFlowersService)
        {
            _data = context;
            _itemFlowersService = itemFlowersService;
        }

        // GET api/items/5/flowers
        [HttpGet]
        public async Task <ActionResult<IEnumerable<ItemFlowerDto>>> GetFlowersForItem(int itemId)
        {
            return await _itemFlowersService.GetFlowersForItem(itemId);
        }

        // POST api/items/5/flowers
        [HttpPost]
        public async Task <ActionResult<ItemFlowerDto>> AddFlowerToItem(int itemId, [FromBody] AddFlowerToItemDto dto)
        {
            try
            {
                return await _itemFlowersService.AddFlowerToItem(itemId, dto);
            }
            catch (DivideByZeroException)
            {
                return BadRequest("Item or flower not found");
            }
            catch
            {
                return Problem();
            }
        }

        // PUT api/items/5/flowers/3 (update quantity)
        [HttpPut("{flowerId}")]
        public async Task <IActionResult> UpdateFlowerQuantity(int itemId, int flowerId, [FromBody] UpdateFlowerQuantityDto dto)
        {
            try
            {
                await UpdateFlowerQuantity(itemId, flowerId, dto);
                return Ok();
            }
            catch (DivideByZeroException)
            {
                return BadRequest("Item or flower not found");
            }
            catch
            {
                return Problem();
            }
        }

        // DELETE api/items/5/flowers/3
        [HttpDelete("{itemId}/flowers/{flowerId}")]
        public async Task<IActionResult> RemoveFlowerFromItem(int itemId, int flowerId)
        {
            try
            {
                await _itemFlowersService.RemoveFlowerFromItem(itemId, flowerId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error removing flower from item");
            }
        }
    }
}
