
namespace WebApplication1
{

    public class ClientWithDetailedOrdersDto
    {

        public int Id { get; set; }
        
        
        public string? Name { get; set; }
        
        
        public string? PhoneNumber { get; set; }
        
        
        public int TotalOrdersCount { get; set; }
        
        
        public int DiscountLevel { get; set; }
        
        
        public List<OrderResponseDto> Orders { get; set; } = [];
    
    }

}