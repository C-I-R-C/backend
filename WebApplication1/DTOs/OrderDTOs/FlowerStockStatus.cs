using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;

namespace WebApplication1
{
    public class FlowerStockStatus
    {
        public int FlowerId { get; set; }
        public string? FlowerName { get; set; }
        public int RequiredQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public bool IsAvailable { get; set; }
    }
}