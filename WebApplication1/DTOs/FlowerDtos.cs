using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;

namespace WebApplication1
{
    public class FlowerCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Range(0, int.MaxValue)]
        public int InStock { get; set; }

        [Range(0.01, 1000)]
        public decimal CostPerUnit { get; set; }

        public int? ColorId { get; set; }
    }

    public class FlowerUpdateDto
    {
        [StringLength(100)]
        public string Name { get; set; }

        [Range(0, int.MaxValue)]
        public int InStock { get; set; }

        [Range(0.01, 1000)]
        public decimal CostPerUnit { get; set; }

        public int? ColorId { get; set; }
    }

    public class FlowerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int InStock { get; set; }
        public decimal CostPerUnit { get; set; }
        public ColorDto Color { get; set; }
    }

    public class FlowerWithIngredientsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int InStock { get; set; }
        public decimal CostPerUnit { get; set; }
        public ColorDto Color { get; set; }
        public List<FlowerIngredientDto> Ingredients { get; set; } = new();
    }

    public class FlowerUsageDto
    {
        public int FlowerId { get; set; }
        public string FlowerName { get; set; }
        public int QuantityUsed { get; set; }
        public decimal UnitCost { get; set; }
        public string Color { get; set; }
        public List<ItemFlowerUsageDto> Items { get; set; } = new();
    }

    public class FlowerDetailDto
    {
        public int FlowerId { get; set; }
        public string FlowerName { get; set; }
        public int QuantityPerItem { get; set; }
        public int TotalQuantity { get; set; }
        public decimal UnitCost { get; set; }
        public string Color { get; set; }
        public List<IngredientDto> Ingredients { get; set; } = new();
    }

    public class AddIngredientToFlowerDto
    {
        [Range(1, int.MaxValue)]
        public int IngredientId { get; set; }

        [Range(1, 100)]
        public int QuantityRequired { get; set; }
    }
    public class FlowerQuantityUpdateDto
    {
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a positive number")]
        public int Quantity { get; set; }

        [Required]
        public bool IsIncrement { get; set; } // True to add, false to subtract
    }
    public class FlowerWithIngredientsDtoNew
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int InStock { get; set; }
        public decimal CostPerUnit { get; set; }
        public ColorDto Color { get; set; }
        public List<FlowerIngredientDtoNew> Ingredients { get; set; } = new();
    }
}