using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BoxesController : ControllerBase
    {
        private readonly DataService _data;

        public BoxesController(DataService data)
        {
            _data = data;
        }

        // GET api/boxes
        [HttpGet]
        public ActionResult<IEnumerable<BoxDto>> GetAll()
        {
            return Ok(_data.Boxes.Select(b => new BoxDto
            {
                Id = b.Id,
                Name = b.Name,
                InStock = b.InStock
            }));
        }

        // GET api/boxes/5
        [HttpGet("{id}")]
        public ActionResult<BoxDto> GetById(int id)
        {
            var box = _data.Boxes.FirstOrDefault(b => b.Id == id);
            return box == null ? NotFound() : Ok(new BoxDto
            {
                Id = box.Id,
                Name = box.Name,
                InStock = box.InStock
            });
        }

        // POST api/boxes
        [HttpPost]
        public ActionResult<BoxDto> Create([FromBody] BoxCreateDto dto)
        {
            var box = new Box
            {
                Id = _data.NextBoxId,
                Name = dto.Name,
                InStock = dto.InStock
            };

            _data.Boxes.Add(box);

            return CreatedAtAction(nameof(GetById), new { id = box.Id },
                new BoxDto { Id = box.Id, Name = box.Name, InStock = box.InStock });
        }

        // PUT api/boxes/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] BoxUpdateDto dto)
        {
            var box = _data.Boxes.FirstOrDefault(b => b.Id == id);
            if (box == null) return NotFound();

            if (!string.IsNullOrEmpty(dto.Name)) box.Name = dto.Name;
            if (dto.InStock.HasValue) box.InStock = dto.InStock.Value;

            return NoContent();
        }

        // DELETE api/boxes/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var box = _data.Boxes.FirstOrDefault(b => b.Id == id);
            if (box == null) return NotFound();

            if (_data.Items.Any(i => i.BoxId == id))
                return BadRequest("Cannot delete box assigned to items");

            _data.Boxes.Remove(box);
            return NoContent();
        }

        // PATCH api/boxes/5/stock
        [HttpPatch("{id}/stock")]
        public ActionResult<BoxDto> UpdateStock(int id, [FromBody] StockUpdateDto dto)
        {
            var box = _data.Boxes.FirstOrDefault(b => b.Id == id);
            if (box == null) return NotFound();

            box.InStock += dto.Adjustment;
            if (box.InStock < 0) box.InStock = 0;

            return Ok(new BoxDto { Id = box.Id, Name = box.Name, InStock = box.InStock });
        }
    }

    public class StockUpdateDto
    {
        [Required(ErrorMessage = "Adjustment value is required")]
        public int Adjustment { get; set; }
    }
}
