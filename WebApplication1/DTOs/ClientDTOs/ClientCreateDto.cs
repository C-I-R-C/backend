using System.ComponentModel.DataAnnotations;

namespace WebApplication1
{

    public class ClientCreateDto
    {
        [Required(ErrorMessage = "Client name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string? Name { get; set; }


        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^\+?[0-9]{7,15}$", ErrorMessage = "Invalid phone number format")]
        public string? PhoneNumber { get; set; }


        [Range(0, 100, ErrorMessage = "Discount level must be between 0 and 100")]
        public int DiscountLevel { get; set; } = 0;

    }

}