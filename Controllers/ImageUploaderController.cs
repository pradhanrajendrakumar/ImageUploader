using ImageDirectory.Models;
using ImageDirectory.Publisher;
using Microsoft.AspNetCore.Mvc;

namespace ImageDirectory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageUploaderController : ControllerBase
    {
        private readonly IAsyncImagePublisher _asyncImagePublisher;

        public ImageUploaderController(IAsyncImagePublisher asyncImagePublisher)
        {
            _asyncImagePublisher = asyncImagePublisher;
        }

        [HttpPost("upload")]
        public IActionResult UploadImage(IFormFile image)
        {
            // Validate and process the image
            byte[] imageData;
            using (var stream = image.OpenReadStream())
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                imageData = memoryStream.ToArray();
            }

            _asyncImagePublisher.
                        PublishImage(new Image
                        {
                            ImageData = imageData,
                            FileName = image.FileName,
                            ContentType = image.ContentType
                        });
            
            return Ok("Image uploaded and sent to RabbitMQ.");
        }
    }
}
