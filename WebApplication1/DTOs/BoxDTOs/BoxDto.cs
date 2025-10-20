using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;


namespace WebApplication1
{
    public class BoxDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int InStock { get; set; }
        public decimal CostPerUnit { get; set; }
    }
}