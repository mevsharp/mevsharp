using Microsoft.Extensions.Logging;

namespace MEVSharp.Features.Http.Clients.Handlers
{
    public class RetryPolicyHandler : DelegatingHandler
    {
        private readonly int maxRetries;
        private readonly TimeSpan delay;
        private readonly ILogger logger;

        public RetryPolicyHandler(
            HttpMessageHandler innerHandler,
            int maxRetries,
            TimeSpan delay,
            ILogger logger
        )
            : base(innerHandler)
        {
            this.maxRetries = maxRetries;
            this.delay = delay;
            this.logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        )
        {
            HttpResponseMessage response = null;
            int retries = 0;

            while (retries <= maxRetries)
            {
                try
                {
                    if (request.Method == HttpMethod.Post)
                    {
                        if (retries == 0)
                            logger.LogDebug($"Post request sent to relay {request.RequestUri}");
                        else
                        {
                            logger.LogDebug(
                                $"Post request sent to relay (retry {retries} / {maxRetries} {request.RequestUri}"
                            );
                        }
                    }
                    else if (request.Method == HttpMethod.Get)
                    {
                        logger.LogDebug(
                            $"Get request sent to relay {request.RequestUri.AbsoluteUri.ToString()}"
                        );
                    }
                    response = await base.SendAsync(request, cancellationToken)
                        .ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        logger.LogDebug($"Received Status Code: {response.StatusCode}");
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogDebug($"Error: {ex.Message}");
                }

                retries++;
                if (retries <= maxRetries)
                {
                    await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                }
            }

            return response;
        }
    }
}
