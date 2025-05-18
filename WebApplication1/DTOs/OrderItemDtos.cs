using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;

namespace WebApplication1
{
    public class OrderItemDto
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderItemResponseDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public ItemDto Item { get; set; }
    }

    public class OrderItemWithDetailsDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public ItemDto Item { get; set; }
        public List<FlowerDetailDto> Flowers { get; set; } = new();
    }

    public class OrderItemFlowersResponseDto
    {
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int ItemQuantity { get; set; }
        public List<FlowerDetailDto> Flowers { get; set; } = new();
    }

    public class OrderItemCreateDto
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
    }
}