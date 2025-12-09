using System.ComponentModel.DataAnnotations;

namespace WebApplication1
{

    public class AddIngredientToFlowerDto
    {

        [Range(1, int.MaxValue)]
        public int IngredientId { get; set; }

 
        public int QuantityRequired { get; set; }
    
    }

}