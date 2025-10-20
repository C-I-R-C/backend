using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;

namespace WebApplication1
{
    public class OrderFlowerValidationResult
    {
        public int OrderId { get; set; }
        public bool IsValid { get; set; }
        public List<FlowerStockStatus>? FlowerStatuses { get; set; }
    }
}