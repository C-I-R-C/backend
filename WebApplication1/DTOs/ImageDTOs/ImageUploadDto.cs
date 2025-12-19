using Microsoft.AspNetCore.Http;

namespace WebApplication1
{

    public class ImageUploadDto
    {

        public IFormFile? File { get; set; }


        public string? EntityType { get; set; }


        public int EntityId { get; set; }

    }
    
}
