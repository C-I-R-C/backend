using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;

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