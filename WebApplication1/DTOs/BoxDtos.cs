using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;


namespace WebApplication1
{
    public class BoxDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int InStock { get; set; }
        public decimal CostPerUnit { get; set; }
    }

    public class BoxCreateDto
    {
        [Required(ErrorMessage = "Box name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int InStock { get; set; } = 0;
        [Required(ErrorMessage = "Box cost is required")]
        public decimal CostPerUnit { get; set; }
    }

    public class BoxStockUpdateDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be positive")]
        public int Quantity { get; set; }

        [Required]
        public bool IsIncrement { get; set; }

    }
}