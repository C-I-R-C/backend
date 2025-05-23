using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Services
{
    public class BoxService
    {
        private readonly ApplicationDbContext _context;

        public BoxService(ApplicationDbContext context)
        {
            _context = context;
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
    }
}
