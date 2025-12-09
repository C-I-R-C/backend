
namespace WebApplication1
{

    public class ItemFlowerDetailDto
    {

        public int FlowerId { get; set; }
        
        
        public string? FlowerName { get; set; }
        
        
        public int Quantity { get; set; }
        
        
        public decimal UnitCost { get; set; }
        
        public string? Color { get; set; }
        
        
        public List<FlowerIngredient1Dto> Ingredients { get; set; } = new();
    
    }

}