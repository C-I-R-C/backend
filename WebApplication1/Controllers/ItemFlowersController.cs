using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;



namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]


    public class ItemFlowersController : ControllerBase
    {

        private readonly ItemFlowersService _itemFlowersService;

        public ItemFlowersController(ItemFlowersService itemFlowersService)
        {

            _itemFlowersService = itemFlowersService;

        }


        [HttpGet]
        public async Task <ActionResult<PagedResult<ItemFlowerDto>>> 
            GetFlowersForItem(
                int itemId, 
                [FromQuery] int pageNumber = 1,
                [FromQuery] int pageSize = 10 )

        {

            var parameters = new PaginationParameters
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            try
            {

                var result =  await _itemFlowersService.GetFlowersForItem(itemId, parameters);
                return Ok(result);

            }

            catch (DivideByZeroException)
            {

                return BadRequest("Item not found");

            }

        }


        [HttpPost]
        public async Task <ActionResult<ItemFlowerDto>> 
            AddFlowerToItem(
                int itemId, 
                [FromBody] AddFlowerToItemDto dto )

        {

            try
            {

                return await _itemFlowersService.AddFlowerToItem(itemId, dto);

            }

            catch (DivideByZeroException)
            {

                return BadRequest("Item or flower not found");

            }

            catch
            {

                return Problem();

            }

        }


        [HttpPut("{flowerId}")]
        public async Task <IActionResult> 
            UpdateFlowerQuantity(
                int itemId, 
                int flowerId, 
                [FromBody] UpdateFlowerQuantityDto dto )

        {

            try
            {

                await _itemFlowersService.UpdateFlowerQuantity(itemId, flowerId, dto);
                return Ok();

            }

            catch (DivideByZeroException)
            {

                return BadRequest("Item or flower not found");

            }

            catch
            {

                return Problem();

            }

        }


        [HttpDelete("{itemId}/flowers/{flowerId}")]
        public async Task<IActionResult> 
            RemoveFlowerFromItem(
                int itemId, 
                int flowerId )

        {

            try
            {

                await _itemFlowersService.RemoveFlowerFromItem(itemId, flowerId);
                return NoContent();

            }

            catch (KeyNotFoundException ex)
            {

                return NotFound(ex.Message);

            }

            catch (Exception)
            {

                return StatusCode(500, "Error removing flower from item");

            }

        }


    }



}
