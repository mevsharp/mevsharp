using MEVSharp.Application.Configurations;
using MEVSharp.Application.Factories;
using MEVSharp.Application.Models;
using MEVSharp.Application.Providers;
using MEVSharp.Features.Http.Clients;
using MEVSharp.Features.Http.Clients.Handlers;
using MEVSharp.Features.Http.Clients.Resources.APIBuilder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MEVSharp.Tests.Integrations.Features.Shared
{
    public class IntegrationMockContextStartupTest : MEVSharp.UI.API.Startup
    {
        private readonly ILogger logger;

        public IntegrationMockContextStartupTest(IConfiguration env)
            : base(env)
        {
            this.logger = logger;
        }

        public string ConnectionString { get; private set; }

        public override void AddConfigureServices(IServiceCollection service)
        {
            service.AddScoped<IBuilderAPIClient, BuilderAPIMock>();

            service.AddScoped<IBuilderAPIResource>(builder =>
            {
                var settings = builder.GetService<AppSettings>();
                var client = new HttpClient()
                {
                    BaseAddress = new Uri(
                        "https://0xafa4c6985aa049fb79dd37010438cfebeb0f2bd42b115b89dd678dab0670c1de38da0c4e9138c9290a398ecd9a0b3110@mevsharp.io"
                    )
                };
                return new BuilderAPIResourceMOCK(client, logger);
            });
            service.AddScoped<IBuilderAPIClient>(builder =>
            {
                var resource = builder.GetService<IBuilderAPIResource>();
                return new BuilderAPIClient(resource);
            });
            service.AddScoped<IRelayProvider, RelayProviderMock>(
                (x) =>
                {
                    var appSettings = x.GetService<AppSettings>();
                    var factory = x.GetService<IAPIBuilderFactory>();
                    //var builderAPI = x.GetService<IBuilderApi>();
                    var logger = x.GetService<ILogger<EthHeader>>();
                    var client = x.GetService<IBuilderAPIClient>();
                    return new RelayProviderMock(factory, logger, client, appSettings);
                }
            );
            service.AddSingleton<AppSettings>(serviceProvider =>
            {
                var options = serviceProvider.GetService<IOptions<AppSettings>>();
                options.Value.Network = "goerli";

                return options?.Value
                    ?? throw new InvalidOperationException("AppSettings is not configured.");
            });
            service.AddScoped<IAPIBuilderFactory>(
                (builder) =>
                {
                    var settings = builder.GetService<AppSettings>();
                    var logger = builder.GetService<ILogger<EthHeader>>();
                    var httpLogger = builder.GetService<ILogger<HttpClient>>();
                    var clientFactory = builder.GetService<IHttpClientFactory>();
                    var httpContext = builder.GetService<IHttpContextAccessor>();

                    httpContext?.HttpContext?.Request?.Headers?.TryGetValue(
                        "User-Agent",
                        out var userAgent
                    );

                    List<IBuilderAPIClient> clients = new List<IBuilderAPIClient>();
                    foreach (var item in settings.RelayUrls)
                    {
                        var client = new HttpClient(
                            new RetryPolicyHandler(
                                new HttpClientHandler(),
                                settings.HttpRetryCount,
                                TimeSpan.FromMilliseconds(settings.HttpRetryDelay),
                                httpLogger
                            )
                        )
                        {
                            BaseAddress = new Uri(item)
                        };

                        var resource = new BuilderAPIResourceMOCK(client, httpLogger);
                        var clientResource = new BuilderAPIClient(resource);
                        clients.Add(clientResource);
                    }
                    return new APIBuilderFactory(logger, settings, clients);
                }
            );
        }
    }
}
