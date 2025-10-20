using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;


namespace WebApplication1
{
    public class BoxStockUpdateDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be positive")]
        public int Quantity { get; set; }

        [Required]
        public bool IsIncrement { get; set; }

    }
}