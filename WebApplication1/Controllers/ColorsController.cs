using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ColorsController : ControllerBase
    {
        private readonly DataService _data;

        public ColorsController(DataService data)
        {
            _data = data;
        }

        // GET api/colors
        [HttpGet]
        public ActionResult<IEnumerable<ColorDto>> GetAll()
        {
            var colors = _data.Colors.Select(c => new ColorDto
            {
                Id = c.Id,
                Name = c.Name,
                IsNatural = c.IsNatural
            });

            return Ok(colors);
        }

        // GET api/colors/5
        [HttpGet("{id}")]
        public ActionResult<ColorDto> GetById(int id)
        {
            var color = _data.Colors.FirstOrDefault(c => c.Id == id);
            if (color == null) return NotFound();

            return Ok(new ColorDto
            {
                Id = color.Id,
                Name = color.Name,
                IsNatural = color.IsNatural
            });
        }

        // POST api/colors
        [HttpPost]
        public ActionResult<ColorDto> Create([FromBody] ColorCreateDto colorDto)
        {
            var color = new Color
            {
                Id = _data.NextColorId,
                Name = colorDto.Name,
                IsNatural = colorDto.IsNatural
            };

            _data.Colors.Add(color);

            return CreatedAtAction(nameof(GetById), new { id = color.Id },
                new ColorDto
                {
                    Id = color.Id,
                    Name = color.Name,
                    IsNatural = color.IsNatural
                });
        }

        // PUT api/colors/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ColorUpdateDto colorDto)
        {
            var color = _data.Colors.FirstOrDefault(c => c.Id == id);
            if (color == null) return NotFound();

            if (!string.IsNullOrEmpty(colorDto.Name))
                color.Name = colorDto.Name;

            if (colorDto.IsNatural.HasValue)
                color.IsNatural = colorDto.IsNatural.Value;

            return NoContent();
        }

        // DELETE api/colors/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var color = _data.Colors.FirstOrDefault(c => c.Id == id);
            if (color == null) return NotFound();

            // Check if color is used by any flowers
            if (_data.Flowers.Any(f => f.ColorId == id))
            {
                return BadRequest("Cannot delete color assigned to flowers");
            }

            _data.Colors.Remove(color);
            return NoContent();
        }
    }
}
