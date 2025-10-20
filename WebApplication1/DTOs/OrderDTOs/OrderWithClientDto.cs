using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;

namespace WebApplication1
{
    public class OrderWithClientDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsCurrent { get; set; }
        public string? Comment { get; set; }
        public ClientInfoDto? Client { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}