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
        [HttpGet("{id}/profit")]
        public async Task<ActionResult<OrderProfitDto>> GetOrderProfit(int id)
        {
            try
            {
                var profit = await _orderService.CalculateOrderProfit(id);
                return Ok(profit);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error calculating profit");
            }
        }

        [HttpGet("validate-order/{orderId}")]
        public async Task<ActionResult<OrderValidationResult>> ValidateOrderMaterials(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Item)
                            .ThenInclude(i => i.ItemFlowers)
                                .ThenInclude(itemf => itemf.Flower)
                            .ThenInclude(f => f.FlowerIngredients)
                                .ThenInclude(fi => fi.Ingredient)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Item)
                    .ThenInclude(i => i.Box)
            .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                    return NotFound($"Order with ID {orderId} not found");

                var result = new OrderValidationResult
                {
                    OrderId = orderId,
                    IsValid = true,
                    FlowerStatuses = new List<MaterialStatus>(),
                    IngredientStatuses = new List<MaterialStatus>(),
                    BoxStatuses = new List<MaterialStatus>()
                };

                var (flowerRequirements, ingredientRequirements, boxRequirements) = CalculateRequirements(order);

                result = await ValidateAllMaterialsAsync(
                    flowerRequirements,
                    ingredientRequirements,
                    boxRequirements,
                    result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while validating order materials");
            }
        }

        private (Dictionary<int, int> flowers,
                 Dictionary<int, int> ingredients,
                 Dictionary<int, int> boxes)
            CalculateRequirements(Order order)
        {
            var flowerRequirements = new Dictionary<int, int>();
            var ingredientRequirements = new Dictionary<int, int>();
            var boxRequirements = new Dictionary<int, int>();

            foreach (var orderItem in order.OrderItems)
            {
                foreach (var itemFlower in orderItem.Item.ItemFlowers)
                {
                    var totalFlowersNeeded = itemFlower.Quantity * orderItem.Quantity;
                    AddToDictionary(flowerRequirements, itemFlower.FlowerId, totalFlowersNeeded);

                    foreach (var flowerIngredient in itemFlower.Flower.FlowerIngredients)
                    {
                        var totalIngredientsNeeded = flowerIngredient.QuantityRequired *
                                                   itemFlower.Quantity *
                                                   orderItem.Quantity;
                        AddToDictionary(ingredientRequirements, flowerIngredient.IngredientId, totalIngredientsNeeded);
                    }
                }

                if (orderItem.Item.BoxId.HasValue)
                {
                    AddToDictionary(boxRequirements, orderItem.Item.BoxId.Value, orderItem.Quantity);
                }
            }

            return (flowerRequirements, ingredientRequirements, boxRequirements);
        }

        private async Task<OrderValidationResult> ValidateAllMaterialsAsync(
            Dictionary<int, int> flowerRequirements,
            Dictionary<int, int> ingredientRequirements,
            Dictionary<int, int> boxRequirements,
            OrderValidationResult result)
        {
            // Validate flowers
            var flowerValidation = await ValidateMaterialsAsync(
                flowerRequirements,
                _context.Flowers,
                f => f.Id,
                f => f.Name,
                f => f.InStock);

            result.FlowerStatuses = flowerValidation.statuses;
            if (!flowerValidation.allAvailable) result.IsValid = false;

            // Validate ingredients
            var ingredientValidation = await ValidateMaterialsAsync(
                ingredientRequirements,
                _context.Ingredients,
                i => i.Id,
                i => i.Name,
                i => i.InStock);

            result.IngredientStatuses = ingredientValidation.statuses;
            if (!ingredientValidation.allAvailable) result.IsValid = false;

            // Validate boxes
            var boxValidation = await ValidateMaterialsAsync(
                boxRequirements,
                _context.Boxes,
                b => b.Id,
                b => b.Name,
                b => b.InStock);

            result.BoxStatuses = boxValidation.statuses;
            if (!boxValidation.allAvailable) result.IsValid = false;

            return result;
        }

        private async Task<(List<MaterialStatus> statuses, bool allAvailable)> ValidateMaterialsAsync<T>(
            Dictionary<int, int> requirements,
            DbSet<T> dbSet,
            Func<T, int> idSelector,
            Func<T, string> nameSelector,
            Func<T, int> stockSelector) where T : class
        {
            var statuses = new List<MaterialStatus>();
            bool allAvailable = true;

            foreach (var req in requirements)
            {
                var material = await dbSet.FindAsync(req.Key);
                if (material == null)
                {
                    statuses.Add(new MaterialStatus
                    {
                        MaterialId = req.Key,
                        MaterialName = "Unknown",
                        RequiredQuantity = req.Value,
                        AvailableQuantity = 0,
                        IsAvailable = false
                    });
                    allAvailable = false;
                    continue;
                }

                bool isAvailable = stockSelector(material) >= req.Value;
                if (!isAvailable) allAvailable = false;

                statuses.Add(new MaterialStatus
                {
                    MaterialId = idSelector(material),
                    MaterialName = nameSelector(material),
                    RequiredQuantity = req.Value,
                    AvailableQuantity = stockSelector(material),
                    IsAvailable = isAvailable
                });
            }

            return (statuses, allAvailable);
        }
        private void AddToDictionary(Dictionary<int, int> dict, int id, int quantity)
        {
            if (dict.ContainsKey(id))
            {
                dict[id] += quantity;
            }
            else
            {
                dict.Add(id, quantity);
            }
        }
    }
}

