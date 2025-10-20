using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;

namespace WebApplication1
{
    public class FlowerDetailDto
    {
        public int FlowerId { get; set; }
        public string? FlowerName { get; set; }
        public int QuantityPerItem { get; set; }
        public int TotalQuantity { get; set; }
        public decimal UnitCost { get; set; }
        public string? Color { get; set; }
        public List<IngredientDto> Ingredients { get; set; } = new();
    }
}