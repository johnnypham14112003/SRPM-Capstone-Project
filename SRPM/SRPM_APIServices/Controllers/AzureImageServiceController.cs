using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SRPM_Services.Extensions.AzureImageSerivce;

namespace SRPM_APIServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AzureImageServiceController : ControllerBase
    {
        private readonly IBlobService _blobService;

        public AzureImageServiceController(IBlobService blobService)
        {
            _blobService = blobService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            const long maxFileSize = 30 * 1024 * 1024; // 30MB in bytes

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (file.Length > maxFileSize)
                return BadRequest("File size exceeds the 30MB limit.");

            var url = await _blobService.UploadFileAsync(file);
            return Ok(new { Url = url });
        }

        [HttpDelete("delete/{blobName}")]
        public async Task<IActionResult> Delete(string blobName)
        {
            var success = await _blobService.DeleteFileAsync(blobName);
            return success ? Ok("File deleted.") : NotFound("File not found.");
        }
        [HttpGet("get-url")]
        public async Task<IActionResult> GetBlobByName(string blobName)
        {
            var success = await _blobService.GetBlobUrl(blobName);
            return Ok(success);
        }
    }
}
