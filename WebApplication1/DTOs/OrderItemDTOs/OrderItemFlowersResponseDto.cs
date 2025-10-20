using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;

namespace WebApplication1
{
    public class OrderItemFlowersResponseDto
    {
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public string? ItemName { get; set; }
        public int ItemQuantity { get; set; }
        public List<FlowerDetailDto> Flowers { get; set; } = new();
    }
}