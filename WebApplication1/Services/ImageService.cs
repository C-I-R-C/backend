using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;



namespace WebApplication1.Services
{

    public class ImageService
    {

        private readonly ApplicationDbContext _data;


        private readonly IWebHostEnvironment _env;


        private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];


        private const long MaxFileSizeBytes = 5 * 1024 * 1024;


        public ImageService(ApplicationDbContext data, IWebHostEnvironment env)
        {

            _data = data;
            _env = env;

        }


        public async Task<ImageDto> UploadAsync(IFormFile file, string entityType, int entityId)
        {

            ValidateFile(file);
            var bytes = await ReadFileBytes(file);


            var img = new Image
            {
                FileName = Path.GetFileName(file.FileName),
                Data = bytes,
                ContentType = file.ContentType ?? "application/octet-stream",
                EntityType = entityType,
                EntityId = entityId,
                CreatedAt = DateTime.UtcNow
            };


            _data.Images.Add(img);
            await _data.SaveChangesAsync();


            img.Url = $"/api/images/{img.Id}/raw";
            await _data.SaveChangesAsync();


            return MapToDto(img);

        }


        public async Task<ImageDto?> ReplaceAsync(int id, IFormFile file)
        {

            var existing = await _data.Images.FirstOrDefaultAsync(i => i.Id == id);
            if (existing == null) return null;


            ValidateFile(file);
            var bytes = await ReadFileBytes(file);


            existing.FileName = Path.GetFileName(file.FileName);
            existing.Data = bytes;
            existing.ContentType = file.ContentType ?? "application/octet-stream";
            existing.CreatedAt = DateTime.UtcNow;
            existing.Url = $"/api/images/{existing.Id}/raw";


            await _data.SaveChangesAsync();
            return MapToDto(existing);

        }


        public async Task DeleteAsync(int id)
        {

            var existing = await _data.Images.FirstOrDefaultAsync(i => i.Id == id);
            if (existing == null) return;


            _data.Images.Remove(existing);
            await _data.SaveChangesAsync();

        }


        public async Task<List<ImageDto>> GetForEntityAsync(string entityType, int entityId)
        {

            return await _data.Images
                .Where(i => i.EntityType == entityType && i.EntityId == entityId)
                .OrderByDescending(i => i.CreatedAt)
                .Select(i => new ImageDto
                {
                    Id = i.Id,
                    
                    FileName = i.FileName,

                    Url = $"/api/images/{i.Id}/raw",

                    ContentType = i.ContentType,

                    EntityType = i.EntityType,

                    EntityId = i.EntityId,

                    CreatedAt = i.CreatedAt
                })
                .ToListAsync();

        }


        public async Task<(byte[] Data, string ContentType)?> GetRawAsync(int id)
        {

            var img = await _data.Images.FirstOrDefaultAsync(i => i.Id == id);
            if (img == null || img.Data == null) return null;
            return (img.Data, img.ContentType ?? "application/octet-stream");

        }


        private static void ValidateFile(IFormFile file)
        {

            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            if (file.Length > MaxFileSizeBytes)
                throw new ArgumentException("File too large");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
                throw new ArgumentException("Invalid file type");

        }


        private static async Task<byte[]> ReadFileBytes(IFormFile file)
        {

            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            return ms.ToArray();

        }


        private static ImageDto MapToDto(Image img) => new()
        {

            Id = img.Id,

            FileName = img.FileName,

            Url = img.Url,

            ContentType = img.ContentType,

            EntityType = img.EntityType,

            EntityId = img.EntityId,

            CreatedAt = img.CreatedAt

        };

    }

}
