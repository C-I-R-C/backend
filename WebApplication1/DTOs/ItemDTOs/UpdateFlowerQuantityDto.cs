using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;

namespace WebApplication1
{
    public class UpdateFlowerQuantityDto
    {
        [Range(1, 100)]
        public int Quantity { get; set; }
    }
}