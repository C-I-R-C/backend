using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(TimingFilter))]
    [ServiceFilter(typeof(LogActionFilter))]
    public class UniversalController<T> : ControllerBase where T : class
    {
        [HttpGet]
        //[ServiceFilter(typeof(CacheResponseFilter), Arguments = new object[] { "30" })] 
        public ActionResult<IEnumerable<T>> GetAll() => throw new NotImplementedException();

        [HttpGet("{id}")]
        [ServiceFilter(typeof(ETagFilter))]
        public ActionResult<T> GetById(int id) => throw new NotImplementedException();

        [HttpPost]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public virtual ActionResult<T> Add([FromBody] T entity) => throw new NotImplementedException();

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        [ServiceFilter(typeof(ConcurrencyCheckFilter))]
        public IActionResult Update(int id, [FromBody] T entity) => throw new NotImplementedException();

        [HttpDelete("{id}")]
        //[ServiceFilter(typeof(RoleBasedAuthFilter), Arguments = new object[] { "Admin" })]
        public IActionResult Delete(int id) => throw new NotImplementedException();
    }
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : UniversalController<Client>
    {
            [HttpGet("by-number/{phoneNumber}")]
            [ServiceFilter(typeof(ClientPhoneNumberFormatFilter))]
            public ActionResult<Client> GetClientByPhoneNumber(string phoneNumber) => throw new NotImplementedException();

            [HttpGet("by-name/{name}")]
            //[ServiceFilter(typeof(CacheResponseFilter), Arguments = new object[] { "60" })] // Cache for 1 minute
            public ActionResult<Client> GetClientByName(string name) => throw new NotImplementedException();
    }
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : UniversalController<Order>
        {
            [HttpPost("add order")]
            [ServiceFilter(typeof(OrderPriorityFilter))]
            [ServiceFilter(typeof(StockAvailabilityFilter))] 
            public override ActionResult<Order> Add([FromBody] Order entity) => base.Add(entity);

            [HttpGet("most-urgent")]
            //[ServiceFilter(typeof(RoleBasedAuthFilter), Arguments = new object[] { "Manager" })]
            public ActionResult<IEnumerable<Order>> GetMostUrgentOrders() => throw new NotImplementedException();
        }
    [Route("api/[controller]")]
    [ApiController]
    public class FlowerController : UniversalController<Flower>
        {
            [HttpPatch("{id}/increase-stock")]
            [ServiceFilter(typeof(StockModificationFilter))]
            [ServiceFilter(typeof(ValidateModelAttribute))]
            public ActionResult<Flower> IncreaseStock(int id, [FromBody] int amountToAdd) => throw new NotImplementedException();

            [HttpGet("{flowerId}/check-stock")]
            //[ServiceFilter(typeof(CacheResponseFilter), Arguments = new object[] { "5" })] // Short cache
           
        [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(TimingFilter))]
    [ServiceFilter(typeof(LogActionFilter))]
    public class OrderItemsController : UniversalController<OrderItem>
    {
        [HttpPost("post oderItem")]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        [ServiceFilter(typeof(OrderItemValidationFilter))]
        public ActionResult<OrderItem> Add([FromBody] OrderItem entity)
        {
            throw new NotImplementedException();
        }
    }
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(TimingFilter))]
    [ServiceFilter(typeof(LogActionFilter))]
    public class ItemsController : UniversalController<Item>
    {
        [HttpPost("post Item")]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        [ServiceFilter(typeof(ItemCompositionValidationFilter))]
        public override ActionResult<Item> Add([FromBody] Item entity)
        {
            throw new NotImplementedException();
        }

        [HttpPatch("{id}/update-price")]
        [ServiceFilter(typeof(PriceUpdateAuthorizationFilter))]
        public ActionResult<Item> UpdatePrice(int id, [FromBody] decimal newPrice)
        {
            throw new NotImplementedException();
        }
    }
    }


