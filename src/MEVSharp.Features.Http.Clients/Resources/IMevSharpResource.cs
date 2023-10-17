using MEVSharp.Features.Http.Clients.Dtos;
using MEVSharp.Features.Http.Clients.Dtos.BlindedBlocks;

namespace MEVSharp.Features.Http.Clients.Resources
{
    public interface IMevSharpResource
    {
        Task<(HttpResponseMessage Response, int Entity)> GetStatus();
        Task<HttpResponseMessage> Register(List<RegisterValidatorRequestDTO> request);
        Task<(HttpResponseMessage Response, GetHeaderResponseDTO Entity)> GetHeader(
            string slot,
            string parentHash,
            string pubkey
        );
        Task<(HttpResponseMessage Response, BlindedBlockResponse Entity)> GetPayload(
            BlindedBlockRequest request
        );
    }
}
