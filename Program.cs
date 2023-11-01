using ImageDirectory.BackgroundServices;
using ImageDirectory.Middlewares;
using ImageDirectory.Publisher;
using ImageDirectory.Services;
using Microsoft.AspNetCore.Http.Features;
using MongoDB.Driver;
using RabbitMQ.Client;

namespace ImageDirectory
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHostedService<ImageProcessingService>();

            //builder.Services.Configure<FormOptions>(options =>
            //{
            //    options.MultipartBodyLengthLimit = long.MaxValue; // Set to allow larger file uploads
            //});

            var mongoClient = new MongoClient("mongodb://localhost:27017");
            builder.Services.AddSingleton<IMongoClient>(mongoClient);
            builder.Services.AddSingleton<AzureBlobStorageService>();

            
            builder.Services.AddSingleton<IConnectionFactory, ConnectionFactory>(_=>
            {
                return new ConnectionFactory()
                {
                    HostName = "localhost", // RabbitMQ server address
                    Port = 5672,            // Default port for AMQP
                    UserName = "guest",    // Default username
                    Password = "guest"     // Default password
                }; 
            });
            builder.Services.AddSingleton<IAsyncImagePublisher, RabbitMqImagePublisher>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseMiddleware<RequestResponseLoggingMiddleware>();
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
