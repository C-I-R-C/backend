
namespace WebApplication1
{

    public class OrderValidationResult
    {

        public int OrderId { get; set; }


        public bool IsValid { get; set; }


        public List<MaterialStatus>? FlowerStatuses { get; set; }


        public List<MaterialStatus>? IngredientStatuses { get; set; }


        public List<MaterialStatus>? BoxStatuses { get; set; }

    }

}