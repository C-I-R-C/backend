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

    public class ClientInfoDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DiscountLevel { get; set; }
    }
}