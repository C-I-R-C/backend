using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;

namespace WebApplication1
{
    public class FlowerIngredientDtoNew
    {
        public int IngredientId { get; set; }
        public string? IngredientName { get; set; }
        public int QuantityRequired { get; set; }
        public decimal CostPerUnit { get; set; }
        public decimal TotalCost { get; set; }
    }
}