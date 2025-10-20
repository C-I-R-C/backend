using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;

namespace WebApplication1
{
    public class ItemResponseDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal BasePrice { get; set; }
        public BoxDto? Box { get; set; }
        public List<ItemFlowerDetailDto> Flowers { get; set; } = new();
        public decimal ProductionCost { get; set; }
    }
}