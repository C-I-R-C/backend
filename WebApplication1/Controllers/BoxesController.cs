// Controllers/BoxesController.cs
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;
using WebApplication1.Models;
using Microsoft.AspNetCore.Authorization;



namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]


    public class BoxesController : ControllerBase
    {

        private readonly IBoxService _boxService;


        public BoxesController(IBoxService boxService)
        {

            _boxService = boxService;

        }


        [HttpGet]
        public async Task<ActionResult<PagedResult<Box>>> 
            GetBoxes(
                [FromQuery] int pageNumber = 1, 
                [FromQuery] int pageSize = 10 )

        {

            var parameters = new PaginationParameters
            {

                PageNumber = pageNumber,
                PageSize = pageSize

            };

            var result = await _boxService.GetBoxes(parameters);

            return Ok(result);

        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Box>> 
            GetBox(
                int id )

        {

            return await _boxService.GetBox(id);

        }


        [HttpPut("{id}")]
        public async Task<IActionResult> 
            PutBox(
                int id, 
                Box box )

        {

            await _boxService.PutBox(id, box);
            return Ok();

        }


        [HttpPost]
        public async Task<ActionResult<BoxDto>> 
            Create(
                [FromBody] BoxCreateDto dto )

        {

            var result = await _boxService.Create(dto);
            return CreatedAtAction(nameof(GetBox), new { id = result.Id }, result);

        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> 
            DeleteBox(
                int id )

        {

            await _boxService.DeleteBox(id);
            return NoContent();

        }


        [HttpPatch("{id}/stock")]
        public async Task<ActionResult<BoxDto>> 
            UpdateBoxStock(
                int id, 
                [FromBody] BoxStockUpdateDto updateDto )

        {

            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);

            }

            var result = await _boxService.UpdateBoxStock(id, updateDto);

            return Ok(result);

        }


    }

}