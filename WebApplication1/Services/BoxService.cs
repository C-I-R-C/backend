using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class BoxService
    {
        private readonly ApplicationDbContext _context;

        public BoxService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Box>> GetBoxes()
        {
            return await _context.Boxes.ToListAsync();
        }
        public async Task<Box> GetBox(int id)
        {
            var box = await _context.Boxes.FindAsync(id);

            if (box == null)
            {
                throw new DivideByZeroException();
            }

            return box;
        }
        public async Task PutBox(int id, Box box)
        {
            if (id != box.Id)
            {
                throw new ArgumentException();
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
                    throw new DivideByZeroException();
                }
                else
                {
                    throw;
                }
            }

        }
        public async Task<ActionResult<BoxDto>> Create([FromBody] BoxCreateDto dto)
        {
            var box = new Box
            {
                Name = dto.Name,
                InStock = dto.InStock,
                CostPerUnit = dto.CostPerUnit,
            };

            _context.Boxes.Add(box);
            await _context.SaveChangesAsync();
            return new BoxDto { Id = box.Id, Name = box.Name, InStock = box.InStock, CostPerUnit = box.CostPerUnit };
        }
        public async Task DeleteBox(int id)
        {
            var box = await _context.Boxes.FindAsync(id);
            if (box == null)
            {
                throw new DivideByZeroException();
            }

            _context.Boxes.Remove(box);
            await _context.SaveChangesAsync();

        }
        public async Task<BoxDto> UpdateBoxStock(int boxId, BoxStockUpdateDto updateDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var box = await _context.Boxes
                    .FirstOrDefaultAsync(b => b.Id == boxId);

                if (box == null)
                {
                    throw new KeyNotFoundException($"Box with ID {boxId} not found");
                }

                if (updateDto.IsIncrement)
                {
                    box.InStock += updateDto.Quantity;

                }
                else
                {
                    if (box.InStock < updateDto.Quantity)
                    {
                        throw new InvalidOperationException(
                            $"Cannot remove {updateDto.Quantity} from stock. Only {box.InStock} available.");
                    }
                    box.InStock -= updateDto.Quantity;


                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new BoxDto
                {
                    Id = box.Id,
                    Name = box.Name,
                    InStock = box.InStock,
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        private bool BoxExists(int id)
        {
            return _context.Boxes.Any(e => e.Id == id);
        }
    }
}
