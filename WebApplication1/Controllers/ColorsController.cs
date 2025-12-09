using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;



namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]


    public class ColorsController : ControllerBase
    {

        private readonly ColorsService _colorsService;


        public ColorsController(ColorsService colorsService)
        {

            _colorsService = colorsService;

        }


        [HttpGet]
        public async Task<ActionResult<PagedResult<Color>>> 
            GetColors(
                [FromQuery] int pageNumber = 1,
                [FromQuery] int pageSize = 10 )

        {

            var parameters = new PaginationParameters
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _colorsService.GetColors(parameters);
            
            return Ok(result);

        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Color>> 
            GetColor(
                int id )
        {

            try
            {

                return await _colorsService.GetColor(id);

            }

            catch (DivideByZeroException)
            {

                return BadRequest("Color not found");

            }

            catch 
            {

                return Problem();

            }

        }


        [HttpPut("{id}")]
        public async Task<IActionResult> 
            PutColor(
                int id, 
                Color color )

        {

            try
            {

                await _colorsService.PutColor(id, color);
                return Ok();
            
            }
            
            catch (DivideByZeroException)
            {

                return BadRequest("invalid Id");
            
            }
            catch
            {

                return Problem();
            
            }


        }


        [HttpPost]
        public async Task <ActionResult<ColorDto>> 
            Create(
                [FromBody] ColorCreateDto colorDto )

        {

            return await _colorsService.Create(colorDto);

        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> 
            DeleteColor(
                int id )

        {
            try
            {

                await _colorsService.DeleteColor(id);
                return Ok();
            
            }
            
            catch (DivideByZeroException)
            {

                return BadRequest("No such color");
            
            }
            catch (ArgumentException)
            {

                return BadRequest("Already used in flower");
            
            }
            catch
            {

                return Problem();
            
            }
        }


    }
}
