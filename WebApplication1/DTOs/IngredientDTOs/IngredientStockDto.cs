
namespace WebApplication1
{

    public class IngredientStockDto
    {

        public int Id { get; set; }


        public string? Name { get; set; }


        public int InStock { get; set; }


        public decimal CostPerUnit { get; set; }


        public string StockStatus => InStock switch
        {

            < 3 => "Critical",
            < 5 => "Low",
            _ => "Adequate"

        };

    }

}