
namespace WebApplication1
{

    public class FlowerUsageDto
    {

        public int FlowerId { get; set; }


        public string? FlowerName { get; set; }

        
        public int QuantityUsed { get; set; }

        
        public decimal UnitCost { get; set; }

        
        public string? Color { get; set; }

        public List<ItemFlowerUsageDto> Items { get; set; } = [];

    }

}