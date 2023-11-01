using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using MongoDB.Driver;
using MongoDB.Bson;
using ImageDirectory.Services;
using System.Text;
using System.Text.Json;
using ImageDirectory.Models;

namespace ImageDirectory.BackgroundServices
{
    public class ImageProcessingService(IServiceProvider serviceProvider, AzureBlobStorageService azureBlobStorageService) : IHostedService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly AzureBlobStorageService azureBlobStorageService = azureBlobStorageService;
        private IModel _channel;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost", // RabbitMQ server address
                UserName = "guest",
                Password = "guest",
                Port = 5672
            };

            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();

            _channel.QueueDeclare(queue: "image_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                Image image = JsonSerializer.Deserialize<Image>(Encoding.UTF8.GetString(body));
                var url = await azureBlobStorageService.UploadImageAsync(image);

                // Process and save the image data to MongoDB
                using (var scope = _serviceProvider.CreateScope())
                {
                    var mongoClient = scope.ServiceProvider.GetRequiredService<IMongoClient>();
                    var database = mongoClient.GetDatabase("ImageDirectory");
                    var collection = database.GetCollection<BsonDocument>("images");
                    var imageDocument = new BsonDocument
                    {
                        { "imageUri", url }
                    };
                    collection.InsertOne(imageDocument);
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(queue: "image_queue", autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel.Close();
            return Task.CompletedTask;
        }
    }
}
