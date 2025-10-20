using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;

namespace WebApplication1
{
    public class UrgentOrderDto
    {
        public int OrderId { get; set; }
        public string? ClientName { get; set; }
        public DateTime CompletionDate { get; set; }
        public TimeSpan TimeUntilDue { get; set; }
        public int ItemsCount { get; set; }
        public decimal TotalPrice { get; set; }

        public string UrgencyLevel => TimeUntilDue.TotalHours switch
        {
            < 24 => "Critical",
            < 72 => "High",
            _ => "Normal"
        };
    }
}