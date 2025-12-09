
namespace WebApplication1
{

    public class OrderItemWithDetailsDto
    {

        public int Id { get; set; }


        public int Quantity { get; set; }


        public decimal UnitPrice { get; set; }


        public ItemDto? Item { get; set; }


        public List<FlowerDetailDto> Flowers { get; set; } = [];


    }


}