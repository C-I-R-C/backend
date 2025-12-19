using System.ComponentModel.DataAnnotations;

namespace WebApplication1
{

    public class FlowerUpdateDto
    {

        [StringLength(100)]
        public string? Name { get; set; }


        [Range(0, int.MaxValue)]
        public int InStock { get; set; }


        [Range(0.01, 1000)]
        public decimal CostPerUnit { get; set; }


        public int? ColorId { get; set; }

    }

}