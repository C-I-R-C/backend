using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;



namespace WebApplication1.Services
{

    public class FlowersService
    {

        private readonly ApplicationDbContext _data;


        public FlowersService(ApplicationDbContext context)
        {

            _data = context;

        }


        public async Task<PagedResult<FlowerDto>> 
            GetAll(
                PaginationParameters parameters )

        {

            var query = _data.Flowers.Select(f => new FlowerDto
            {
                Id = f.Id,
                Name = f.Name,
                InStock = f.InStock,
                CostPerUnit = f.CostPerUnit,
                Color = f.Color != null ? new ColorDto
                {
                    Id = f.Color.Id,
                    Name = f.Color.Name,
                    IsNatural = f.Color.IsNatural
                } : null
            });

            query = query.OrderBy(f => f.Name);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();


            return new PagedResult<FlowerDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize
            };

        }


        public async Task<FlowerWithIngredientsDto> 
            GetById(
                int id )

        {

            var flower = await _data.Flowers.FirstOrDefaultAsync(f => f.Id == id);


            if (flower == null)
            {

                throw new DivideByZeroException();

            }


            return new FlowerWithIngredientsDto
            {
                Id = flower.Id,

                Name = flower.Name,

                InStock = flower.InStock,

                CostPerUnit = flower.CostPerUnit,

                Color = flower.Color != null ? new ColorDto
                {
                    Id = flower.Color.Id,
                    Name = flower.Color.Name,
                    IsNatural = flower.Color.IsNatural
                } : null,

                Ingredients = [.. _data.FlowerIngredients
                    .Where(fi => fi.FlowerId == id)
                    .Select(fi => new FlowerIngredientDto
                    {
                        FlowerId = fi.FlowerId,
                        IngredientId = fi.IngredientId,
                        QuantityRequired = fi.QuantityRequired,
                        Ingredient = _data.Ingredients
                            .Where(i => i.Id == fi.IngredientId)
                            .Select(i => new IngredientDto
                            {
                                Id = i.Id,
                                Name = i.Name,
                                InStock = i.InStock,
                                CostPerUnit = i.CostPerUnit
                            })
                            .FirstOrDefault()
                    })]

            };

        }


        public async Task<FlowerDto> 
            Create(
                [FromBody] FlowerCreateDto flowerDto )

        {

            var flower = new Flower
            {
                Name = flowerDto.Name,
                InStock = flowerDto.InStock,
                CostPerUnit = flowerDto.CostPerUnit,
                ColorId = flowerDto.ColorId
            };


            _data.Flowers.Add(flower);
            await _data.SaveChangesAsync();
            return new FlowerDto
                {
                    Id = flower.Id,
                    Name = flower.Name,
                    InStock = flower.InStock,
                    CostPerUnit = flower.CostPerUnit
                };

        }


        public async Task 
            Update(
                int id, 
                [FromBody] FlowerUpdateDto flowerDto )

        {

            var flower = _data.Flowers.FirstOrDefault(f => f.Id == id);


            if (flower == null)
            {

                throw new DivideByZeroException();

            }

            flower.Name = flowerDto.Name;
            flower.InStock = flowerDto.InStock;
            flower.CostPerUnit = flowerDto.CostPerUnit;
            flower.ColorId = flowerDto.ColorId;

            await _data.SaveChangesAsync();

        }


        public async Task 
            Delete(
                int id )

        {

            var flower = _data.Flowers.FirstOrDefault(f => f.Id == id);

            if (flower == null)
            {

                throw new DivideByZeroException();

            }


            if (_data.ItemFlowers.Any(itemf => itemf.FlowerId == id))
            {

                throw new BadImageFormatException();

            }


            _data.Flowers.Remove(flower);
            await _data.SaveChangesAsync();

        }


        public async Task<FlowerDto> 
            UpdateQuantity(
                int id, 
                FlowerQuantityUpdateDto updateDto )

        {

            using var transaction = await _data.Database.BeginTransactionAsync();


            try
            {

                var flower = await _data.Flowers
                    .Include(f => f.FlowerIngredients)
                        .ThenInclude(fi => fi.Ingredient)
                    .FirstOrDefaultAsync(f => f.Id == id);


                if (flower == null)
                {

                    throw new KeyNotFoundException("Flower not found");

                }


                var originalStock = flower.InStock;

                var originalIngredientStocks = flower.FlowerIngredients
                    .ToDictionary(fi => fi.IngredientId, fi => fi.Ingredient.InStock);


                if (updateDto.IsIncrement)
                {
                    
                    foreach (var flowerIngredient in flower.FlowerIngredients)
                    {

                        var ingredient = flowerIngredient.Ingredient;
                        var quantityToDeduct = flowerIngredient.QuantityRequired * updateDto.Quantity;


                        if (ingredient.InStock < quantityToDeduct)
                        {

                            throw new InvalidOperationException(
                                $"Not enough {ingredient.Name} in stock. " +
                                $"Need {quantityToDeduct}, have {ingredient.InStock}");

                        }


                        ingredient.InStock -= quantityToDeduct;

                    }

                    flower.InStock += updateDto.Quantity;

                }

                else
                {

                    if (flower.InStock < updateDto.Quantity)
                    {

                        throw new InvalidOperationException(
                            $"Cannot subtract {updateDto.Quantity} from stock. Only {flower.InStock} available.");

                    }

                    flower.InStock -= updateDto.Quantity;

                }


                await _data.SaveChangesAsync();
                await transaction.CommitAsync();


                return new FlowerDto
                {
                    Id = flower.Id,
                    Name = flower.Name,
                    CostPerUnit = flower.CostPerUnit,
                    InStock = flower.InStock,
                    Color = flower.Color != null ? new ColorDto
                    {
                        Id = flower.Color.Id,
                        Name = flower.Color.Name
                    } : null
                };


            }

            catch
            {

                await transaction.RollbackAsync();
                throw;

            }

        }


        public async Task<PagedResult<FlowerWithIngredientsDtoNew>> 
            GetFlowersByName(
                string name,
                PaginationParameters parameters )

        {

            var query = _data.Flowers
                .Where(f => f.Name.ToLower().Contains(name.ToLower()))
                .Include(f => f.Color)
                .Include(f => f.FlowerIngredients)
                    .ThenInclude(fi => fi.Ingredient)
                .AsQueryable();

            query = query.OrderBy(f => f.Name);

            var totalCount = await query.CountAsync();


            var flowers = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .Select(f => new FlowerWithIngredientsDtoNew
                {
                    Id = f.Id,
                    Name = f.Name,
                    InStock = f.InStock,
                    CostPerUnit = f.CostPerUnit,
                    Color = f.Color != null ? new ColorDto
                    {
                        Id = f.Color.Id,
                        Name = f.Color.Name
                    } : null,
                    Ingredients = f.FlowerIngredients.Select(fi => new FlowerIngredientDtoNew
                    {
                        IngredientId = fi.IngredientId,
                        IngredientName = fi.Ingredient.Name,
                        QuantityRequired = fi.QuantityRequired,
                        CostPerUnit = fi.Ingredient.CostPerUnit,
                        TotalCost = fi.QuantityRequired * fi.Ingredient.CostPerUnit
                    }).ToList()
                })
                .ToListAsync();


            return new PagedResult<FlowerWithIngredientsDtoNew>
            {
                Items = flowers,
                TotalCount = totalCount,
                PageNumber = parameters.PageSize > 0 ? parameters.PageNumber : 1,
                PageSize = parameters.PageSize
            };

        }


    }



}
