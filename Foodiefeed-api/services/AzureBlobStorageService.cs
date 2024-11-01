using Azure.Storage.Blobs;
using Windows.Storage;

namespace Foodiefeed_api.services
{

    public interface IAzureBlobStorageSerivce
    {
        public Task UploadPostImagesAsync(int userId, int postId, List<IFormFile> images);
        public Task<List<Stream>> FetchPostImagesAsync(int userId, int postId);
        public Task<Stream> FetchProfileImageAsync(int userId);
        public Task<List<string>> ConvertStreamToBase64Async(List<Stream> streams);
        public Task<string> ConvertStreamToBase64Async(Stream stream);
    }

    public class AzureBlobStorageService : IAzureBlobStorageSerivce
    {
        private readonly BlobServiceClient blobService = new BlobServiceClient("AZURE_FILE_STORAGE_SYSTEM_KEY");

        private readonly BlobContainerClient container;


        public AzureBlobStorageService()
        {
            container = blobService.GetBlobContainerClient("images-storage");
        }

        public async Task<Stream> FetchProfileImageAsync(int userId)
        {
            var dir = $"{userId}/pfp.jpg";

            var blobCleint = container.GetBlobClient(dir);

            var memStream = new MemoryStream();

            await blobCleint.DownloadToAsync(memStream);
            memStream.Position = 0;

            return memStream;
        }

        public async Task UploadPostImagesAsync(int userId,int postId,List<IFormFile> images)
        {
            var dir = $"{userId}/posts/{postId}/";

            int i = 1;

            foreach (var image in images)
            {
                var extension = PickFileExtension(image.ContentType);
                var filename = $"{i}{extension}";
                var fileDir = $"{dir}{filename}";

                var blobClient = container.GetBlobClient(fileDir);

                using (var stream = image.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, overwrite: false);
                }
            }
        }

        public async Task<List<Stream>> FetchPostImagesAsync(int userId, int postId)
        {
            var dir = $"{userId}/posts/{postId}/";

            var images = new List<Stream>();

            await foreach (var item in container.GetBlobsAsync(prefix: dir))
            {
                var blobClient = container.GetBlobClient(item.Name);

                var memStream = new MemoryStream();
                blobClient.DownloadTo(memStream);

                memStream.Position = 0;

                images.Add(memStream);
            }

            return images;
        }

        public async Task<List<string>> ConvertStreamToBase64Async(List<Stream> streams)
        {
            List<string> bases64 = new List<string>();
            foreach(var stream in streams)
            {
                using(var memStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memStream);
                    byte[] bytes = memStream.ToArray();

                    string base64 = Convert.ToBase64String(bytes);
                    memStream.Position = 0;
                    bases64.Add(base64);

                }
            }
            return bases64;
        }

        public async Task<string> ConvertStreamToBase64Async(Stream stream)
        {       
             using (var memStream = new MemoryStream())
             {
                 await stream.CopyToAsync(memStream);
                 byte[] bytes = memStream.ToArray();

                 return Convert.ToBase64String(bytes);
             }
        }

        private string PickFileExtension(string contentType) => contentType switch
        {
            "image/jpeg" => ".jpeg",
            "image/png" => ".png",         
        };
    }
}
