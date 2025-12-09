using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Net;



namespace WebApplication1.Filters
{

    public class GlobalExceptionFilter : IExceptionFilter
    {

        private readonly ILogger<GlobalExceptionFilter> _logger;
        private readonly IWebHostEnvironment _environment;


        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, IWebHostEnvironment environment)
        {

            _logger = logger;
            _environment = environment;

        }


        public void OnException(ExceptionContext context)
        {

            _logger.LogError(context.Exception, "An unhandled exception occurred");
            var (statusCode, errorType, message) = MapException(context.Exception);
            var response = new ApiErrorResponse(errorType, message);


            if (_environment.IsDevelopment())
            {

                response.Message = $"{message} | Details: {context.Exception.Message}";

            }


            context.Result = new ObjectResult(response)
            {

                StatusCode = (int)statusCode

            };


            context.ExceptionHandled = true;

        }


        private (HttpStatusCode statusCode, string errorType, string message) MapException(Exception exception)
        {

            return exception switch
            {

                KeyNotFoundException => 
                    (HttpStatusCode.NotFound, 
                        "NOT_FOUND", "Resource not found"),


                ArgumentException => 
                    (HttpStatusCode.BadRequest, 
                        "VALIDATION_ERROR", "Invalid request parameters"),


                InvalidOperationException => 
                    (HttpStatusCode.BadRequest, 
                        "BUSINESS_RULE_ERROR", exception.Message),


                DbUpdateConcurrencyException => 
                    (HttpStatusCode.Conflict, 
                        "CONCURRENCY_ERROR", "The resource was modified by another request"),
               

                DbUpdateException => 
                    (HttpStatusCode.BadRequest, 
                        "DATABASE_ERROR", "Error saving data"),
                

                NotImplementedException => 
                    (HttpStatusCode.NotImplemented, 
                        "NOT_IMPLEMENTED", "Feature not implemented"),
                

                UnauthorizedAccessException => 
                    (HttpStatusCode.Unauthorized, 
                        "UNAUTHORIZED", "Access denied"),


                _ => 
                    (HttpStatusCode.InternalServerError, 
                        "INTERNAL_ERROR", "An unexpected error occurred")

            };

        }


    }



}