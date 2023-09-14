using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSP.ImageService.Data;
using DSP.ImageService.Protos;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace DSP.ImageService.Services
{
    public class UploadService : UploadFileService.UploadFileServiceBase
    {
        private readonly ILogger<UploadService> _logger;
        private readonly ImageServiceDbContext _db;

        public UploadService(ILogger<UploadService> logger, ImageServiceDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public override async Task<Result> AddFile(Chunk request, ServerCallContext context)
        {
            var content = request.Content.ToArray();
            var img = new Image
            {
                Id = new Guid(request.PostId),
                Full = content,
                TimeCreated = DateTime.Now
            };

            _db.Images.Add(img);
            await _db.SaveChangesAsync();

            var factory = new ConnectionFactory() { HostName = "localhost" };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();


            channel.QueueDeclare(queue: "image_crop",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            string msg = img.Id.ToString();
            var body = Encoding.UTF8.GetBytes(msg);

            channel.BasicPublish(exchange: "",
                 routingKey: "image_crop",
                 basicProperties: null,
                 body: body);

            return new Result { Success = true };
        }
        public override async Task<Result> UpdateFile(Chunk request, ServerCallContext context)
        {
            var content = request.Content.ToArray();
            var Id = new Guid(request.PostId);
            var dbImage = _db.Images.FirstOrDefault(a => a.Id == Id);

            if (dbImage == null)
            {
                return new Result
                {
                    Success = false
                };
            }

            dbImage.Full = content;
            dbImage.TimeCreated = DateTime.Now;
            _db.Update(dbImage);

            await _db.SaveChangesAsync();

            var factory = new ConnectionFactory() { HostName = "localhost" };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();


            channel.QueueDeclare(queue: "image_crop",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            string msg = dbImage.Id.ToString();
            var body = Encoding.UTF8.GetBytes(msg);

            channel.BasicPublish(exchange: "",
                 routingKey: "image_crop",
                 basicProperties: null,
                 body: body);

            return new Result { Success = true };
        }
        public override async Task<Result> DeleteFile(FileId request, ServerCallContext context)
        {
            var result = new Result
            {
                Success = false
            };
            var Id = new Guid(request.Id);
            var image = _db.Images.FirstOrDefault(a => a.Id == Id);
            if (image == null)
            {
                return result;
            }

            _db.Images.Remove(image);
            if (await _db.SaveChangesAsync() > 0)
            {
                result.Success = true;
            }
            return result;
        }
    }
}
