using DSP.ImageService.Protos;
using Google.Protobuf;

namespace DSP.Gateway.Utilities
{
    public static class UploadImage
    {
        public static async Task SendFile(UploadFileService.UploadFileServiceClient client, IFormFile filePath, string postId)
        {
            byte[] buffer;
            var fileStream = new MemoryStream();
            await filePath.CopyToAsync(fileStream);
            fileStream.Position = 0;
            try
            {
                int length = (int)fileStream.Length;
                buffer = new byte[length];
                int count;
                int sum = 0;

                while ((count = await fileStream.ReadAsync(buffer, sum, length - sum)) > 0)
                    sum += count;
            }
            finally
            {
                fileStream.Close();
            }

            var result = await client.AddFileAsync(new Chunk
            {
                PostId = postId,
                Content = ByteString.CopyFrom(buffer)
            });

            Console.WriteLine(result.Success);

        }
        public static async Task UpdateFile(UploadFileService.UploadFileServiceClient client, IFormFile filePath, string postId)
        {
            byte[] buffer;
            var fileStream = new MemoryStream();
            await filePath.CopyToAsync(fileStream);
            fileStream.Position = 0;
            try
            {
                int length = (int)fileStream.Length;
                buffer = new byte[length];
                int count;
                int sum = 0;

                while ((count = await fileStream.ReadAsync(buffer, sum, length - sum)) > 0)
                    sum += count;
            }
            finally
            {
                fileStream.Close();
            }

            var result = await client.UpdateFileAsync(new Chunk
            {
                PostId = postId,
                Content = ByteString.CopyFrom(buffer)
            });

            Console.WriteLine(result.Success);
        }
        public static async Task DeleteFile(UploadFileService.UploadFileServiceClient client, string fileId)
        {
            var result = await client.DeleteFileAsync(new FileId
            {
                Id = fileId,
            });

            Console.WriteLine(result.Success);
        }
    }
}
