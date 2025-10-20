using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;

namespace WebApplication1
{
    public class MaterialStatus
    {
        public int MaterialId { get; set; }
        public string? MaterialName { get; set; }
        public int RequiredQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public bool IsAvailable { get; set; }
    }
}