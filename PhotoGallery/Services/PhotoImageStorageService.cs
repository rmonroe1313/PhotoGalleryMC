using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoGallery.Services
{
    public class PhotoImageStorageService
    {
        CloudBlobClient _cloudBlobClient;
        string _baseUrl;
        IConfiguration _configuration;

        public PhotoImageStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
            _baseUrl = _configuration["BaseUrl"];

            StorageCredentials credentials = new StorageCredentials(configuration["StorageAccountName"], configuration["StorageCredentials"]);

            _cloudBlobClient = new CloudBlobClient(new Uri(_baseUrl), credentials);
        }

        public async Task<string> SaveImage(Stream imageStream, string imageIdentifier)
        {
            // Get a reference to the container
            CloudBlobContainer container = _cloudBlobClient.GetContainerReference(_configuration["ContainerName"]);
            CloudBlockBlob blob = container.GetBlockBlobReference(imageIdentifier);

            try
            {
                await blob.UploadFromStreamAsync(imageStream);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return imageIdentifier;

        }

        public string UrlFor(string imageIdentifier)
        {
            // Get a reference to the container
            CloudBlobContainer container = _cloudBlobClient.GetContainerReference(_configuration["ContainerName"]);
            CloudBlockBlob blob = container.GetBlockBlobReference(imageIdentifier);

            SharedAccessBlobPolicy sharedAccessSignaturePolicy = new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessStartTime = DateTime.Now.AddMinutes(-15),
                SharedAccessExpiryTime = DateTime.Now.AddMinutes(15)
            };

            string sharedAccessSignature = blob.GetSharedAccessSignature(sharedAccessSignaturePolicy);

            return $"{_baseUrl}/{_configuration["ContainerName"]}/{imageIdentifier}{sharedAccessSignature}";

        }

    }
}
