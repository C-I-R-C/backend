using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;



namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]


    public class IngredientsController : ControllerBase
    {

        private readonly IngredientsService _ingredientsService;

        public IngredientsController(ApplicationDbContext context, IngredientsService ingredientsService)
        {

            _ingredientsService = ingredientsService;
        
        }


        [HttpGet]
        public async Task<ActionResult<PagedResult<Ingredient>>> 
            GetIngredients(
                [FromQuery] int pageNumber = 1,
                [FromQuery] int pageSize = 10 )
        {

            var parameters = new PaginationParameters
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _ingredientsService.GetIngredients(parameters);
            return Ok(result);

        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Ingredient>> GetIngredient(int id)
        {

            try
            {

                return Ok(await _ingredientsService.GetIngredient(id));

            }

            catch (ArgumentException)
            {

                return NoContent();

            }

            catch
            {

                return Problem();

            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> 
            PutIngredient(
                int id, 
                Ingredient ingredient )
        {

            try
            {

                await _ingredientsService.PutIngredient(id, ingredient);
                return Ok();

            }

            catch (ArgumentException)
            {

                return NoContent();

            }
            catch {

                return Problem();

            }

        }


        [HttpPost]
        public async Task <ActionResult<IngredientDto>> 
            Create(
                [FromBody] IngredientCreateDto dto )
        {

            return await _ingredientsService.Create(dto);

        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> 
            DeleteIngredient(
                int id )
        {

            try
            {

                await _ingredientsService.DeleteIngredient(id);
                return Ok();

            }

            catch(DivideByZeroException)
            { 

                return BadRequest(new
                {
                    Message = "Cannot delete ingredient used in flowers"
                });

            }

            catch
            {

                return Problem();

            }
        }


        [HttpGet("low-stock")]
        public async Task<ActionResult<List<IngredientStockDto>>> 
            GetLowStockIngredients(
                [FromQuery] int count = 5 )
        {

            try
            {

                if (count < 1 || count > 50)
                {
                    return BadRequest("Count must be between 1 and 50");
                }


                var lowStockIngredients = await _ingredientsService.GetLowestStockIngredients(count);
                return Ok(lowStockIngredients);

            }

            catch (Exception)
            {

                return StatusCode(500, "An error occurred while retrieving low stock ingredients");

            }

        }


        [HttpPatch("{id}/stock")]
        public async Task<ActionResult<IngredientDto>> 
            UpdateIngredientStock(
                int id,
                [FromBody] UpdateIngredientStockDto updateDto )

        {
            try
            {

                var result = await _ingredientsService.UpdateIngredientStock(id, updateDto);
                return Ok(result);

            }

            catch (KeyNotFoundException ex)
            {

                return NotFound(ex.Message);

            }

            catch (InvalidOperationException ex)
            {

                return BadRequest(ex.Message);

            }

            catch (Exception)
            {

                return StatusCode(500, "Error updating ingredient stock");

            }
        }


    }



}
