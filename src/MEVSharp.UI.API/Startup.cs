using MEVSharp.Application.Configurations;
using MEVSharp.Application.Factories;
using MEVSharp.Application.Models;
using MEVSharp.Features.Http.Clients;
using MEVSharp.Features.Http.Clients.Handlers;
using MEVSharp.Features.Http.Clients.Resources;
using MEVSharp.Features.Http.Clients.Resources.Telegram;
using MEVSharp.Features.Http.Clients.Resources.Zapier;
using MEVSharp.Features.Http.Clients.Services;
using MEVSharp.UI.API.Middlewares;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Linq;
using Nethermind.Trie.Pruning;

namespace MEVSharp.UI.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public virtual void AddConfigureServices(IServiceCollection service) { }

        

        public void ConfigureServices(IServiceCollection services)
        {


            
            services.AddScoped<BenchmarkMiddleware>();
            services.AddScoped<LoggingMiddleware>();
          

            ConfigureService.AddApplication(services, Configuration);
            services.Configure<AppSettings>(Configuration.GetSection("Settings"));
            services.AddSingleton<AppSettings>(serviceProvider =>

            {
                var options = serviceProvider.GetService<IOptions<AppSettings>>();

                if (Program.CLISettings == null)
                    return options?.Value
                        ?? throw new InvalidOperationException("AppSettings is not configured.");

                options.Value.RequestTimeoutGetheader = Program.CLISettings.RequestTimeoutGetHeader;
                options.Value.RequestTimeoutRegval = Program.CLISettings.RequestTimeoutRegVal;
                options.Value.RequestTimeoutGetpayload = Program
                    .CLISettings
                    .RequestTimeoutGetPayload;
                options.Value.RelayUrls = Program.CLISettings.Relay.ToList();
                options.Value.HostPort = Program.CLISettings.Listen.ToList();
                options.Value.Network = Program.CLISettings.Network;
                options.Value.GenesisForkVersion = Program.CLISettings.GenesisForkVersion;
                options.Value.ZapierID = Program.CLISettings.ZapierID;
                options.Value.ZapierSecret = Program.CLISettings.ZapierSecret;
                options.Value.TelegramAPI = Program.CLISettings.TelegramAPI;
                options.Value.TelegramChatID = Program.CLISettings.TelegramChatID;
                options.Value.MinBid = Program.CLISettings.MinBid;
                options.Value.RelayCheck = Program.CLISettings.RelayCheck;
                options.Value.SetLoglevel(Program.CLISettings.LogLevel);



                /*Not implemented:
                 * RELAY_MONITORS skip
                 * GENESIS_TIMESTAMP skip
                */
                if (Environment.GetEnvironmentVariable("BOOST_LISTEN_ADDR") is not null)
                    options.Value.HostPort = Environment.GetEnvironmentVariable("BOOST_LISTEN_ADDR").Split(",").ToList();

                if (Environment.GetEnvironmentVariable("LOG_LEVEL") is not null)
                    options.Value.SetLoglevel(Environment.GetEnvironmentVariable("LOG_LEVEL"));

                if (Environment.GetEnvironmentVariable("RELAY_STARTUP_CHECK") is not null)
                    options.Value.RelayCheck = bool.Parse(Environment.GetEnvironmentVariable("RELAY_STARTUP_CHECK"));

                if (Environment.GetEnvironmentVariable("MIN_BID_ETH") is not null)
                    options.Value.MinBid = decimal.Parse(Environment.GetEnvironmentVariable("MIN_BID_ETH"));

                if (Environment.GetEnvironmentVariable("DEBUG") is not null)
                {
                    var debug = Environment.GetEnvironmentVariable("DEBUG");
                    if (debug is not null)
                        options.Value.SetLoglevel("Debug");
                }
                if (Environment.GetEnvironmentVariable("LOG_SERVICE_TAG") is not null)
                    options.Value.LogServiceTag = Environment.GetEnvironmentVariable("LOG_SERVICE_TAG");
                if (Environment.GetEnvironmentVariable("RELAYS") is not null)
                    options.Value.RelayUrls = Environment.GetEnvironmentVariable("RELAYS").Split(",").ToList();
                if (Environment.GetEnvironmentVariable("REQUEST_MAX_RETRIES") is not null)
                    options.Value.HttpRetryCount = int.Parse(Environment.GetEnvironmentVariable("REQUEST_MAX_RETRIES"));
                if (Environment.GetEnvironmentVariable("GENESIS_FORK_VERSION") is not null)
                    options.Value.GenesisForkVersion = Environment.GetEnvironmentVariable("GENESIS_FORK_VERSION");
                if (Environment.GetEnvironmentVariable("network") is not null) 
                    options.Value.Network = Environment.GetEnvironmentVariable("network");
                if (Environment.GetEnvironmentVariable("RELAY_TIMEOUT_MS_GETHEADER") is not null)
                    options.Value.RequestTimeoutGetheader = int.Parse(Environment.GetEnvironmentVariable("RELAY_TIMEOUT_MS_GETHEADER"));
                if (Environment.GetEnvironmentVariable("RELAY_TIMEOUT_MS_GETPAYLOAD") is not null)
                    options.Value.RequestTimeoutGetpayload = int.Parse(Environment.GetEnvironmentVariable("RELAY_TIMEOUT_MS_GETPAYLOAD"));
                if (Environment.GetEnvironmentVariable("RELAY_TIMEOUT_MS_REGVAL") is not null)
                    options.Value.RequestTimeoutRegval = int.Parse(Environment.GetEnvironmentVariable("RELAY_TIMEOUT_MS_REGVAL"));

                return options?.Value
                    ?? throw new InvalidOperationException("AppSettings is not configured.");
            });
      
            services.AddControllers().AddNewtonsoftJson();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddHttpContextAccessor();
            services.AddScoped<IAPIBuilderFactory>(
               (builder) =>
               {
                   var settings = builder.GetService<AppSettings>();
                   var logger = builder.GetService<ILogger<EthHeader>>();
                   var httpLogger = builder.GetService<ILogger<HttpClient>>();
                   var clientFactory = builder.GetService<IHttpClientFactory>();
                   var httpContext = builder.GetService<IHttpContextAccessor>();

                   StringValues userAgent = new StringValues();
                   httpContext?.HttpContext?.Request?.Headers?.TryGetValue(
                       "User-Agent",
                       out userAgent
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
                       if (userAgent.Any())
                           client.DefaultRequestHeaders.TryAddWithoutValidation(
                               "User-Agent",
                               userAgent[0]
                           );
                       var resource = new BuilderAPIResource(
                           client,
                           settings.RequestTimeoutGetheader,
                           settings.RequestTimeoutGetpayload,
                           settings.RequestTimeoutRegval,
                           httpLogger
                       );
                       var clientResource = new BuilderAPIClient(resource);
                       clients.Add(clientResource);
                   }
                   return new APIBuilderFactory(logger, settings, clients);
               }
           );

            services.AddScoped<INotificationService, NotificationService>(builder =>
            {
                var service = new NotificationService();
                if(!string.IsNullOrEmpty(Program.CLISettings?.ZapierID))
                {
                    var zapierLogger = builder.GetService<ILogger<ZapierResource>>();
                    var zapierClient = new HttpClient() { BaseAddress = new Uri("https://hooks.zapier.com") };
                    IZapierResource zapierResource = new ZapierResource(zapierClient, zapierLogger, Program.CLISettings.ZapierID, Program.CLISettings.ZapierSecret);
                    service.Add(zapierResource);
                }

                if(!string.IsNullOrEmpty(Program.CLISettings?.TelegramAPI))
                {
                    var telegramLogger = builder.GetService<ILogger<TelegramResource>>();
                    var telegramClient = new HttpClient() { BaseAddress = new Uri("https://api.telegram.org") };
                    ITelegramResource telegramResource = new TelegramResource(telegramClient, telegramLogger, Program.CLISettings.TelegramAPI, Program.CLISettings.TelegramChatID);
                    service.Add(telegramResource);
                }
                return service;
            });

#if DEBUG
            services.AddMvc().AddApplicationPart(typeof(Startup).Assembly);
#endif
            AddConfigureServices(services);
        }

        protected virtual void DatabaseConfiguration(IServiceCollection services) { }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<BenchmarkMiddleware>();
            app.UseMiddleware<LoggingMiddleware>();
            app.UseSwaggerUI();
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseHttpsRedirection();
        }
    }
}
