using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Foodiefeed_api.exceptions;
using Windows.Storage;

namespace Foodiefeed_api.services
{

    public interface IAzureBlobStorageSerivce
    {
        public Task UploadPostImagesAsync(int userId, int postId, List<IFormFile> images, CancellationToken token);
        //public Task<List<Stream>> FetchPostImagesAsync(int userId, int postId, CancellationToken token);
        //public Task<Stream> FetchProfileImageAsync(int userId, CancellationToken token);
        public Task<List<string>> ConvertStreamToBase64Async(List<Stream> streams, CancellationToken token);
        public Task<string> ConvertStreamToBase64Async(Stream stream, CancellationToken token);
        public Task RemvePostImagesRangeAsync(int userId, int postId);
        //public Task<MemoryStream> FetchRecipeImage(int postId, int userId, CancellationToken token);
        public Task UploadNewProfilePicture(int userId, IFormFile file);
        public Task RemoveUserProfilePicture(int userId);

        public Task<List<Uri>> FetchPostImages(int userId, int postId, CancellationToken token);
        public Task<Uri> FetchProfileImage(int userId, CancellationToken token);
        public Task<Uri> FetchRecipeImageAsync(int postId, int userId, CancellationToken token);



    }

    public class AzureBlobStorageService : IAzureBlobStorageSerivce
    {
        private readonly BlobServiceClient blobService;
        private readonly BlobContainerClient container;

        private readonly IConfiguration _config;

        public AzureBlobStorageService(IConfiguration config)
        {
            _config = config;
            blobService = new BlobServiceClient(_config["AZURE_FILE_STORAGE_SYSTEM_KEY"]);
            container = blobService.GetBlobContainerClient("images-storage");
        }

        //public async Task<Stream> FetchProfileImageAsync(int userId, CancellationToken token)
        //{
        //    var dir = $"{userId}/pfp.";

        //    token.ThrowIfCancellationRequested();

        //    await foreach (var blobItem in container.GetBlobsAsync(prefix: dir))
        //    {
        //        var blobClient = container.GetBlobClient(blobItem.Name);

        //        var memStream = new MemoryStream();
        //        await blobClient.DownloadToAsync(memStream);
        //        memStream.Position = 0;

        //        return memStream;
        //    }
        //    return null;
        //}

        public async Task UploadPostImagesAsync(int userId,int postId,List<IFormFile> images, CancellationToken token)
        {
            var dir = $"{userId}/posts/{postId}/";

            int i = 1;

            token.ThrowIfCancellationRequested();

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

        //public async Task<List<Stream>> FetchPostImagesAsync(int userId, int postId, CancellationToken token)
        //{
        //    var dir = $"{userId}/posts/{postId}/";

        //    var images = new List<Stream>();

        //    token.ThrowIfCancellationRequested();

        //    await foreach (var item in container.GetBlobsAsync(prefix: dir))
        //    {
        //        var blobClient = container.GetBlobClient(item.Name);

        //        var memStream = new MemoryStream();
        //        blobClient.DownloadTo(memStream);

        //        memStream.Position = 0;

        //        images.Add(memStream);
        //    }

        //    return images;
        //}

        //public async Task<MemoryStream> FetchRecipeImage(int postId, int userId, CancellationToken token)
        //{
        //    var dir = $"{userId}/posts/{postId}/";

        //    token.ThrowIfCancellationRequested();

        //    using (var memStream = new MemoryStream())
        //    {
        //        await foreach (var blobItem in container.GetBlobsAsync(prefix: dir))
        //        {
        //            if (Path.GetFileNameWithoutExtension(blobItem.Name) == "1")
        //            {
        //                var blobClient = container.GetBlobClient(blobItem.Name);

        //                await blobClient.DownloadToAsync(memStream);

        //                memStream.Position = 0;
        //                break;
        //            }
        //        }
        //        return memStream;
        //    }
        //}

        public async Task<List<string>> ConvertStreamToBase64Async(List<Stream> streams, CancellationToken token)
        {
            List<string> bases64 = new List<string>();

            foreach(var stream in streams)
            {
                token.ThrowIfCancellationRequested();

                using (var memStream = new MemoryStream())
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

        public async Task<string> ConvertStreamToBase64Async(Stream stream, CancellationToken token)
        {
             if (stream is null) return null;

            token.ThrowIfCancellationRequested();

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

        public async Task RemvePostImagesRangeAsync(int userId, int postId)
        {
            var dir = $"{userId}/posts/{postId}/";


            await foreach (BlobItem item in container.GetBlobsAsync(prefix: dir))
            {
                BlobClient client = container.GetBlobClient(item.Name);
                await client.DeleteIfExistsAsync();
            }
        }

        public async Task UploadNewProfilePicture(int userId,IFormFile file)
        {
            var dir = $"{userId}/";
            var prefix = $"{dir}/pfp.";

            await foreach (var blobItem in container.GetBlobsAsync(prefix: prefix))
            {
                var existingBlobClient = container.GetBlobClient(blobItem.Name);
                await existingBlobClient.DeleteIfExistsAsync();
                break;
            }

            var newBlobClient = container.GetBlobClient($"{dir}pfp{Path.GetExtension(file.FileName)}");

            using (var stream = file.OpenReadStream())
            {
                await newBlobClient.UploadAsync(stream, overwrite: true);
            }
        }

        public async Task RemoveUserProfilePicture(int userId)
        {
            var dir = $"{userId}/";

            await foreach (var item in container.GetBlobsAsync(prefix: dir))
            {
                if (item.Name.StartsWith($"{dir}pfp."))
                {
                    var blobClient = container.GetBlobClient(item.Name);

                    await blobClient.DeleteIfExistsAsync();
                    return;
                }
            }
        }

        //asdasdasasdasdasdasdasdasdasdasd
       
        public async Task<List<Uri>> FetchPostImages(int userId,int postId,CancellationToken token)
        {
            var dir = $"{userId}/posts/{postId}/";

            var imageUrls = new List<Uri>();

            token.ThrowIfCancellationRequested();

            await foreach (var item in container.GetBlobsAsync(prefix: dir, cancellationToken: token))
            {
                var blobClient = container.GetBlobClient(item.Name);
                imageUrls.Add(blobClient.Uri); // Dodanie URL do listy
            }

            return imageUrls;
        }

        public async Task<Uri> FetchRecipeImageAsync(int postId, int userId, CancellationToken token)
        {
            var dir = $"{userId}/posts/{postId}/";

            token.ThrowIfCancellationRequested();

            using (var memStream = new MemoryStream())
            {
                await foreach (var blobItem in container.GetBlobsAsync(prefix: dir))
                {
                    if (Path.GetFileNameWithoutExtension(blobItem.Name) == "1")
                    {
                        var blobClient = container.GetBlobClient(blobItem.Name);

                        return blobClient.Uri;
                    }
                }
            }
            return null;
        }

        public async Task<Uri> FetchProfileImage(int userId, CancellationToken token)
        {
            var dir = $"{userId}/pfp.";

            token.ThrowIfCancellationRequested();

            await foreach (var blobItem in container.GetBlobsAsync(prefix: dir))
            {
                var blobClient = container.GetBlobClient(blobItem.Name);

                return blobClient.Uri;
            }
            return null;
        }
    }
}
