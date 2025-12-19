using System.ComponentModel.DataAnnotations;



namespace WebApplication1
{

    public class BoxCreateDto
    {

        [Required(ErrorMessage = "Box name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string? Name { get; set; }


        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int InStock { get; set; } = 0;


        [Required(ErrorMessage = "Box cost is required")]
        public decimal CostPerUnit { get; set; }

    }

}