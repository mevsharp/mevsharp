using MEVSharp.Features.Http.Clients.Resources;

namespace MEVSharp.Features.Http.Clients
{
    public class MevSharpClient : IMevSharpClient
    {
        public MevSharpClient(IMevSharpResource resource)
        {
            this.Resource = resource;
        }

        public IMevSharpResource Resource { get; private set; }
    }

    public interface IMevSharpClient
    {
        IMevSharpResource Resource { get; }
    }
}
