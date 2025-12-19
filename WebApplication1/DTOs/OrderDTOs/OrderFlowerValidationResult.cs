
namespace WebApplication1
{

    public class OrderFlowerValidationResult
    {

        public int OrderId { get; set; }


        public bool IsValid { get; set; }


        public List<FlowerStockStatus>? FlowerStatuses { get; set; }

    }

}