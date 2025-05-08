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
        public DateTime OrderDate { get; set; } = DateTime.Now;
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
        public int InStock { get; set; }

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



    public class DataService
    {
        public List<Client> Clients { get; } = new()
    {
        new Client { Id = 1, Name = "Emma Johnson", PhoneNumber = "+1 555-0101", TotalOrdersCount = 5, DiscountLevel = 10 },
        new Client { Id = 2, Name = "James Smith", PhoneNumber = "+1 555-0102", TotalOrdersCount = 2, DiscountLevel = 5 },
        new Client { Id = 3, Name = "Olivia Williams", PhoneNumber = "+1 555-0103", TotalOrdersCount = 8, DiscountLevel = 15 },
        new Client { Id = 4, Name = "Liam Brown", PhoneNumber = "+1 555-0104", TotalOrdersCount = 1, DiscountLevel = 0 },
        new Client { Id = 5, Name = "Sophia Jones", PhoneNumber = "+1 555-0105", TotalOrdersCount = 12, DiscountLevel = 20 }
    };

        public List<Order> Orders { get; } = new()
    {
       new Order
        {
            Id = 1,
            ClientId = 1, // Emma Johnson
            OrderDate = DateTime.UtcNow.AddDays(-5),
            TotalPrice = 89.97m,
            IsCurrent = false,
            Comment = "Wedding bouquet",
            OrderItems = new List<OrderItem>
            {
                new OrderItem { Id = 1, ItemId = 1, Quantity = 2, UnitPrice = 25.99m },
                new OrderItem { Id = 2, ItemId = 3, Quantity = 1, UnitPrice = 37.99m }
            }
        },
        new Order
        {
            Id = 2,
            ClientId = 3, // Olivia Williams
            OrderDate = DateTime.UtcNow.AddDays(-2),
            TotalPrice = 45.98m,
            IsCurrent = true,
            Comment = "Birthday flowers",
            OrderItems = new List<OrderItem>
            {
                new OrderItem { Id = 3, ItemId = 2, Quantity = 1, UnitPrice = 19.99m },
                new OrderItem { Id = 4, ItemId = 1, Quantity = 1, UnitPrice = 25.99m }
            }
        },
        new Order
        {
            Id = 3,
            ClientId = 1, // Emma Johnson (repeat customer)
            OrderDate = DateTime.UtcNow,
            TotalPrice = 199.99m,
            IsCurrent = true,
            Comment = "Anniversary arrangement",
            OrderItems = new List<OrderItem>
            {
                new OrderItem { Id = 5, ItemId = 3, Quantity = 1, UnitPrice = 199.99m }
            }
        }
    };
        public List<Item> Items = new List<Item>
        {
            new Item { Id = 1, Name = "Rose Bouquet", BasePrice = 25.99m },
            new Item { Id = 2, Name = "Tulip Arrangement", BasePrice = 19.99m },
            new Item { Id = 3, Name = "Wedding Flowers", BasePrice = 199.99m }
        };
        public int NextClientId => Clients.Max(c => c.Id);
        public int NextOrderId => Orders.Max(o => o.Id)+1;
        public int NextOrderItemId =>
        Orders.SelectMany(o => o.OrderItems).Any()
            ? Orders.SelectMany(o => o.OrderItems).Max(oi => oi.Id)
            : 1;
        public int NextItemId => Items.Any() ? Items.Max(i => i.Id) + 1 : 1;
        public List<ItemFlower> ItemFlowers { get; set; } = new()
    {
        // Sample data showing items and their flower components
        new ItemFlower { ItemId = 1, FlowerId = 1, Quantity = 12 }, // 12 red roses in bouquet
        new ItemFlower { ItemId = 1, FlowerId = 3, Quantity = 5 },  // 5 lavender sprigs
        new ItemFlower { ItemId = 2, FlowerId = 2, Quantity = 10 }   // 10 white tulips
    };
        public List<Flower> Flowers { get; set; } = new()
{
    new Flower { Id = 1, Name = "Red Rose", InStock = 100, CostPerUnit = 1.50m },
    new Flower { Id = 2, Name = "White Tulip", InStock = 75, CostPerUnit = 1.20m },
    new Flower { Id = 3, Name = "Lavender", InStock = 50, CostPerUnit = 0.80m }
};

        public List<Color> Colors { get; set; } = new()
{
    new Color { Id = 1, Name = "Red", IsNatural = true },
    new Color { Id = 2, Name = "White", IsNatural = true },
    new Color { Id = 3, Name = "Purple", IsNatural = true }
};

        public int NextFlowerId => Flowers.Any() ? Flowers.Max(f => f.Id) + 1 : 1;
        public int NextColorId => Colors.Any() ? Colors.Max(c => c.Id) + 1 : 1;
        public List<Ingredient> Ingredients { get; set; } = new()
    {
        new Ingredient { Id = 1, Name = "Zephyr Silk", InStock = 100, CostPerUnit = 2.50m },
        new Ingredient { Id = 2, Name = "Luminous Thread", InStock = 75, CostPerUnit = 1.80m },
        new Ingredient { Id = 3, Name = "Ethereal Dye", InStock = 50, CostPerUnit = 3.20m }
    };

        public List<FlowerIngredient> FlowerIngredients { get; set; } = new()
    {
        // Red Rose requires Zephyr Silk and Ethereal Dye
        new FlowerIngredient { FlowerId = 1, IngredientId = 1, QuantityRequired = 2 },
        new FlowerIngredient { FlowerId = 1, IngredientId = 3, QuantityRequired = 1 },
        
        // White Tulip requires Luminous Thread
        new FlowerIngredient { FlowerId = 2, IngredientId = 2, QuantityRequired = 3 }
    };

        public int NextIngredientId => Ingredients.Any() ? Ingredients.Max(i => i.Id) + 1 : 1;
        public List<Box> Boxes { get; set; } = new()
    {
        new Box { Id = 1, Name = "Standard Box", InStock = 50 },
        new Box { Id = 2, Name = "Deluxe Box", InStock = 20 }
    };

        public int NextBoxId => Boxes.Any() ? Boxes.Max(b => b.Id) + 1 : 1;
    }
}
