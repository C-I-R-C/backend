using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;

namespace WebApplication1
{
    public class FlowerIngredient1Dto
    {
        public int IngredientId { get; set; }
        public string? Name { get; set; }
        public int QuantityRequired { get; set; }
        public decimal CostPerUnit { get; set; }
    }
}