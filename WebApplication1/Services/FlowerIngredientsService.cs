using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class FlowerIngredientsService
    {
        private readonly ApplicationDbContext _context;

        public FlowerIngredientsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<FlowerIngredientDto>> GetIngredientsForFlower(int flowerId, PaginationParameters parameters)
        {
            bool flowerExists = await _context.Flowers.AnyAsync(f => f.Id == flowerId);
            if (!flowerExists)
            {
                throw new DivideByZeroException($"Flower with ID {flowerId} not found");
            }

            var query = from fi in _context.FlowerIngredients
                        join i in _context.Ingredients on fi.IngredientId equals i.Id
                        where fi.FlowerId == flowerId
                        select new FlowerIngredientDto
                        {
                            FlowerId = fi.FlowerId,
                            IngredientId = fi.IngredientId,
                            QuantityRequired = fi.QuantityRequired,
                            Ingredient = new IngredientDto
                            {
                                Id = i.Id,
                                Name = i.Name,
                                InStock = i.InStock,
                                CostPerUnit = i.CostPerUnit
                            }
                        };
            query = query.OrderBy(dto => dto.Ingredient.Name);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return new PagedResult<FlowerIngredientDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize
            };
        }

        public async Task<ActionResult<FlowerIngredient>> GetFlowerIngredient(int id)
        {
            var flowerIngredient = await _context.FlowerIngredients.FindAsync(id);

            if (flowerIngredient == null)
            {
                throw new DivideByZeroException();
            }

            return flowerIngredient;
        }

        public async Task PutFlowerIngredient(int id, FlowerIngredient flowerIngredient)
        {
            if (id != flowerIngredient.FlowerId)
            {
                throw new BadImageFormatException();
            }

            _context.Entry(flowerIngredient).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlowerIngredientExists(id))
                {
                    throw new DivideByZeroException();
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task DeleteFlowerIngredient(int id)
        {
            var flowerIngredient = await _context.FlowerIngredients.FindAsync(id);
            if (flowerIngredient == null)
            {
                throw new DivideByZeroException();
            }

            _context.FlowerIngredients.Remove(flowerIngredient);
            await _context.SaveChangesAsync();
        }

        private bool FlowerIngredientExists(int id)
        {
            return _context.FlowerIngredients.Any(e => e.FlowerId == id);
        }
        public async Task<FlowerIngredientDto> AddIngredientToFlower(
       int flowerId, [FromBody] AddIngredientToFlowerDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            if (dto.QuantityRequired <= 0)
                throw new ArgumentException();

            var flower = await _context.Flowers
                .Include(f => f.FlowerIngredients)
                .ThenInclude(fi => fi.Ingredient)
                .FirstOrDefaultAsync(f => f.Id == flowerId);

            if (flower == null)
                throw new DivideByZeroException();

            var ingredient = await _context.Ingredients
                .FirstOrDefaultAsync(i => i.Id == dto.IngredientId);

            if (ingredient == null)
                throw new DivideByZeroException();
            var existing = flower.FlowerIngredients
                .FirstOrDefault(fi => fi.IngredientId == dto.IngredientId);

            decimal costChange = 0;

            if (existing != null)
            {
                costChange = ingredient.CostPerUnit * (dto.QuantityRequired - existing.QuantityRequired);
                existing.QuantityRequired = dto.QuantityRequired;
            }
            else
            {
                costChange = ingredient.CostPerUnit * dto.QuantityRequired;

                var newFlowerIngredient = new FlowerIngredient
                {
                    FlowerId = flowerId,
                    IngredientId = dto.IngredientId,
                    QuantityRequired = dto.QuantityRequired
                };

                _context.FlowerIngredients.Add(newFlowerIngredient);
                flower.FlowerIngredients.Add(newFlowerIngredient);
            }

            flower.CostPerUnit += costChange;


            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new FlowerIngredientDto
            {
                FlowerId = flowerId,
                IngredientId = dto.IngredientId,
                QuantityRequired = dto.QuantityRequired,
                Ingredient = new IngredientDto
                {
                    Id = ingredient.Id,
                    Name = ingredient.Name,
                    InStock = ingredient.InStock,
                    CostPerUnit = ingredient.CostPerUnit
                }
            };
        }

        public async Task RemoveIngredientFromFlower(int flowerId, int ingredientId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var flower = await _context.Flowers
                    .Include(f => f.FlowerIngredients)
                    .FirstOrDefaultAsync(f => f.Id == flowerId);
                var ingredient = await _context.Ingredients.FirstOrDefaultAsync(i => i.Id == ingredientId);
                if ((flower == null) | (ingredient == null))
                {
                    throw new DivideByZeroException();
                }

                var flowerIngredient = await _context.FlowerIngredients
                    .FirstOrDefaultAsync(fi => fi.FlowerId == flowerId && fi.IngredientId == ingredientId);

                if (flowerIngredient == null)
                {
                    throw new DivideByZeroException();
                }

                _context.FlowerIngredients.Remove(flowerIngredient);

                flower.CostPerUnit = flower.CostPerUnit - flowerIngredient.QuantityRequired * ingredient.CostPerUnit;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

            }
        }
    }

}


