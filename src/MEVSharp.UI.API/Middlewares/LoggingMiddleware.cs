using MEVSharp.Application.Exceptions;

namespace MEVSharp.UI.API.Middlewares
{
    public class LoggingMiddleware : IMiddleware
    {
        private readonly ILogger<LoggingMiddleware> logger;

        public LoggingMiddleware(ILogger<LoggingMiddleware> logger)
        {
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                logger.LogInformation($"From: {context.Request.Path}");
                await next(context);
            }
            catch (ParentHashMismatchException e)
            {
                logger.LogInformation(e, e.Message);
                throw;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
