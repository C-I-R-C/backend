using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Client
    {

        public int Id { get; set; }


        public string? Name { get; set; }


        public string? PhoneNumber { get; set; }


        public int TotalOrdersCount { get; set; }


        public int DiscountLevel { get; set; }



        public ICollection<Order> Orders { get; set; } = [];

    }


    public class Order
    {

        public int Id { get; set; }


        public int ClientId { get; set; }


        public Client? Client { get; set; }


        [Column(TypeName = "timestamp with time zone")]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;


        [Column(TypeName = "timestamp with time zone")]
        public DateTime OrderCompleteDate { get; set; }


        public ICollection<OrderItem> OrderItems { get; set; } = [];


        public string? Comment { get; set; }


        public decimal TotalPrice { get; set; } 


        public bool IsCurrent { get; set; } 

    }


    public class OrderItem
    {
        public int Id { get; set; }


        public int OrderId { get; set; }


        public int ItemId { get; set; }


        public int Quantity { get; set; }


        public decimal UnitPrice { get; set; }



        public Order? Order { get; set; }


        public Item? Item { get; set; }

    }


    public class Item
    {

        public int Id { get; set; }


        public string? Name { get; set; }


        public decimal BasePrice { get; set; }


        public ICollection<ItemFlower> ItemFlowers { get; set; } = [];


        public int? BoxId { get; set; }



        public Box? Box { get; set; }

    }


    public class ItemFlower
    {

        public int ItemId { get; set; }


        public int FlowerId { get; set; }


        public int Quantity { get; set; }



        public Item? Item { get; set; }


        public Flower? Flower { get; set; }


    }


    public class Flower
    {

        public int Id { get; set; }


        public string? Name { get; set; }


        public int InStock { get; set; }


        public decimal CostPerUnit { get; set; }


        public ICollection<FlowerIngredient> FlowerIngredients { get; set; } = [];


        public int? ColorId { get; set; }



        public Color? Color { get; set; }

    }


    public class Box
    {

        public int Id { get; set; }


        public string? Name { get; set; }


        public int InStock { get; set; }


        public decimal CostPerUnit { get; set; }


    }


    public class Ingredient
    {

        public int Id { get; set; }


        public string? Name { get; set; }


        public int InStock { get; set; }


        public decimal CostPerUnit { get; set; }

    }


    public class FlowerIngredient
    {

        public int FlowerId { get; set; }


        public int IngredientId { get; set; }


        public int QuantityRequired { get; set; }



        public Flower? Flower { get; set; }


        public Ingredient? Ingredient { get; set; }

    }


    public class Color
    {

        public int Id { get; set; }
        
        
        public string? Name { get; set; }
        

        public bool IsNatural { get; set; } 

    }


    public class Image
    {

        public int Id { get; set; }


        public string? FileName { get; set; }


        public string? Url { get; set; }


        public byte[]? Data { get; set; }


        public string? ContentType { get; set; }


        public string? EntityType { get; set; }


        public int EntityId { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }

}
