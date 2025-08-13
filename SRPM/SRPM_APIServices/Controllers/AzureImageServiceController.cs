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
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

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
