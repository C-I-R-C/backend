using System.ComponentModel.DataAnnotations;

namespace WebApplication1
{

    public class UpdateFlowerQuantityDto
    {

        [Range(1, 100)]
        public int Quantity { get; set; }

    }

}