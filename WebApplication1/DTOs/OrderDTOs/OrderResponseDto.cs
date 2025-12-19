
namespace WebApplication1
{

    public class OrderResponseDto
    {

        public int Id { get; set; }


        public DateTime OrderDate { get; set; }


        public DateTime OrderCompleteDate { get; set; }


        public decimal TotalPrice { get; set; }


        public bool IsCurrent { get; set; }


        public string? Comment { get; set; }


        public ClientInfoDto? Client { get; set; }


        public List<OrderItemWithDetailsDto> ? Items { get; set; } = [];


        public TimeSpan TimeUntilDue { get; set; }


        public bool IsUrgent => TimeUntilDue.TotalHours < 24;

    }

}