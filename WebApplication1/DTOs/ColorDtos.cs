using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;

namespace WebApplication1
{
    public class ColorDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool IsNatural { get; set; }
    }

    public class ColorCreateDto
    {
        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        public bool IsNatural { get; set; }
    }

    public class ColorUpdateDto
    {
        [StringLength(50)]
        public string? Name { get; set; }

        public bool? IsNatural { get; set; }
    }
}