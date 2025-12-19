
namespace WebApplication1
{

    public class ItemResponseDto
    {

        public int Id { get; set; }


        public string? Name { get; set; }


        public decimal BasePrice { get; set; }


        public BoxDto? Box { get; set; }


        public List<ItemFlowerDetailDto> Flowers { get; set; } = [];


        public decimal ProductionCost { get; set; }

    }

}