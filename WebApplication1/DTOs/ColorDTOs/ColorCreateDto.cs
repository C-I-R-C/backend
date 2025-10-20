using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;

namespace WebApplication1
{
    public class ColorCreateDto
    {
        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        public bool IsNatural { get; set; }
    }
}