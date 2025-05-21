using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;

namespace WebApplication1
{
    public class IngredientCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Range(0, int.MaxValue)]
        public int InStock { get; set; }

        [Range(0.01, 1000)]
        public decimal CostPerUnit { get; set; }
    }

    public class IngredientDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int InStock { get; set; }
        public decimal CostPerUnit { get; set; }
    }

    public class FlowerIngredientDto
    {
        public int FlowerId { get; set; }
        public int IngredientId { get; set; }
        public int QuantityRequired { get; set; }
        public IngredientDto Ingredient { get; set; }
    }

    public class FlowerIngredient1Dto
    {
        public int IngredientId { get; set; }
        public string Name { get; set; }
        public int QuantityRequired { get; set; }
        public decimal CostPerUnit { get; set; }
    }
    public class FlowerIngredientDtoNew
    {
        public int IngredientId { get; set; }
        public string IngredientName { get; set; }
        public decimal QuantityRequired { get; set; }
        public decimal CostPerUnit { get; set; }
        public decimal TotalCost { get; set; }
    }

}