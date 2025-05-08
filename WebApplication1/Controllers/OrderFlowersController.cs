using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/orders/{orderId}/flowers")]
    public class OrderFlowersController : ControllerBase
    {
        private readonly DataService _data;

        public OrderFlowersController(DataService data)
        {
            _data = data;
        }

        // GET api/orders/5/flowers
        [HttpGet]
        public ActionResult<OrderFlowersResponseDto> GetFlowersForOrder(int orderId)
        {
            var order = _data.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null) return NotFound("Order not found");

            var result = new OrderFlowersResponseDto
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                ClientName = _data.Clients.FirstOrDefault(c => c.Id == order.ClientId)?.Name,
                Flowers = order.OrderItems
                    .SelectMany(oi => _data.ItemFlowers
                        .Where(itemf => itemf.ItemId == oi.ItemId)
                    .Select(itemf => new FlowerUsageDto
                    {
                        FlowerId = itemf.FlowerId,
                        FlowerName = _data.Flowers.FirstOrDefault(f => f.Id == itemf.FlowerId)?.Name ?? "Unknown",
                        QuantityUsed = itemf.Quantity * oi.Quantity, // Total across all items
                        UnitCost = _data.Flowers.FirstOrDefault(f => f.Id == itemf.FlowerId)?.CostPerUnit ?? 0,
                        Color = _data.Flowers
                            .FirstOrDefault(f => f.Id == itemf.FlowerId)?
                            .Color?.Name ?? "N/A",
                        Items = new List<ItemFlowerUsageDto>
                        {
                            new ItemFlowerUsageDto
                            {
                                ItemId = oi.ItemId,
                                ItemName = _data.Items.FirstOrDefault(i => i.Id == oi.ItemId)?.Name,
                                QuantityInItem = itemf.Quantity,
                                ItemQuantity = oi.Quantity
                                }
    }
                    }))
                .GroupBy(f => f.FlowerId)
                .Select(g => new FlowerUsageDto
                {
                    FlowerId = g.Key,
                    FlowerName = g.First().FlowerName,
                    QuantityUsed = g.Sum(x => x.QuantityUsed),
                    UnitCost = g.First().UnitCost,
                    Color = g.First().Color,
                    Items = g.SelectMany(x => x.Items).ToList()
                })
                .ToList()
            };

            return Ok(result);
        }
    }
}
