using System.ComponentModel.DataAnnotations;

namespace WebApplication1
{

    public class UpdateIngredientStockDto
    {

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be positive")]
        public int Quantity { get; set; }


        [Required]
        public bool IsIncrement { get; set; }
    
    }

}