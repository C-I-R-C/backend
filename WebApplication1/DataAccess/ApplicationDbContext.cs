using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemFlower> ItemFlowers { get; set; }
        public DbSet<Flower> Flowers { get; set; }
        public DbSet<Box> Boxes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<FlowerIngredient> FlowerIngredients { get; set; }
        public DbSet<Color> Colors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Client>()
                .HasMany(c => c.Orders)
                .WithOne(o => o.Client)
                .HasForeignKey(o => o.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Item)
                .WithMany()
                .HasForeignKey(oi => oi.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Item>()
                .HasMany(i => i.ItemFlowers)
                .WithOne(itemFlower => itemFlower.Item)
                .HasForeignKey(itemFlower => itemFlower.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ItemFlower>()
                .HasOne(itemFlower => itemFlower.Flower)
                .WithMany()
                .HasForeignKey(itemFlower => itemFlower.FlowerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Item>()
                .HasOne(i => i.Box)
                .WithMany()
                .HasForeignKey(i => i.BoxId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Flower>()
                .HasMany(f => f.FlowerIngredients)
                .WithOne(fi => fi.Flower)
                .HasForeignKey(fi => fi.FlowerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Flower>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<FlowerIngredient>()
                .HasKey(fi => new { fi.FlowerId, fi.IngredientId });

            modelBuilder.Entity<FlowerIngredient>()
                .HasOne(fi => fi.Ingredient)
                .WithMany()
                .HasForeignKey(fi => fi.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Flower>()
                .HasOne(f => f.Color)
                .WithMany()
                .HasForeignKey(f => f.ColorId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ItemFlower>()
                .HasKey(itemFlower => new { itemFlower.ItemId, itemFlower.FlowerId });
            modelBuilder.Entity<ItemFlower>().ToTable("ItemFlowers");

            modelBuilder.Entity<Client>()
                .Property(c => c.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Box>()
                .Property(b => b.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Order>()
                .Property(b => b.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<OrderItem>()
                .Property(b => b.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Color>()
                .Property(b => b.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Ingredient>()
                .Property(b => b.Id)
                .ValueGeneratedOnAdd();
        }
            
    };
}

