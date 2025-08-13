using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.Extensions.AzureImageSerivce
{
    public interface IBlobService
    {
        Task<string> UploadFileAsync(IFormFile file);
        Task<bool> DeleteFileAsync(string blobName);
        Task<string> GetBlobUrl(string blobName);
    }
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private const string ContainerName = "srpm-public";

        public BlobService(IConfiguration config)
        {
            var connectionString = config["storage-account-key"];
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Azure Blob connection string not found in configuration.");

            _blobServiceClient = new BlobServiceClient(connectionString);
        }


        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
            await containerClient.CreateIfNotExistsAsync();
            await containerClient.SetAccessPolicyAsync(PublicAccessType.Blob);

            var blobName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var httpHeaders = new BlobHttpHeaders
            {
                ContentType = file.ContentType,
                ContentDisposition = "inline"
            };

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, new BlobUploadOptions
            {
                HttpHeaders = httpHeaders
            });

            return blobClient.Name; // Return blob name for tracking/deletion
        }

        public async Task<bool> DeleteFileAsync(string blobName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var response = await blobClient.DeleteIfExistsAsync();
            return response.Value;
        }

        public async Task<string> GetBlobUrl(string blobName)
        {
            var accountName = _blobServiceClient.AccountName;
            return $"https://{accountName}.blob.core.windows.net/{ContainerName}/{blobName}";
        }
    }
}
