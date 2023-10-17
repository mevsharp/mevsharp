using AutoMapper;
using MEVSharp.Application.Configurations;
using MEVSharp.Features.Http.Clients;
using MEVSharp.Features.Http.Clients.Resources.APIBuilder;

namespace MEVSharp.Tests.Integrations.Features.Shared
{
    internal class BuilderAPIMock : IBuilderAPIClient
    {
        private readonly IMapper mapper;
        private readonly AppSettings appsettings;

        public IBuilderAPIResource Resource { get; }

        public BuilderAPIMock(IBuilderAPIResource resource)
        {
            this.Resource = resource;
        }
    }
}
