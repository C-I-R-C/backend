using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
namespace WebApplication1
{
    public class RoleBasedAuthFilter : IAsyncAuthorizationFilter
    {
        //private readonly string _requiredRole;

        //public RoleBasedAuthFilter(string requiredRole) => _requiredRole = requiredRole;

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
        //    var user = context.HttpContext.User;
        //    if (!user.IsInRole(_requiredRole))
        //    {
        //        context.Result = new ForbidResult();
        //    }
        }
    }

    // Usage:
    public class CacheResponseFilter : IAsyncActionFilter
    {
        //private readonly IMemoryCache _cache;
        //private readonly TimeSpan _duration;

        //public CacheResponseFilter(IMemoryCache cache, TimeSpan duration)
        //{
        //    _cache = cache;
        //    _duration = duration;
        //}

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //var cacheKey = GenerateCacheKey(context.HttpContext.Request);

            //if (_cache.TryGetValue(cacheKey, out object cachedValue))
            //{
            //    context.Result = new OkObjectResult(cachedValue);
            //    return;
            //}

            //var executedContext = await next();

            //if (executedContext.Result is OkObjectResult okResult)
            //{
            //    _cache.Set(cacheKey, okResult.Value, _duration);
            //}
        }
    }
    public class StockAvailabilityFilter : IAsyncActionFilter
    {
        //private readonly FlowerShopContext _db;

        //public StockAvailabilityFilter(FlowerShopContext db) => _db = db;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //var order = context.ActionArguments["request"] as OrderCreateDto;

            //var stockIssues = await CheckStockAvailability(order.Items);
            //if (stockIssues.Any())
            //{
            //    context.Result = new ConflictObjectResult(new
            //    {
            //        Message = "Insufficient stock",
            //        Details = stockIssues
            //    });
            //    return;
            //}

            //await next();
        }
    }
    public class ETagFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //var request = context.HttpContext.Request;
            //if (request.Method == HttpMethod.Put.Method)
            //{
            //    if (!request.Headers.ContainsKey("If-Match"))
            //    {
            //        context.Result = new BadRequestObjectResult("If-Match header required");
            //        return;
            //    }
            //}

            //await next();

            //var response = context.HttpContext.Response;
            //if (request.Method == HttpMethod.Get.Method && response.StatusCode == 200)
            //{
            //    var etag = GenerateETag(context.Result);
            //    response.Headers.ETag = etag;
            //}
        }
    }
    public class TimingFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //var sw = Stopwatch.StartNew();

            //await next();

            //sw.Stop();
            //context.HttpContext.Response.Headers.Add("X-Execution-Time", sw.ElapsedMilliseconds.ToString());
        }
    }
}
