
namespace WebApplication1
{
    public class OrderItemFlowersResponseDto
    {

        public int OrderId { get; set; }


        public int ItemId { get; set; }


        public string? ItemName { get; set; }


        public int ItemQuantity { get; set; }


        public List<FlowerDetailDto> Flowers { get; set; } = [];

    }

}