using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ImageDirectory.Models;
using Microsoft.Extensions.Configuration;

namespace ImageDirectory.Services
{
    public class AzureBlobStorageService
    {
        private readonly string _connectionString;
        private readonly string _containerName;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _blobContainerClient;

        public AzureBlobStorageService(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("AzureBlobStorage:ConnectionString").Value;
            _containerName = configuration.GetSection("AzureBlobStorage:ContainerName").Value;

            _blobServiceClient = new BlobServiceClient(_connectionString);
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        }

        public async Task<string> UploadImageAsync(Image imageBytes)
        {        
            var imageId = $"{imageBytes.FileName}"; 
            var blobClient = _blobContainerClient.GetBlobClient(imageId);

            // Upload the image bytes to Azure Blob Storage
            using (var stream = new MemoryStream(imageBytes.ImageData))
            {
                BlobHttpHeaders headers = new BlobHttpHeaders
                {
                    ContentType = imageBytes.ContentType // Set the content type based on the uploaded file's content type
                };

                await blobClient.UploadAsync(stream,new BlobUploadOptions { HttpHeaders=headers});
            }
            return blobClient.Uri.ToString();
        }
    }
}
