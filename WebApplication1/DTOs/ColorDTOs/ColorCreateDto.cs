using System.ComponentModel.DataAnnotations;

namespace WebApplication1
{

    public class ColorCreateDto
    {

        [Required]
        [StringLength(50)]
        public string? Name { get; set; }


        public bool IsNatural { get; set; }

    }

}