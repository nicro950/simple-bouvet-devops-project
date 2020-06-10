using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Bouvet.Syndicate.TestProject.Configuration.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;


namespace Bouvet.Syndicate.TestProject.Helpers.BlobStorage
{
    public class StorageManager
    {
        private StorageSettings storageSettings;
        private readonly CloudStorageAccount storageAccount;
        private CloudBlobContainer? defaultContainer;

        public StorageManager(IOptionsMonitor<StorageSettings> storageSettings, ILogger<StorageManager> logger)
        {
            this.storageSettings = storageSettings.CurrentValue;
            storageSettings.OnChange((x) =>
            {
                this.storageSettings = x;
            });

            if (!CloudStorageAccount.TryParse(this.storageSettings.ConnectionStringBlob, out var cloudStorageAccount))
            {
                logger.LogError("Failed to parse Connection string for CloudStorageAccount");
                throw new StorageAccountParseException();
            }

            storageAccount = cloudStorageAccount;
        }

        private async Task<CloudBlobContainer> GetContainer(string name)
        {
            var client = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(name);
            await container.CreateIfNotExistsAsync();

            return container;
        }

        public async Task<Stream?> GetFileStream(string name)
        {
            var container = await GetDefaultContainer();

            var blob = container.GetBlockBlobReference(name);

            if (!await blob.ExistsAsync()) return null;

            var memoryStream = new MemoryStream();
            await blob.DownloadToStreamAsync(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }

        private async Task<CloudBlobContainer> GetDefaultContainer()
        {

            if (defaultContainer != null) return defaultContainer;
            if (storageSettings.DefaultContainerName == null) throw new NullReferenceException("The DefaultContainerName is null");

            return defaultContainer = await GetContainer(storageSettings.DefaultContainerName);
        }

        public async Task<string> UploadFile(IFormFile localFile)
        {
            var container = await GetDefaultContainer();

            var fileName = SanitizeFileName(localFile.FileName);

            var blob = container.GetBlockBlobReference(Guid.NewGuid() + "_" + fileName);
            await blob.UploadFromStreamAsync(localFile.OpenReadStream());

            return blob.Name;
        }

        public static string SanitizeFileName(string fileName)
        {
            // Checks if this is a file system path and removes all directory info
            var slashPosition = fileName.LastIndexOfAny(new char[] { '/', '\\' });
            if (slashPosition >= 0)
                fileName = fileName[(slashPosition + 1)..];

            // We could also Uri Encode this data, but this is already handled by the client,
            // so there should not be a need for us to do it here. It would have resulted in 
            // a double encoding.

            return fileName;
        }

        public async Task<bool> DeleteFile(string uploadName)
        {
            var container = await GetDefaultContainer();

            var blob = container.GetBlockBlobReference(uploadName);
            return await blob.DeleteIfExistsAsync();
        }

        public async Task DeleteFiles(IEnumerable<string> uploadNames)
        {
            foreach (var name in uploadNames)
            {
                await DeleteFile(name);
            }
        }
    }


    [Serializable]
    public class StorageAccountParseException : Exception
    {
        public StorageAccountParseException() : base("Failed to parse connection string for storage account") { }
        public StorageAccountParseException(string message) : base(message) { }
        public StorageAccountParseException(string message, Exception inner) : base(message, inner) { }
        protected StorageAccountParseException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
