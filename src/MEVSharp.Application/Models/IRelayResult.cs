using MEVSharp.Features.Http.Clients;

namespace MEVSharp.Application.Models
{
    public interface IRelayResult<T>
    {
        IBuilderAPIClient Client { get; }
        T Result { get; }


    }
}
