using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;

namespace WebApplication1
{
    public class ItemDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal BasePrice { get; set; }
    }

    public class ItemResponseDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal BasePrice { get; set; }
        public BoxDto? Box { get; set; }
        public List<ItemFlowerDetailDto> Flowers { get; set; } = new();
        public decimal ProductionCost { get; set; }
    }

    public class ItemCreateDto
    {
        public string? Name { get; set; }
        public decimal BasePrice { get; set; }
        public int BoxId { get; set; }
    }

    public class ItemUpdateDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal BasePrice { get; set; }
    }
    public class ItemFlowerDto
    {
        public int ItemId { get; set; }
        public int FlowerId { get; set; }
        public int Quantity { get; set; }
        public FlowerDto? Flower { get; set; }
    }

    public class ItemFlowerDetailDto
    {
        public int FlowerId { get; set; }
        public string? FlowerName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public string? Color { get; set; }
        public List<FlowerIngredient1Dto> Ingredients { get; set; } = new();
    }

    public class ItemFlowerUsageDto
    {
        public int ItemId { get; set; }
        public string? ItemName { get; set; }
        public int QuantityInItem { get; set; }
        public int ItemQuantity { get; set; }
    }

    public class AddFlowerToItemDto
    {
        [Range(1, int.MaxValue)]
        public int FlowerId { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }
    }

    public class UpdateFlowerQuantityDto
    {
        [Range(1, 100)]
        public int Quantity { get; set; }
    }
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