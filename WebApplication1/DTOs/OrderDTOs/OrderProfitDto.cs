using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;

namespace WebApplication1
{
    public class OrderProfitDto
    {
        public int OrderId { get; set; }
        public decimal TotalSellingPrice { get; set; }
        public decimal TotalActualCost { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ProfitBeforeDiscount { get; set; }
        public decimal FinalProfit { get; set; }
        public decimal ProfitMargin { get; set; } // Percentage
    }
}