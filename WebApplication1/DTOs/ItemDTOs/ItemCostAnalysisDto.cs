using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;

namespace WebApplication1
{
    public class ItemCostAnalysisDto
    {
        public decimal BasePrice { get; set; }
        public decimal TotalComponentsCost { get; set; }
        public decimal FlowersCost { get; set; }
        public decimal BoxCost { get; set; }
        public decimal IngredientsCost { get; set; }
        public decimal LaborCost { get; set; }
        public decimal Profit { get; set; }
        public decimal ProfitMargin { get; set; }
    }
}