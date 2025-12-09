using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;



namespace WebApplication1.Services
{

    public class IngredientsService
    {

        private readonly ApplicationDbContext _context;


        public IngredientsService(ApplicationDbContext context)
        {

            _context = context;

        }


        public async Task<PagedResult<Ingredient>> 
            GetIngredients(
                PaginationParameters parameters )

        {

            var query = _context.Ingredients.AsQueryable();
            
            query = query.OrderBy(i => i.Name);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();
            
            
            return new PagedResult<Ingredient>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize
            };

        }


        public async Task<Ingredient> 
            GetIngredient(
                int id )

        {

            var ingredient = await _context.Ingredients.FindAsync(id);


            if (ingredient == null)
            {

                throw new ArgumentException("NoIngre");
            
            }


            return ingredient;

        }


        public async Task 
            DeleteIngredient(
                int id )

        {

            using var transaction = await _context.Database.BeginTransactionAsync();


            try
            {

                var ingredient = await _context.Ingredients.FindAsync(id);
                
                if (ingredient == null)
                {

                    throw new ArgumentException("No Ingredient with such id");
                
                }

                var isUsedInFlowers = await _context.FlowerIngredients
                    .AnyAsync(fi => fi.IngredientId == id);


                if (isUsedInFlowers)
                {

                    throw new DivideByZeroException();
                    
                }


                _context.Ingredients.Remove(ingredient);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

            }

            catch (Exception)
            {

                await transaction.RollbackAsync();
                throw;

            }

        }


        public async Task 
            PutIngredient(
                int id, 
                Ingredient ingredient )

        {

            if (id != ingredient.Id)
            {

                throw new ArgumentException("NoIngre");
            
            }


            _context.Entry(ingredient).State = EntityState.Modified;


            try
            {

                await _context.SaveChangesAsync();

            }

            catch (DbUpdateConcurrencyException)
            {

                if (!IngredientExists(id))
                {

                    throw new ArgumentException("NoIngre");

                }

                else
                {

                    throw;

                }

            }

        }


        public async Task<IngredientDto> 
            Create(
                [FromBody] IngredientCreateDto dto )

        {

            var ingredient = new Ingredient
            {

                Name = dto.Name,

                InStock = dto.InStock,

                CostPerUnit = dto.CostPerUnit

            };


            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();


            return
                new IngredientDto
                {
                    Id = ingredient.Id,
                    Name = ingredient.Name,
                    InStock = ingredient.InStock,
                    CostPerUnit = ingredient.CostPerUnit
                };

        }


        private bool 
            IngredientExists(
                int id )

        {

            return _context.Ingredients.Any(e => e.Id == id);

        }


        public async Task<List<IngredientStockDto>> 
            GetLowestStockIngredients(
                int count )

        {

            return await _context.Ingredients
                .OrderBy(i => i.InStock)
                .Take(count)
                .Select(i => new IngredientStockDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    InStock = i.InStock,
                    CostPerUnit = i.CostPerUnit,
                })
                .ToListAsync();

        }


        public async Task<IngredientDto> 
            UpdateIngredientStock(
                int id, 
                UpdateIngredientStockDto updateDto )

        {

            using var transaction = await _context.Database.BeginTransactionAsync();


            try
            {

                var ingredient = await _context.Ingredients.FindAsync(id);


                if (ingredient == null)
                {

                    throw new KeyNotFoundException($"Ingredient with ID {id} not found");

                }


                if (updateDto.IsIncrement)
                {

                    ingredient.InStock += updateDto.Quantity;

                }

                else
                {

                    if (ingredient.InStock < updateDto.Quantity)
                    {

                        throw new InvalidOperationException(
                            $"Cannot remove {updateDto.Quantity} from stock. Only {ingredient.InStock} available.");

                    }

                    ingredient.InStock -= updateDto.Quantity;

                }


                await _context.SaveChangesAsync();
                await transaction.CommitAsync();


                return new IngredientDto
                {
                    Id = ingredient.Id,
                    Name = ingredient.Name,
                    InStock = ingredient.InStock,
                    CostPerUnit = ingredient.CostPerUnit,
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
