using MEVSharp.Application.Factories;
using MEVSharp.Application.Models;
using MEVSharp.Features.Http.Clients.Dtos;
using MEVSharp.Features.Http.Clients.Dtos.BlindedBlocks;
using System.Net;

namespace MEVSharp.Application.Providers
{
    public interface IRelayProvider
    {
        IAPIBuilderFactory Factory { get; }
        Task<IEnumerable<IRelayResult<EthHeader>>> GetHeaders(
            string slot,
            string parentHash,
            string pubkey
        );
        Task<IEnumerable<IRelayResult<HttpStatusCode>>> GetStatuses();
        Task<IEnumerable<IRelayResult<HttpResponseMessage>>> RegisterValidator(
            List<RegisterValidatorRequestDTO> registerValidatorRequestInner,
            int operationIndex = 0
        );
        Task<
            IEnumerable<IRelayResult<Features.Http.Clients.Dtos.BlindedBlocks.BlindedBlockResponse>>
        > SubmitBlindedBlocks(BlindedBlockRequest request);
    }
}
