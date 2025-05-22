using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly OrderService _orderService;

        public OrdersController(ApplicationDbContext context, OrderService orderService)
        {
            _context = context;
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

        // POST: api/Orders
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

        // PUT: api/Orders/5
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

        // DELETE: api/Orders/5
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

                // Validate pagination parameters
                if (pageNumber < 1) return BadRequest("Page number must be at least 1");
                if (pageSize < 1 || pageSize > 100) return BadRequest("Page size must be between 1 and 100");
                DateTime? OrderDate =
                DateTime.TryParse(query.OrderDateString, out var date)? DateTime.SpecifyKind(date.Date, DateTimeKind.Utc) : null;
                DateTime? CompletionDate = DateTime.TryParse(query.CompletionDateString, out var date1) ? DateTime.SpecifyKind(date1.Date, DateTimeKind.Utc): null;
            // Validate at least one filter is provided
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
            catch (Exception ex)
            {
                return StatusCode(500, "Error retrieving urgent orders");
            }
        }
    }
    }
