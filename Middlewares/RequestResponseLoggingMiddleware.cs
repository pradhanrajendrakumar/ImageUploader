using System.Text;

namespace ImageDirectory.Middlewares
{
    internal class RequestResponseLoggingMiddleware
    {
        public readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            // Log the request
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(await FormatRequest(context.Request));
            }
            
            // capture the response
            var originalBodyStream = context.Response.Body;
            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;
                await _next(context);
                // Log the response
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation(await FormatResponse(context.Response));
                }

                //responseBody.Seek(0, SeekOrigin.Begin);
                //await responseBody.CopyToAsync(originalBodyStream);

            }
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            var body = request.Body;
            request.EnableBuffering();
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body = body;
            return $"Request: {request.Scheme} {request.Host}{request.Path} {request.QueryString} {bodyAsText}";
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            return $"Response: {text}";
        }

    }
}