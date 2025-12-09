
namespace WebApplication1
{

    public class OrderFlowersResponseDto
    {

        public int OrderId { get; set; }


        public DateTime OrderDate { get; set; }


        public string? ClientName { get; set; }


        public List<FlowerUsageDto> Flowers { get; set; } = [];

    }

}