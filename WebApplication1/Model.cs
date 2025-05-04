using System.Drawing;

namespace WebApplication1
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public int TotalOrdersCount { get; set; }
        public int DiscountLevel { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }

    public class Order
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public string Comment { get; set; }
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

        public Order Order { get; set; }
        public Item Item { get; set; }
    }
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal BasePrice { get; set; }

        public ICollection<ItemFlower> ItemFlowers { get; set; } = new List<ItemFlower>();

        public int? BoxId { get; set; }
        public Box Box { get; set; }
    }
    public class ItemFlower
    {
        public int ItemId { get; set; }
        public int FlowerId { get; set; }
        public int Quantity { get; set; }

        public Item Item { get; set; }
        public Flower Flower { get; set; }
    }
    public class Flower
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int InStock { get; set; }
        public decimal CostPerUnit { get; set; }
        public ICollection<FlowerIngredient> FlowerIngredients { get; set; } = new List<FlowerIngredient>();
        public int? ColorId { get; set; }
        public Color Color { get; set; }
    }
    public class Box
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int inStock { get; set; }

    }
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int InStock { get; set; }
        public decimal CostPerUnit { get; set; }
    }

    public class FlowerIngredient
    {
        public int FlowerId { get; set; }
        public int IngredientId { get; set; }
        public int QuantityRequired { get; set; }

        public Flower Flower { get; set; }
        public Ingredient Ingredient { get; set; }
    }

    public class Color
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsNatural { get; set; } 
    }
}
