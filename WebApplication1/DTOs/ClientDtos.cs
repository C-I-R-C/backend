using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;

namespace WebApplication1
{
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
        [Required(ErrorMessage = "Client name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^\+?[0-9]{7,15}$", ErrorMessage = "Invalid phone number format")]
        public string PhoneNumber { get; set; }

        [Range(0, 10, ErrorMessage = "Discount level must be between 0 and 100")]
        public int DiscountLevel { get; set; } = 0;
    }

    public class ClientUpdateDto
    {
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public int? DiscountLevel { get; set; }
    }

    public class ClientInfoDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DiscountLevel { get; set; }
    }
    public class ClientWithDetailedOrdersDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public int TotalOrdersCount { get; set; }
        public int DiscountLevel { get; set; }
        public List<OrderResponseDto> Orders { get; set; } = new();
    }
}