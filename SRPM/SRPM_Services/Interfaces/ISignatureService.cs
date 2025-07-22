using Microsoft.AspNetCore.Http;
using SRPM_Services.BusinessModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.Interfaces
{
    public interface ISignatureService
    {
        Task<bool> CreateSignatureAsync(Guid documentId, IFormFile signatureImage);
        Task<List<RS_Signature>> ReviewSignatureInDocumentAsync(Guid documentId);
        Task<List<RS_Signature>> GetAllSignature();
        Task<bool> ValidateSignatureAsync(Guid documentId, IFormFile signatureImage);
    }

}
