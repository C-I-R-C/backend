using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;

namespace WebApplication1
{
    public class AddIngredientToFlowerDto
    {
        [Range(1, int.MaxValue)]
        public int IngredientId { get; set; }

        [Range(1, 100)]
        public int QuantityRequired { get; set; }
    }
    public class ClientWithOrdersDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public int TotalOrdersCount { get; set; }
        public int DiscountLevel { get; set; }

        public List<OrderSummaryDto> Orders { get; set; } = new();
    }
    public class ClientResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public int TotalOrdersCount { get; set; }
        public int DiscountLevel { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime? LastOrderDate { get; set; }
    }

    public class ClientCreateDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        public int DiscountLevel { get; set; } = 0;
    }

    public class ClientUpdateDto
    {
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public int? DiscountLevel { get; set; }
    }


    public class OrderSummaryDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsCurrent { get; set; }
    }
    public class OrderWithClientDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsCurrent { get; set; }
        public string Comment { get; set; }

        public ClientInfoDto Client { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class ClientInfoDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DiscountLevel { get; set; }
    }
    public class OrderCreateDto
    {
        public int ClientId { get; set; }
        public string Comment { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
    }
    public class OrderItemResponseDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public ItemDto Item { get; set; }
    }

    public class ItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal BasePrice { get; set; }
    }
    //public class OrderResponseDto
    //{
    //    public int Id { get; set; }
    //    public DateTime OrderDate { get; set; }
    //    public decimal TotalPrice { get; set; }
    //    public bool IsCurrent { get; set; }
    //    public string Comment { get; set; }
    //    public ClientInfoDto Client { get; set; }
    //    public List<OrderItemResponseDto> Items { get; set; } = new();
    //}

    public class OrderFlowersResponseDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string ClientName { get; set; }
        public List<FlowerUsageDto> Flowers { get; set; } = new();
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

    public class ItemFlowerUsageDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int QuantityInItem { get; set; } // How many flowers per item
        public int ItemQuantity { get; set; } // How many items ordered
    }
    public class OrderItemFlowersResponseDto
    {
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int ItemQuantity { get; set; }
        public List<FlowerDetailDto> Flowers { get; set; } = new();
    }

    //public class FlowerDetailDto
    //{
    //    public int FlowerId { get; set; }
    //    public string FlowerName { get; set; }
    //    public int QuantityPerItem { get; set; }
    //    public int TotalQuantity { get; set; }
    //    public decimal UnitCost { get; set; }
    //    public string Color { get; set; }
    //}
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

    // Dtos/ItemFlowerDto.cs
    public class ItemFlowerDto
    {
        public int ItemId { get; set; }
        public int FlowerId { get; set; }
        public int Quantity { get; set; }
        public FlowerDto Flower { get; set; } // Include full flower details
    }

    // Dtos/ColorDto.cs
    public class ColorDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsNatural { get; set; }
    }

    // Dtos/ColorCreateDto.cs
    public class ColorCreateDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public bool IsNatural { get; set; }
    }

    // Dtos/ColorUpdateDto.cs
    public class ColorUpdateDto
    {
        [StringLength(50)]
        public string Name { get; set; }

        public bool? IsNatural { get; set; }
    }
    //public class IngredientDto
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //    public int InStock { get; set; }
    //    public decimal CostPerUnit { get; set; }
    //}

    // Dtos/IngredientCreateDto.cs
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

    // Dtos/FlowerIngredientDto.cs
    public class FlowerIngredientDto
    {
        public int FlowerId { get; set; }
        public int IngredientId { get; set; }
        public int QuantityRequired { get; set; }
        public IngredientDto Ingredient { get; set; }
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

    public class OrderUpdateDto
    {
        public string Comment { get; set; }
        public bool? IsCurrent { get; set; }
    }


    //public class OrderItemWithDetailsDto
    //{
    //    public int Id { get; set; }
    //    public int Quantity { get; set; }
    //    public decimal UnitPrice { get; set; }
    //    public ItemDto Item { get; set; }
    //    public List<FlowerDetailDto> Flowers { get; set; } = new();
    //}
    //public class OrderCreateDto
    //{
    //    [Required]
    //    [Range(1, int.MaxValue)]
    //    public int ClientId { get; set; }

    //    public string Comment { get; set; }

    //    [Required]
    //    [MinLength(1)]
    //    public List<OrderItemCreateDto> Items { get; set; } = new();
    //}

    //public class OrderUpdateDto
    //{
    //    public string Comment { get; set; }
    //    public bool? IsCurrent { get; set; }
    //}

    public class OrderResponseDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsCurrent { get; set; }
        public string Comment { get; set; }
        public ClientInfoDto Client { get; set; }
        public List<OrderItemWithDetailsDto> Items { get; set; } = new();
    }

    public class OrderItemWithDetailsDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public ItemDto Item { get; set; }
        public List<FlowerDetailDto> Flowers { get; set; } = new();
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
    public class BoxDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int InStock { get; set; }
    }

    // Dtos/BoxCreateDto.cs
    public class BoxCreateDto
    {
        [Required(ErrorMessage = "Box name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int InStock { get; set; } = 0;
    }

    // Dtos/BoxUpdateDto.cs
    public class BoxUpdateDto
    {
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int? InStock { get; set; }
    }
  

    public class OrderItemCreateDto
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
    }


    public class IngredientDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int InStock { get; set; }
        public decimal CostPerUnit { get; set; }
    }
    public class ItemResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal BasePrice { get; set; }

        // Box information (if exists)
        public BoxDto Box { get; set; }

        // Flowers used in this item
        public List<ItemFlowerDetailDto> Flowers { get; set; } = new();

        // Calculated total cost based on components
        public decimal ProductionCost { get; set; }
    }

    //public class BoxDto
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //    public int InStock { get; set; }
    //}

    public class ItemFlowerDetailDto
    {
        public int FlowerId { get; set; }
        public string FlowerName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public string Color { get; set; }
        public List<FlowerIngredient1Dto> Ingredients { get; set; } = new();
    }

    public class FlowerIngredient1Dto
    {
        public int IngredientId { get; set; }
        public string Name { get; set; }
        public int QuantityRequired { get; set; }
        public decimal CostPerUnit { get; set; }
    }
    public class ItemCreateDto
    {
        public string Name { get; set; }
        public decimal BasePrice { get; set; }
        public int BoxId { get; set; }
    }

    public class ItemUpdateDto
    {
        public string Name { get; set; }
        public decimal BasePrice { get; set; }
    }
}
