using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/orders/{orderId}/items/{itemId}/flowers")]
    public class OrderItemFlowersController : ControllerBase
    {
        private readonly DataService _data;

        public OrderItemFlowersController(DataService data)
        {
            _data = data;
        }

        // GET api/orders/5/items/1/flowers
        [HttpGet]
        public ActionResult<OrderItemFlowersResponseDto> GetFlowersForOrderItem(
            int orderId, int itemId)
        {
            var order = _data.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null) return NotFound("Order not found");

            var orderItem = order.OrderItems.FirstOrDefault(oi => oi.ItemId == itemId);
            if (orderItem == null) return NotFound("Item not found in order");

            var item = _data.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null) return NotFound("Item not found");

            var result = new OrderItemFlowersResponseDto
            {
                OrderId = order.Id,
                ItemId = item.Id,
                ItemName = item.Name,
                ItemQuantity = orderItem.Quantity,
                Flowers = _data.ItemFlowers
                    .Where(itemf => itemf.ItemId == itemId)
                .Select(itemf => new FlowerDetailDto
                {
                    FlowerId = itemf.FlowerId,
                    FlowerName = _data.Flowers.FirstOrDefault(f => f.Id == itemf.FlowerId)?.Name ?? "Unknown",
                    QuantityPerItem = itemf.Quantity,
                    TotalQuantity = itemf.Quantity * orderItem.Quantity,
                    UnitCost = _data.Flowers.FirstOrDefault(f => f.Id == itemf.FlowerId)?.CostPerUnit ?? 0,
                    Color = _data.Flowers
                        .FirstOrDefault(f => f.Id == itemf.FlowerId)?
                        .Color?.Name ?? "N/A"
                })
                .ToList()
            };

            return Ok(result);
        }
    }
}
