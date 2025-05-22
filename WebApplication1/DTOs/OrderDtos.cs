using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;

namespace WebApplication1
{
    public class OrderSummaryDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsCurrent { get; set; }
    }

    public class OrderWithClientDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsCurrent { get; set; }
        public string Comment { get; set; }
        public ClientInfoDto Client { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderCreateDto
    {
        public int ClientId { get; set; }
        public DateTime? OrderCompleteDate { get; set; }
        public string Comment { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderUpdateDto
    {
        public string Comment { get; set; }
        public bool? IsCurrent { get; set; }
        public DateTime? OrderCompleteDate { get; set; }
    }

    public class OrderResponseDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime OrderCompleteDate { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsCurrent { get; set; }
        public string Comment { get; set; }
        public ClientInfoDto Client { get; set; }
        public List<OrderItemWithDetailsDto> Items { get; set; } = new();
        public TimeSpan TimeUntilDue { get; set; }
        public bool IsUrgent => TimeUntilDue.TotalHours < 24;
    }

    public class OrderFlowersResponseDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string ClientName { get; set; }
        public List<FlowerUsageDto> Flowers { get; set; } = new();
    }
    public class OrderQueryDto
    {
        [FromQuery(Name = "orderDate")]
        public string? OrderDateString { get; set; }

        [FromQuery(Name = "completionDate")]
        public string? CompletionDateString { get; set; }

        public bool? IsCompleted { get; set; }

    }
    public class UrgentOrderDto
    {
        public int OrderId { get; set; }
        public string ClientName { get; set; }
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