using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;

namespace WebApplication1
{
    public class ItemCreateDto
    {
        public string? Name { get; set; }
        public decimal BasePrice { get; set; }
        public int BoxId { get; set; }
    }
}