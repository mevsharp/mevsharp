using MEVSharp.UI.API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace MEVSharp.Tests.Integrations.Features.Shared
{
    public class IntegrationTestFixture<TStartup>
        where TStartup : Startup
    {
        public readonly TestServer _server;

        public readonly TStartup TestContext;
        public readonly HttpClient Client;

        public IntegrationTestFixture()
        {
            var projectDir =
                Directory
                    .GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
                    ?.Parent?.Parent?.Parent?.FullName + "//MEVSharp.UI.API";

            _server = new TestServer(
                new WebHostBuilder()
                    .UseContentRoot(projectDir)
                    .UseConfiguration(
                        new ConfigurationBuilder()
                            .SetBasePath(projectDir)
                            .AddJsonFile("appsettings.json")
                            .Build()
                    )
                    .UseStartup<TStartup>()
            );

            TestContext = (TStartup)
                Activator.CreateInstance(
                    typeof(TStartup),
                    _server.Host.Services.GetService(typeof(IConfiguration))
                );

            Client = _server.CreateClient();
        }

        public HttpClient CretateClient()
        {
            return _server.CreateClient();
        }

        public T GetService<T>()
        {
            return (T)_server.Host.Services.GetService(typeof(T));
        }
    }
}
