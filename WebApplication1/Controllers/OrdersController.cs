using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1;
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

        // PATCH: api/Orders/5/complete
        [HttpPatch("{id}/complete")]
        public async Task<IActionResult> MarkAsCompleted(int id)
        {
            try
            {
                await _orderService.MarkAsCompleted(id);
                return Ok();
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
    }
}