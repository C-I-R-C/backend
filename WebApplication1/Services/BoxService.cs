using Microsoft.EntityFrameworkCore;
using WebApplication1.Exceptions;
using WebApplication1.Models;



namespace WebApplication1.Services
{

    public interface IBoxService
    {

        Task<PagedResult<Box>> GetBoxes(PaginationParameters parameters);


        Task<Box> GetBox(int id);


        Task PutBox(int id, Box box);


        Task<BoxDto> Create(BoxCreateDto dto);


        Task DeleteBox(int id);


        Task<BoxDto> UpdateBoxStock(int boxId, BoxStockUpdateDto updateDto);

    }


    public class BoxService : IBoxService
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<BoxService> _logger;


        public BoxService(ApplicationDbContext context, ILogger<BoxService> logger)
        {

            _context = context;
            _logger = logger;

        }


        public async Task<PagedResult<Box>> 
            GetBoxes(
                PaginationParameters parameters )

        {

            try
            {

                var query = _context.Boxes.AsQueryable();

                var totalCount = await query.CountAsync();

                var items = await query
                    .OrderBy(b => b.Id) 
                    .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                    .Take(parameters.PageSize)
                    .ToListAsync();


                return new PagedResult<Box>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = parameters.PageNumber,
                    PageSize = parameters.PageSize
                };

            }

            catch (Exception ex)
            {

                _logger.LogError(ex, "Error retrieving boxes");
                throw;

            }

        }

        public async Task<Box> 
            GetBox(
                int id ) 

        {

            try
            {

                var box = await _context.Boxes.FindAsync(id);


                if (box == null)
                {

                    throw new BoxNotFoundException();
                }


                return box;

            }

            catch (BoxNotFoundException)
            {

                throw;

            }

            catch (Exception ex)
            {

                _logger.LogError(ex, "Error retrieving box with ID {BoxId}", id);
                throw;

            }

        }


        public async Task 
            PutBox(
                int id, 
                Box box )

        {

            try
            {

                if (id != box.Id)
                {

                    throw new AppValidationException("Box ID in URL does not match ID in body");
                
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

                        throw new BoxNotFoundException();

                    }

                    else
                    {

                        throw;

                    }

                }

            }

            catch (BoxNotFoundException)
            {

                throw;

            }

            catch (Exception ex)
            {

                _logger.LogError(ex, "Error updating box with ID {BoxId}", id);
                throw;

            }

        }


        public async Task<BoxDto> 
            Create(
                BoxCreateDto dto )

        {

            try
            {
                
                if (dto.InStock < 0)
                {

                    throw new AppValidationException("Stock quantity cannot be negative");

                }


                if (dto.CostPerUnit < 0)
                {

                    throw new AppValidationException("Cost per unit cannot be negative");

                }


                var box = new Box
                {
                    Name = dto.Name,
                    InStock = dto.InStock,
                    CostPerUnit = dto.CostPerUnit,
                };


                _context.Boxes.Add(box);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created new box with ID {BoxId}", box.Id);


                return new BoxDto
                {
                    Id = box.Id,
                    Name = box.Name,
                    InStock = box.InStock,
                    CostPerUnit = box.CostPerUnit
                };

            }

            catch (Exception ex)
            {

                _logger.LogError(ex, "Error creating new box");
                throw;

            }

        }

        public async Task 
            DeleteBox(
                int id )

        {

            try
            {

                var box = await _context.Boxes.FindAsync(id);


                if (box == null)
                {

                    throw new BoxNotFoundException();

                }

                _context.Boxes.Remove(box);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted box with ID {BoxId}", id);

            }

            catch (BoxNotFoundException)
            {

                throw;

            }

            catch (Exception ex)
            {

                _logger.LogError(ex, "Error deleting box with ID {BoxId}", id);
                throw;

            }

        }


        public async Task<BoxDto> 
            UpdateBoxStock(
                int boxId, 
                BoxStockUpdateDto updateDto )

        {

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {

                var box = await _context.Boxes
                    .FirstOrDefaultAsync(b => b.Id == boxId);


                if (box == null)
                {

                    throw new BoxNotFoundException();

                }


                if (updateDto.Quantity <= 0)
                {

                    throw new AppValidationException("Quantity must be greater than zero");

                }


                if (updateDto.IsIncrement)
                {

                    box.InStock += updateDto.Quantity;

                }

                else
                {

                    if (box.InStock < updateDto.Quantity)
                    {

                        throw new BusinessRuleException(
                            $"Cannot remove {updateDto.Quantity} from stock. Only {box.InStock} available.");

                    }

                    box.InStock -= updateDto.Quantity;

                }


                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Updated stock for box {BoxId}. New stock: {Stock}", boxId, box.InStock);


                return new BoxDto
                {
                    Id = box.Id,
                    Name = box.Name,
                    InStock = box.InStock,
                    CostPerUnit = box.CostPerUnit
                };

            }

            catch
            {

                await transaction.RollbackAsync();
                throw;

            }

        }


        private bool 
            BoxExists(
                int id )

        {

            return _context.Boxes.Any(e => e.Id == id);

        }


    }


}