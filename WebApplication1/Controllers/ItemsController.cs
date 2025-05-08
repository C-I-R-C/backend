using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly DataService _data;

        public ItemsController(DataService data)
        {
            _data = data;
        }

        // GET api/items
        [HttpGet]
        public ActionResult<IEnumerable<Item>> GetAll()
        {
            return Ok(_data.Items);
        }

        // GET api/items/5
        [HttpGet("{id}")]
        public ActionResult<Item> GetById(int id)
        {
            var item = _data.Items.FirstOrDefault(i => i.Id == id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        // POST api/items
        [HttpPost]
        public ActionResult<Item> Create([FromBody] ItemCreateDto itemDto)
        {
            var item = new Item
            {
                Id = _data.NextItemId,
                Name = itemDto.Name,
                BasePrice = itemDto.BasePrice
            };

            _data.Items.Add(item);
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        // PUT api/items/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ItemUpdateDto itemDto)
        {
            var item = _data.Items.FirstOrDefault(i => i.Id == id);
            if (item == null) return NotFound();

            item.Name = itemDto.Name;
            item.BasePrice = itemDto.BasePrice;

            return NoContent();
        }

        // DELETE api/items/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var item = _data.Items.FirstOrDefault(i => i.Id == id);
            if (item == null) return NotFound();

            // Check if item is used in any orders
            if (_data.Orders.Any(o => o.OrderItems.Any(oi => oi.ItemId == id)))
            {
                return BadRequest("Cannot delete item referenced in existing orders");
            }

            _data.Items.Remove(item);
            return NoContent();
        }
    }
}
