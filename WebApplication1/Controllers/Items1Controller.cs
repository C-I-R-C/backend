using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Items
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemResponseDto>>> GetItems()
        {
            var items = await _context.Items
                .Include(i => i.Box)
                .Include(i => i.ItemFlowers)
                    .ThenInclude(itemf => itemf.Flower)
        .ToListAsync();

            var itemDtos = items.Select(i => new ItemResponseDto
            {
                Id = i.Id,
                Name = i.Name,
                BasePrice = i.BasePrice,
                Box = i.Box != null ? new BoxDto
                {
                    Id = i.Box.Id,
                    Name = i.Box.Name,
                    InStock = i.Box.InStock
                } : null,
                // For list view, you might omit some deep relationships for performance
                Flowers = i.ItemFlowers.Select(itemf => new ItemFlowerDetailDto
                {
                    FlowerId = itemf.Flower.Id,
            FlowerName = itemf.Flower.Name,
            Quantity = itemf.Quantity
         }).ToList()
    }).ToList();

    return Ok(itemDtos);
}

        // GET: api/Items/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemResponseDto>> GetItem(int id)
        {
            var item = await _context.Items
                .Include(i => i.Box) // Include box
                .Include(i => i.ItemFlowers) // Include flowers
                    .ThenInclude(itemf => itemf.Flower) // Then include flower details
                .ThenInclude(f => f.Color) // Then include color
        .Include(i => i.ItemFlowers) // Include flowers again for ingredients
            .ThenInclude(itemf => itemf.Flower)
                .ThenInclude(f => f.FlowerIngredients)
                    .ThenInclude(fi => fi.Ingredient)
        .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            // Map to DTO
            var itemDto = new ItemResponseDto
            {
                Id = item.Id,
                Name = item.Name,
                BasePrice = item.BasePrice,
                Box = item.Box != null ? new BoxDto
                {
                    Id = item.Box.Id,
                    Name = item.Box.Name,
                    InStock = item.Box.InStock
                } : null,
                Flowers = item.ItemFlowers.Select(itemf => new ItemFlowerDetailDto
                {
                    FlowerId = itemf.Flower.Id,
            FlowerName = itemf.Flower.Name,
            Quantity = itemf.Quantity,
            UnitCost = itemf.Flower.CostPerUnit,
            Color = itemf.Flower.Color?.Name ?? "N/A",
            Ingredients = itemf.Flower.FlowerIngredients.Select(fi => new FlowerIngredient1Dto
            {
                IngredientId = fi.IngredientId,
                Name = fi.Ingredient.Name,
                QuantityRequired = fi.QuantityRequired,
                CostPerUnit = fi.Ingredient.CostPerUnit
            }).ToList()
            }).ToList()
    };

    // Calculate production cost

    return Ok(itemDto);
}

//private decimal CalculateProductionCost(ItemResponseDto item)
//{
//    decimal cost = 0m;

//    // Add flower costs
//    foreach (var flower in item.Flowers)
//    {
//        cost += flower.Quantity * flower.UnitCost;

//        // Add ingredient costs
//        foreach (var ingredient in flower.Ingredients)
//        {
//            cost += ingredient.QuantityRequired * ingredient.CostPerUnit * flower.Quantity;
//        }
//    }

//    // Add box cost if exists
//    if (item.Box != null)
//    {
//        cost += item.Box.InStock > 0 ? item.Box.Cost : 0; // Only count if in stock
//    }

//    return cost;
//}

// PUT: api/Items/5
[HttpPut("{id}")]
        public async Task<IActionResult> PutItem(int id, ItemUpdateDto itemDto)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            item.Name = itemDto.Name;
            item.BasePrice = itemDto.BasePrice;

            _context.Entry(item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Items
        [HttpPost]
        public async Task<ActionResult<Item>> PostItem(ItemCreateDto itemDto)
        {
            // Check if box exists if BoxId is provided
            if (itemDto.BoxId != null && !await _context.Boxes.AnyAsync(b => b.Id == itemDto.BoxId))
            {
                return BadRequest("Specified box does not exist");
            }

            var item = new Item
            {
                Name = itemDto.Name,
                BasePrice = itemDto.BasePrice,
                BoxId = itemDto.BoxId // Only set the foreign key
                
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            // Load the box if you need it in the response
            if (item.BoxId.HasValue)
            {
                await _context.Entry(item)
                    .Reference(i => i.Box)
                    .LoadAsync();
            }

            return CreatedAtAction("GetItem", new { id = item.Id }, item);
        }

        // DELETE: api/Items/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            // Check if item is used in any orders
            var isUsedInOrders = await _context.OrderItems.AnyAsync(oi => oi.ItemId == id);
            if (isUsedInOrders)
            {
                return BadRequest("Cannot delete item referenced in existing orders");
            }

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ItemExists(int id)
        {
            return _context.Items.Any(e => e.Id == id);
        }
    }

    // DTO classes
    public class ItemCreateDto
    {
        public string Name { get; set; }
        public decimal BasePrice { get; set; }
        public int BoxId { get; set; }
    }

    public class ItemUpdateDto
    {
        public string Name { get; set; }
        public decimal BasePrice { get; set; }
    }
}