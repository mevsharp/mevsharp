using MEVSharp.Features.Http.Clients.Resources.APIBuilder;

namespace MEVSharp.Features.Http.Clients
{
    public class BuilderAPIClient : IBuilderAPIClient
    {
        public IBuilderAPIResource Resource { get; }

        public BuilderAPIClient(IBuilderAPIResource resource)
        {
            this.Resource = resource;
        }
    }

    public interface IBuilderAPIClient
    {
        IBuilderAPIResource Resource { get; }
    }
}
