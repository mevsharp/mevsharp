using MEVSharp.Application.Configurations;
using MEVSharp.Application.Models;
using MEVSharp.Features.Http.Clients;
using Microsoft.Extensions.Logging;

namespace MEVSharp.Application.Factories
{
    public class APIBuilderFactory : IAPIBuilderFactory
    {
        Dictionary<string, IApiRelay> clients = new Dictionary<string, IApiRelay>();
        private readonly AppSettings appSettings;
        public IReadOnlyDictionary<string, IApiRelay> Clients
        {
            get => clients;
        }

        public APIBuilderFactory(
            ILogger logger,
            AppSettings appSettings,
            IEnumerable<IBuilderAPIClient> client
        )
        {
            foreach (var baseUrl in client)
            {
                clients.Add(
                    baseUrl.Resource.Client.BaseAddress.ToString(),
                    new ApiRelay(logger, baseUrl, appSettings)
                );
            }
            this.appSettings = appSettings;
        }

        IApiRelay IAPIBuilderFactory.GetByUrl(string url)
        {
            return Clients[url];
        }
    }
}
