using MEVSharp.Application.Configurations;
using MEVSharp.Application.Factories;
using MEVSharp.Application.Models;
using MEVSharp.Features.Http.Clients;
using MEVSharp.Features.Http.Clients.Handlers;
using MEVSharp.Features.Http.Clients.Resources;
using MEVSharp.UI.API.Middlewares;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;

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
                options.Value.MinBid = Program.CLISettings.MinBid;
                options.Value.RelayCheck = Program.CLISettings.RelayCheck;
                options.Value.SetLoglevel(Program.CLISettings.LogLevel);
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
