
namespace WebApplication1
{

    public class FlowerIngredientDto
    {
        public int FlowerId { get; set; }


        public int IngredientId { get; set; }


        public decimal QuantityRequired { get; set; }


        public IngredientDto? Ingredient { get; set; }

    }

}