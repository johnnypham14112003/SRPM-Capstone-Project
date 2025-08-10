using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class SignatureController : ControllerBase
{
    private readonly ISignatureService _signatureService;

    public SignatureController(ISignatureService signatureService)
    {
        _signatureService = signatureService;
    }

    /// <summary>
    /// Creates a signature for a document using an uploaded image.
    /// </summary>
    [HttpPost("{documentId}/sign")]
    public async Task<IActionResult> CreateSignature(Guid documentId, IFormFile signatureImage)
    {
        if (signatureImage == null || signatureImage.Length == 0)
            return BadRequest("Signature image is required.");

        var result = await _signatureService.CreateSignatureAsync(documentId, signatureImage);
        return result ? Ok("Signature created successfully.") : StatusCode(500, "Failed to create signature.");
    }

    /// <summary>
    /// Reviews all signatures associated with a specific document.
    /// </summary>
    [HttpGet("{documentId}/review")]
    public async Task<ActionResult<List<RS_Signature>>> ReviewSignatures(Guid documentId)
    {
        var signatures = await _signatureService.ReviewSignatureInDocumentAsync(documentId);
        return Ok(signatures);
    }

    /// <summary>
    /// Retrieves all signatures in the system.
    /// </summary>
    [HttpGet("all")]
    public async Task<ActionResult<List<RS_Signature>>> GetAllSignatures()
    {
        var signatures = await _signatureService.GetAllSignature();
        return Ok(signatures);
    }

    /// <summary>
    /// Validates a signature image against existing signatures for a document.
    /// </summary>
    [HttpPost("{documentId}/validate")]
    public async Task<IActionResult> ValidateSignature(Guid documentId, IFormFile signatureImage)
    {
        if (signatureImage == null || signatureImage.Length == 0)
            return BadRequest("Signature image is required.");

        var isValid = await _signatureService.ValidateSignatureAsync(documentId, signatureImage);
        return Ok(new { isValid });
    }
    /// <summary>
    /// Deletes a signature by its ID.
    /// </summary>
    [HttpDelete("{signatureId}")]
    public async Task<IActionResult> DeleteSignature(Guid signatureId)
    {
        var result = await _signatureService.DeleteSignatureAsync(signatureId);

        if (!result)
            return NotFound("Signature not found.");

        return Ok("Signature deleted successfully.");
    }
}