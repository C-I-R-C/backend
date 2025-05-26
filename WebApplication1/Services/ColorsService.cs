using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class ColorsService
    {
        private readonly ApplicationDbContext _context;

        public ColorsService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<IEnumerable<Color>> GetColors()
        {
            return await _context.Colors.ToListAsync();
        }

        public async Task<Color> GetColor(int id)
        {
            var color = await _context.Colors.FindAsync(id);

            if (color == null)
            {
                throw new DivideByZeroException();
            }

            return color;
        }

        public async Task PutColor(int id, Color color)
        {
            if (id != color.Id)
            {
                throw new DivideByZeroException();
            }

            _context.Entry(color).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ColorExists(id))
                {
                    throw new DivideByZeroException();
                }
                else
                {
                    throw;
                }
            }

        }
        public async Task<ActionResult<ColorDto>> Create([FromBody] ColorCreateDto colorDto)
        {
            var color = new Color
            {
                Name = colorDto.Name,
                IsNatural = colorDto.IsNatural
            };

            _context.Colors.Add(color);
            await _context.SaveChangesAsync();
            return new ColorDto
                {
                    Id = color.Id,
                    Name = color.Name,
                    IsNatural = color.IsNatural
                };
        }
        [HttpDelete("{id}")]
        public async Task DeleteColor(int id)
        {
            var color = await _context.Colors.FindAsync(id);
            if (color == null)
            {
                throw new DivideByZeroException();
            }
            var isUsedInFlowers = await _context.Flowers.AnyAsync(oi => oi.ColorId == id);
            if (isUsedInFlowers)
            {
                throw new ArgumentException();
            }
            _context.Colors.Remove(color);
            await _context.SaveChangesAsync();

        }

        private bool ColorExists(int id)
        {
            return _context.Colors.Any(e => e.Id == id);
        }
    }
}
