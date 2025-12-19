
namespace WebApplication1
{

    public class OrderSummaryDto
    {

        public int Id { get; set; }


        public DateTime OrderDate { get; set; }


        public decimal TotalPrice { get; set; }


        public bool IsCurrent { get; set; }

    }

}