using MEVSharp.Features.Http.Clients;
using MEVSharp.Features.Http.Clients.Dtos;
using MEVSharp.Features.Http.Clients.Dtos.BlindedBlocks;
using MEVSharp.Features.Http.Clients.Resources.APIBuilder;
using System.Net;

namespace MEVSharp.Application.Models
{
    public interface IApiRelay
    {
        IBuilderAPIResource Resource { get; }
        IBuilderAPIClient Client { get; }
        Task<IRelayResult<EthHeader>> GetHeader(string slot, string parentHash, string pubkey);
        Task<IRelayResult<HttpResponseMessage>> RegisterValidator(
            List<RegisterValidatorRequestDTO> registerValidatorRequestInner,
            int operationIndex = 0
        );
        Task<IRelayResult<HttpStatusCode>> GetStatus();
        Task<
            IRelayResult<Features.Http.Clients.Dtos.BlindedBlocks.BlindedBlockResponse>
        > SubmitBlindedBlock(BlindedBlockRequest request);
    }
}
