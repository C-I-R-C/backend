using System.Drawing;

namespace WebApplication1
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public Order currentOrder { get; set; }
        public int TotalOrderNumder { get; set; }
        public int discount { get; set; }
    }
    public class Order
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public OrderItem[] OrderItems { get; set; }

        public string Commentary { get; set; }
        public int price { get; set; }
    }
    public class OrderItem
    {
        public int Id { get; set; }
        public Item item { get; set; }
        public int amount { get; set; }
    }
    public class Item
    {
        public int Id { get; set; }
        public string name { get; set; }
        public Flower[] Flowers { get; set; }
        public Box Box { get; set; }
    }
    public class Flower
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int inStock { get; set; }
        public Ingredient[] ingredients { get; set; }
        
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
        public int inStock { get; set; }
        public int Price { get; set; }
    }
    public class Color
    { public int Id { get; set; }
    public string Name { get; set; }
    public int inStock { get; set; }
    public bool type  { get; set; }
    }
    public record StockCheckResult(
    bool IsAvailable,
    int CurrentStock,
    int MissingAmount
);
}
