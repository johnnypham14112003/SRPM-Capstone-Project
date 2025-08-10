using Microsoft.AspNetCore.Http;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Extensions.Enumerables;
using SRPM_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.Implements
{
    public class SignatureService : ISignatureService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;

        public SignatureService(IUnitOfWork unitOfWork, IUserContextService userContext)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
        }

        public async Task<bool> CreateSignatureAsync(Guid documentId, IFormFile signatureImage)
        {
            using var ms = new MemoryStream();
            await signatureImage.CopyToAsync(ms);
            byte[] imageBytes = ms.ToArray();
            var accountId = Guid.Parse(_userContext.GetCurrentUserId());
            var currentRole = _userContext.GetCurrentUserRole();
            var SignerName = _unitOfWork.GetAccountRepository().GetByIdAsync(accountId)?.Result?.FullName ?? "Unknown Signer";

            string hashBase64 = Convert.ToBase64String(SHA256.Create().ComputeHash(imageBytes));
            var userRole = await _unitOfWork.GetUserRoleRepository()
                .GetOneAsync(r =>
                    r.AccountId == accountId &&
                    r.Role != null &&
                    r.Role.Name == currentRole &&
                    r.Status.ToLower() != Status.Deleted.ToString().ToLower() &&
                    r.ProjectId == null &&
                    r.AppraisalCouncilId == null,
                    hasTrackings: false
                );
            var signature = new Signature
            {
                Id = Guid.NewGuid(),
                DocumentId = documentId,
                SignerId = userRole.Id,
                SignerName = SignerName,
                SignatureHash = hashBase64,
                SignedDate = DateTime.UtcNow
            };

            await _unitOfWork.GetSignatureRepository().AddAsync(signature);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<List<RS_Signature>> ReviewSignatureInDocumentAsync(Guid documentId)
        {
            var signatures = await _unitOfWork.GetSignatureRepository().GetListAsync(s => s.DocumentId == documentId);

            return signatures.Select(s => new RS_Signature
            {
                Id = s.Id,
                SignerName = s.SignerName,
                SignatureHashPreview = s.SignatureHash?.Substring(0, 16) + "...",
                SignedDate = s.SignedDate
            }).ToList();
        }

        public async Task<List<RS_Signature>> GetAllSignature()
        {
            var allSignatures = await _unitOfWork.GetSignatureRepository().GetListAsync(p => true,hasTrackings:false);

            return allSignatures.Select(s => new RS_Signature
            {
                Id = s.Id,
                SignerName = s.SignerName,
                SignatureHashPreview = s.SignatureHash?.Substring(0, 16) + "...",
                SignedDate = s.SignedDate
            }).ToList();
        }
        public async Task<bool> ValidateSignatureAsync(Guid documentId, IFormFile signatureImage)
        {
            using var ms = new MemoryStream();
            await signatureImage.CopyToAsync(ms);
            byte[] uploadedBytes = ms.ToArray();

            string uploadedHash = Convert.ToBase64String(SHA256.Create().ComputeHash(uploadedBytes));

            var existingSignatures = await _unitOfWork.GetSignatureRepository().GetListAsync(s => s.DocumentId == documentId);

            return existingSignatures.Any(sig => sig.SignatureHash == uploadedHash);
        }
        public async Task<bool> DeleteSignatureAsync(Guid signatureId)
        {
            var signatureRepo = _unitOfWork.GetSignatureRepository();
            var signature = await signatureRepo.GetByIdAsync(signatureId);

            if (signature == null)
                return false;

            await signatureRepo.DeleteAsync(signature);
            return true;
        }
    }

}
