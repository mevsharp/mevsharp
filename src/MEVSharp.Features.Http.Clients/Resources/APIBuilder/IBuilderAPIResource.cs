using MEVSharp.Features.Http.Clients.Dtos;
using MEVSharp.Features.Http.Clients.Dtos.BlindedBlocks;

namespace MEVSharp.Features.Http.Clients.Resources.APIBuilder
{
    public interface IBuilderAPIResource : IResourceBase
    {
        Task<GetHeaderResponseDTO> GetHeaderAsync(
            string slot,
            string parentHash,
            string pubkey,
            int operationIndex = 0,
            CancellationToken cancellationToken = default(CancellationToken)
        );
        Task<HttpResponseMessage> StatusAsync(
            int operationIndex = 0,
            CancellationToken cancellationToken = default(CancellationToken)
        );
        Task<HttpResponseMessage> RegisterValidatorAsync(
            List<RegisterValidatorRequestDTO> registerValidatorRequestInner,
            int operationIndex = 0,
            System.Threading.CancellationToken cancellationToken =
                default(System.Threading.CancellationToken)
        );
        Task<BlindedBlockResponse> SubmitBlindedBlockAsync(
            BlindedBlockRequest submitBlindedBlockRequest,
            string ethConsensusVersion = default(string),
            int operationIndex = 0,
            System.Threading.CancellationToken cancellationToken =
                default(System.Threading.CancellationToken)
        );
    }
}
