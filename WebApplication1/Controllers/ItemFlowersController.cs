using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/items/{itemId}/flowers")]
    public class ItemFlowersController : ControllerBase
    {
        private readonly DataService _data;

        public ItemFlowersController(DataService data)
        {
            _data = data;
        }

        // GET api/items/5/flowers
        [HttpGet]
        public ActionResult<IEnumerable<ItemFlowerDto>> GetFlowersForItem(int itemId)
        {
            if (!_data.Items.Any(i => i.Id == itemId))
                return NotFound("Item not found");

            var itemFlowers = _data.ItemFlowers
                .Where(itemf => itemf.ItemId == itemId)
            .Select(itemf => new ItemFlowerDto
            {
                ItemId = itemf.ItemId,
                FlowerId = itemf.FlowerId,
                Quantity = itemf.Quantity,
                Flower = _data.Flowers
                    .Where(f => f.Id == itemf.FlowerId)
                    .Select(f => new FlowerDto
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
                    })
                    .FirstOrDefault()
                });

        return Ok(itemFlowers);
    }

    // POST api/items/5/flowers
    [HttpPost]
        public ActionResult<ItemFlowerDto> AddFlowerToItem(int itemId, [FromBody] AddFlowerToItemDto dto)
        {
            if (!_data.Items.Any(i => i.Id == itemId))
                return NotFound("Item not found");

            if (!_data.Flowers.Any(f => f.Id == dto.FlowerId))
                return NotFound("Flower not found");

            var existing = _data.ItemFlowers
                .FirstOrDefault(itemf => itemf.ItemId == itemId && itemf.FlowerId == dto.FlowerId) ;

            if (existing != null)
            {
                existing.Quantity += dto.Quantity;
            }
            else
            {
                _data.ItemFlowers.Add(new ItemFlower
                {
                    ItemId = itemId,
                    FlowerId = dto.FlowerId,
                    Quantity = dto.Quantity
                });
            }

            var flower = _data.Flowers.First(f => f.Id == dto.FlowerId);
            return CreatedAtAction(
                nameof(GetFlowersForItem),
                new { itemId },
                new ItemFlowerDto
                {
                    ItemId = itemId,
                    FlowerId = dto.FlowerId,
                    Quantity = dto.Quantity,
                    Flower = new FlowerDto
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
                        } : null
                    }
                });
        }

        // PUT api/items/5/flowers/3 (update quantity)
        [HttpPut("{flowerId}")]
        public IActionResult UpdateFlowerQuantity(int itemId, int flowerId, [FromBody] UpdateFlowerQuantityDto dto)
        {
            var itemFlower = _data.ItemFlowers
                .FirstOrDefault(itemf => itemf.ItemId == itemId && itemf.FlowerId == flowerId) ;

            if (itemFlower == null)
                return NotFound("Flower not found in this item");

            itemFlower.Quantity = dto.Quantity;
            return NoContent();
        }

        // DELETE api/items/5/flowers/3
        [HttpDelete("{flowerId}")]
        public IActionResult RemoveFlowerFromItem(int itemId, int flowerId)
        {
            var itemFlower = _data.ItemFlowers
                .FirstOrDefault(itemf => itemf.ItemId == itemId && itemf.FlowerId == flowerId) ;

            if (itemFlower == null)
                return NotFound("Flower not found in this item");

            _data.ItemFlowers.Remove(itemFlower);
            return NoContent();
        }
    }

// Supporting DTOs
}
