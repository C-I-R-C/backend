using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Concurrent;

    namespace WebApplication1.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class UniversalController<T> : ControllerBase where T : class
        {
            private static readonly ConcurrentDictionary<int, T> _uni = new();
            [HttpGet]
            public ActionResult<IEnumerable<T>> GetAll() => throw new NotImplementedException();

            [HttpGet("{id}")]
            public ActionResult<T> GetById(int id)
            {
                if (!_uni.TryGetValue(id, out var order))
                {
                    return NotFound();
                }
                return Ok(order);
            }

            [HttpPost]
            public ActionResult<T> Add([FromBody] T entity) => throw new NotImplementedException();

            [HttpPut("{id}")]
            public IActionResult Update(int id, [FromBody] T entity) => throw new NotImplementedException();

            [HttpDelete("{id}")]
            public IActionResult Delete(int id)
            {
                if (!_uni.TryRemove(id, out _))
                {
                    return NotFound();
                }
                return NoContent();
            }

        }
        [Route("api/[controller]")]
        [ApiController]
        public class ClientsController : UniversalController<Client>
        {

            [HttpGet("by-number/{phoneNumber}")]
            public ActionResult<Client> GetClientByPhoneNumber(string phoneNumber)
            {
                throw new NotImplementedException();
            }
            [HttpGet("by-name/{name}")]
            public ActionResult<Client> GetClientByName(string name)
            {
                throw new NotImplementedException();
            }
        }
        [Route("api/[controller]")]
        [ApiController]

        public class OrdersController : UniversalController<Order>
        {
            [HttpGet("closest/biggest")]
            public ActionResult<Order> GetClosestBiggestOrder()
            {
                throw new NotImplementedException();
            }
            
            [HttpGet("most-urgent")]
            public ActionResult<IEnumerable<Order>> GetMostUrgentOrders()
            {
                throw new NotImplementedException("Endpoint for getting 5 most urgent orders");
            }

            [HttpGet("biggest")]
            public ActionResult<IEnumerable<Order>> GetBiggestOrders()
            {
                throw new NotImplementedException("Endpoint for getting 5 biggest orders by volume");
            }
           
        }
        [Route("api/[controller]")]
        [ApiController]
        public class OrderItemsController : UniversalController<OrderItem>
        {
        }
        [Route("api/[controller]")]
        [ApiController]
        public class ItemsController : UniversalController<Item>
        {}
        [Route("api/[controller]")]
        [ApiController]
        public class FlowerController : UniversalController<Flower>
        {
        [HttpPatch("{id}/increase-stock")]
        public ActionResult<Flower> IncreaseStock(int id, [FromBody] int amountToAdd) => throw new NotImplementedException();
        [HttpGet("{FlowerId}/check-stock")]
        public ActionResult<StockCheckResult> CheckStock(int FlowerId, [FromQuery] int requiredAmount) => throw new NotImplementedException();
        }
        public class BoxController : UniversalController<Box>
        {
            [HttpPatch("{id}/increase-stock")]
            public ActionResult<Box> IncreaseStock(int id, [FromBody] int amountToAdd) => throw new NotImplementedException();
            [HttpGet("{BoxId}/check-stock")]
            public ActionResult<Box> CheckStock(int BoxId, [FromQuery] int requiredAmount) => throw new NotImplementedException();
        }
        public class IngreController : UniversalController<Ingredient>
        {
            [HttpPatch("{id}/increase-stock")]
            public ActionResult<Box> IncreaseStock(int id, [FromBody] int amountToAdd) => throw new NotImplementedException();
            [HttpGet("{BoxId}/check-stock")]
            public ActionResult<Box> CheckStock(int BoxId, [FromQuery] int requiredAmount) => throw new NotImplementedException();
        }
        public class ColorController : UniversalController<Color>
        {
            [HttpPatch("{id}/increase-stock")]
            public ActionResult<Box> IncreaseStock(int id, [FromBody] int amountToAdd) => throw new NotImplementedException();
            [HttpGet("{BoxId}/check-stock")]
            public ActionResult<Box> CheckStock(int BoxId, [FromQuery] int requiredAmount) => throw new NotImplementedException();
        }
    }
}

