using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class ImagesController : ControllerBase
    {

        private readonly ImageService _imageService;


        public ImagesController(ImageService imageService)
        {

            _imageService = imageService;

        }


        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [AllowAnonymous]
        [RequestSizeLimit(10_485_760)]
        public async Task<ActionResult<ImageDto>>
            Upload(
                [FromForm] ImageUploadDto dto )

        {

            if (dto.File == null)
                return BadRequest("File is required");

            try
            {

                var result = await _imageService.UploadAsync(dto.File, dto.EntityType ?? "", dto.EntityId);
                return Ok(result);

            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Upload failed", details = ex.Message });
            }

        }


        [HttpGet("entity/{entityType}/{entityId}")]
        public async Task<ActionResult<List<ImageDto>>>
            GetForEntity(
                string entityType,
                int entityId )

        {

            var list = await _imageService.GetForEntityAsync(entityType, entityId);
            return Ok(list);

        }


        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        [AllowAnonymous]
        public async Task<ActionResult<ImageDto>>
            Replace(
                int id,
                [FromForm] ImageUploadDto dto )

        {

            if (dto.File == null)
                return BadRequest("File is required");

            try
            {

                var replaced = await _imageService.ReplaceAsync(id, dto.File);
                if (replaced == null)
                    return NotFound();

                return Ok(replaced);

            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Replace failed", details = ex.Message });
            }

        }


        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult>
            Delete(
                int id )

        {

            try
            {

                await _imageService.DeleteAsync(id);
                return NoContent();

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Delete failed", details = ex.Message });
            }

        }


        [AllowAnonymous]
        [HttpGet("{id}/raw")]
        public async Task<IActionResult>
            GetRaw(
                int id )

        {

            var raw = await _imageService.GetRawAsync(id);
            if (raw == null)
                return NotFound();

            return File(raw.Value.Data, raw.Value.ContentType);

        }

    }

}
