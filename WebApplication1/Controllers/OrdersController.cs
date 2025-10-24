using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1;
using WebApplication1.DTOs;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrdersController(ApplicationDbContext context, OrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetOrders()
        {
            return await _orderService.GetOrders();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponseDto>> GetOrder(int id)
        {
            try
            {
                return await _orderService.GetOrder(id);
            }
            catch (DivideByZeroException)
            {
                return NotFound();
            }
            catch
            {
                return Problem();
            }
        }

        [HttpPost]
        public async Task<ActionResult<OrderResponseDto>> Create([FromBody] OrderCreateDto orderDto)
        {
            try
            {
                return await _orderService.Create(orderDto);
            }
            catch (ArgumentException)
            {
                return BadRequest("Client not found");
            }
            catch (AbandonedMutexException)
            {
                return BadRequest($"Item not found");
            }
            catch
            {
                return Problem();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, OrderUpdateDto orderDto)
        {
            try
            {
                await _orderService.PutOrder(id, orderDto);
                return NoContent();
            }
            catch (DivideByZeroException)
            {
                return BadRequest("OrderNotFound");
            }
            catch
            {
                return Problem();
            }
        }

        [HttpPatch("{id}/complete")]
        public async Task<ActionResult<OrderResponseDto>> MarkAsCompleted(int id)
        {
            try
            {
                return await _orderService.MarkAsCompleted(id);
            }
            catch (DivideByZeroException)
            {
                return BadRequest("OrderNotFound");
            }
            catch
            {
                return Problem("BRUH");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                await _orderService.DeleteOrder(id);
                return NoContent();
            }
            catch (DivideByZeroException)
            {
                return BadRequest("OrderNotFound");
            }
            catch
            {
                return Problem();
            }
        }
        [HttpGet("query")]
        public async Task<ActionResult<PagedResponse<OrderResponseDto>>> QueryOrders(
    [FromQuery] OrderQueryDto query,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 20)
        {
                if (pageNumber < 1) return BadRequest("Page number must be at least 1");
                if (pageSize < 1 || pageSize > 100) return BadRequest("Page size must be between 1 and 100");
                DateTime? OrderDate =
                DateTime.TryParse(query.OrderDateString, out var date)? DateTime.SpecifyKind(date.Date, DateTimeKind.Utc) : null;
                DateTime? CompletionDate = DateTime.TryParse(query.CompletionDateString, out var date1) ? DateTime.SpecifyKind(date1.Date, DateTimeKind.Utc): null;
                if (OrderDate == null &&
                    CompletionDate == null &&
                    !query.IsCompleted.HasValue)
                {
                    return BadRequest("At least one filter parameter must be provided");
                }

                var result = await _orderService.QueryOrders(
                    OrderDate,
                    CompletionDate,
                    query.IsCompleted,
                    pageNumber,
                    pageSize);

                if (result.TotalCount == 0)
                {
                    return NotFound("No orders match the specified criteria");
                }

                return Ok(result);
            }
        [HttpGet("urgent")]
        public async Task<ActionResult<List<UrgentOrderDto>>> GetUrgentOrders(
    [FromQuery] int count = 5)
        {
            try
            {
                if (count < 1 || count > 20)
                    return BadRequest("Count must be between 1 and 20");

                var urgentOrders = await _orderService.GetMostUrgentOrders(count);
                return Ok(urgentOrders);
            }
            catch
            {
                return StatusCode(500, "Error retrieving urgent orders");
            }
        }
        [HttpGet("{id}/profit")]
        public async Task<ActionResult<OrderProfitDto>> GetOrderProfit(int id)
        {
            try
            {
                var profit = await _orderService.CalculateOrderProfit(id);
                return Ok(profit);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch
            {
                return StatusCode(500, "Error calculating profit");
            }
        }

        [HttpGet("validate-order/{orderId}")]
        public async Task<ActionResult<OrderValidationResult>> ValidateOrderMaterials(int orderId)
        {
            try
            {
                return await _orderService.ValidateOrderMaterials(orderId);
            }
            catch (DivideByZeroException)
            {
                return BadRequest("NotFound");
            }
            catch
            {
                return Problem();
            }
        }
    }
}

