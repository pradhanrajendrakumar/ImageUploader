using System.Text;
using System.Text.Json;
using ImageDirectory.Models;
using RabbitMQ.Client;

namespace ImageDirectory.Publisher
{
    public class RabbitMqImagePublisher(IConnectionFactory connectionFactory) : IAsyncImagePublisher
    {
        private readonly IConnectionFactory _connectionFactory = connectionFactory;

        public void PublishImage(Image image)
        {
            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var jsonImage = JsonSerializer.Serialize(image);
                channel.QueueDeclare(queue: "image_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                //write the image data to the queue
                channel.BasicPublish(exchange: "", routingKey: "image_queue", basicProperties: null, body: Encoding.UTF8.GetBytes(jsonImage));
            }
        }
    }
}