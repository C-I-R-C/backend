using System.ComponentModel.DataAnnotations;

namespace WebApplication1
{

    public class AddFlowerToItemDto
    {

        [Range(1, int.MaxValue)]
        public int FlowerId { get; set; }


        [Range(1, 100)]
        public int Quantity { get; set; }

    }

}