using System.Diagnostics;

namespace MEVSharp.UI.API.Middlewares
{
    public class BenchmarkMiddleware : IMiddleware
    {
        private readonly ILogger<BenchmarkMiddleware> logger;
        Stopwatch stopwatch;

        public BenchmarkMiddleware(ILogger<BenchmarkMiddleware> logger)
        {
            this.logger = logger;
            stopwatch = new Stopwatch();
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                stopwatch.Restart();
                await next(context);
                stopwatch.Stop();
                logger.LogDebug($"Execution time: {stopwatch.Elapsed}");
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
