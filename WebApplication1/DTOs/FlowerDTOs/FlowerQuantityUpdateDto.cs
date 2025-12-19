using System.ComponentModel.DataAnnotations;

namespace WebApplication1
{

    public class FlowerQuantityUpdateDto
    {

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a positive number")]
        public int Quantity { get; set; }


        [Required]
        public bool IsIncrement { get; set; } 

    }

}