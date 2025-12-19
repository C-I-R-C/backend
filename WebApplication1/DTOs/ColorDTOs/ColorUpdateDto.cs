using System.ComponentModel.DataAnnotations;

namespace WebApplication1
{

    public class ColorUpdateDto
    {

        [StringLength(50)]
        public string? Name { get; set; }


        public bool? IsNatural { get; set; }

    }

}