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
    public class BoxesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BoxesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Boxes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Box>>> GetBoxes()
        {
            return await _context.Boxes.ToListAsync();
        }

        // GET: api/Boxes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Box>> GetBox(int id)
        {
            var box = await _context.Boxes.FindAsync(id);

            if (box == null)
            {
                return NotFound();
            }

            return box;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBox(int id, Box box)
        {
            if (id != box.Id)
            {
                return BadRequest();
            }

            _context.Entry(box).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BoxExists(id))
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

        // POST: api/Boxes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task <ActionResult<BoxDto>> Create([FromBody] BoxCreateDto dto)
        {
            var box = new Box
            {
                Name = dto.Name,
                InStock = dto.InStock
            };

            _context.Boxes.Add(box);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(PutBox), new { id = box.Id },
                new BoxDto { Id = box.Id, Name = box.Name, InStock = box.InStock });
        }


        // DELETE: api/Boxes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBox(int id)
        {
            var box = await _context.Boxes.FindAsync(id);
            if (box == null)
            {
                return NotFound();
            }

            _context.Boxes.Remove(box);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BoxExists(int id)
        {
            return _context.Boxes.Any(e => e.Id == id);
        }
    }
}
