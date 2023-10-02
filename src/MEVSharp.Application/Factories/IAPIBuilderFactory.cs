using MEVSharp.Application.Models;

namespace MEVSharp.Application.Factories
{
    public interface IAPIBuilderFactory
    {
        IReadOnlyDictionary<string, IApiRelay> Clients { get; }
        IApiRelay GetByUrl(string url);
    }
}
