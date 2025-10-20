using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;

namespace WebApplication1
{
    public class ItemFlowerUsageDto
    {
        public int ItemId { get; set; }
        public string? ItemName { get; set; }
        public int QuantityInItem { get; set; }
        public int ItemQuantity { get; set; }
    }
}