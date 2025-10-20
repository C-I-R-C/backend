using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;

namespace WebApplication1
{
    public class AddFlowerToItemDto
    {
        [Range(1, int.MaxValue)]
        public int FlowerId { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }
    }
}