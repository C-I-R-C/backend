using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;

namespace WebApplication1
{
    public class OrderCreateDto
    {
        public int ClientId { get; set; }
        public DateTime? OrderCompleteDate { get; set; }
        public string? Comment { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}