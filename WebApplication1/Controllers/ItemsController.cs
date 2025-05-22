using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class ItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ItemsService _itemsService;

        public ItemsController(ApplicationDbContext context, ItemsService itemsService)
        {
            _context = context;
            _itemsService = itemsService;
        }

        // GET: api/Items
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemResponseDto>>> GetItems()
        {
            return await _itemsService.GetItems();    
        }

        // GET: api/Items/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemResponseDto>> GetItem(int id)
        {
            try
            {
                return await _itemsService.GetItem(id);
            }
            catch (DivideByZeroException)
            {
                return BadRequest("Item not found");
            }
            catch
            {
                return Problem();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutItem(int id, ItemUpdateDto itemDto)
        {
            try
            {
                await PutItem(id, itemDto);
                return Ok();
            }
            catch (DivideByZeroException)
            {
                return BadRequest("Item not found");
            }
            catch
            {
                return Problem();
            }
        }

        // POST: api/Items
        [HttpPost]
        public async Task<ActionResult<Item>> PostItem(ItemCreateDto itemDto)
        {
            try
            {
                return await _itemsService.PostItem(itemDto);
            }
            catch (DivideByZeroException)
            {
                return BadRequest("Item not found");
            }
            catch
            {
                return Problem();
            }
        }

        // DELETE: api/Items/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            try
            {
                await _itemsService.DeleteItem(id);
                return Ok();
            }
            catch (DivideByZeroException)
            {
                return BadRequest("Item not found");
            }
            catch
            {
                return Problem();
            }
        }

    }

    // DTO classes
}