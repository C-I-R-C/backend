
namespace WebApplication1
{
    public class FlowerWithIngredientsDto
    {

        public int Id { get; set; }


        public string? Name { get; set; }


        public int InStock { get; set; }


        public decimal CostPerUnit { get; set; }


        public ColorDto? Color { get; set; }


        public List<FlowerIngredientDto> Ingredients { get; set; } = [];

    }

}