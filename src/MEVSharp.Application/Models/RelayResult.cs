using MEVSharp.Features.Http.Clients;

namespace MEVSharp.Application.Models
{
    public class RelayResult<T> : IRelayResult<T>
    {
        public IBuilderAPIClient Client { get; }
        public T Result { get; }

        public RelayResult(IBuilderAPIClient client, T result)
        {
            this.Client = client;
            this.Result = result;
        }
    }
}
