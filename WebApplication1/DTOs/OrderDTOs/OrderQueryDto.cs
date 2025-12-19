using Microsoft.AspNetCore.Mvc;

namespace WebApplication1
{

    public class OrderQueryDto
    {

        [FromQuery(Name = "orderDate")]
        public string? OrderDateString { get; set; }


        [FromQuery(Name = "completionDate")]
        public string? CompletionDateString { get; set; }


        public bool? IsCompleted { get; set; }

    }

}
