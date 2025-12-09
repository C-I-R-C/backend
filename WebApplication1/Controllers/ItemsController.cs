using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;



namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]


    public class ItemsController : ControllerBase
    {

        private readonly ItemsService _itemsService;

        public ItemsController(ItemsService itemsService)
        {

            _itemsService = itemsService;

        }


        [HttpGet]
        public async Task<ActionResult<PagedResult<ItemResponseDto>>> 
            GetItems(
                [FromQuery] int pageNumber = 1,
                [FromQuery] int pageSize = 10 )

        {

            var parameters = new PaginationParameters
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _itemsService.GetItems(parameters);
            return Ok(result);

        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ItemResponseDto>> 
            GetItem(
                int id )

        {

            try
            {

                return await _itemsService.GetItem(id);

            }

            catch (DivideByZeroException)
            {

                return BadRequest("Item not found");

            }

            catch
            {

                return Problem();

            }

        }


        [HttpPut("{id}")]
        public async Task<IActionResult> 
            PutItem(
                int id, 
                [FromBody] ItemUpdateDto itemDto )

        {

            try
            {

                if (id != itemDto.Id)
                {

                    return BadRequest("ID mismatch");

                }


                var result = await _itemsService.UpdateItemAsync(itemDto);
                return result ? Ok() : NotFound();

            }

            catch (KeyNotFoundException ex)
            {

                return NotFound(ex.Message);

            }

            catch (Exception ex)
            {

                return Problem(detail: ex.Message);

            }

        }


        [HttpPost]
        public async Task<ActionResult<Item>> 
            PostItem(
                ItemCreateDto itemDto )

        {

            try
            {

                return await _itemsService.PostItem(itemDto);

            }

            catch (DivideByZeroException)
            {

                return BadRequest("Item not found");

            }

            catch
            {

                return Problem();

            }

        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> 
            DeleteItem(
                int id )

        {

            try
            {

                await _itemsService.DeleteItem(id);
                return Ok();

            }

            catch (DivideByZeroException)
            {

                return BadRequest("Item not found");

            }

            catch (AbandonedMutexException)
            {

                return BadRequest("Item is used in order");

            }

            catch
            {

                return Problem();

            }

        }


        [HttpGet("{id}/cost-analysis")]
        public async Task<ActionResult<ItemCostAnalysisDto>> 
            GetItemCostAnalysis(
                int id )

        {
            try
            {

                var analysis = await _itemsService.CalculateItemCostAnalysis(id);
                return Ok(analysis);

            }

            catch (Exception ex)
            {

                return BadRequest(ex.Message);

            }
        }


    }



}