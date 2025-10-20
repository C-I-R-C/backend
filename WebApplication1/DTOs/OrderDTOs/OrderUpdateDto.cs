using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;

namespace WebApplication1
{
    public class OrderUpdateDto
    {
        public string? Comment { get; set; }
        public bool? IsCurrent { get; set; }
        public DateTime? OrderCompleteDate { get; set; }
    }
}