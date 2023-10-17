using MEVSharp.Features.Http.Clients.Dtos;
using MEVSharp.Features.Http.Clients.Dtos.BlindedBlocks;
using Microsoft.Extensions.Logging;

namespace MEVSharp.Features.Http.Clients.Resources
{
    public class MevSharpResource : ResourceBase, IMevSharpResource
    {
        public MevSharpResource(HttpClient client, ILogger logger)
            : base(client, logger) { }

        public async Task<(HttpResponseMessage Response, GetHeaderResponseDTO Entity)> GetHeader(
            string Slot,
            string ParentHash,
            string Pubkey
        )
        {
            var response = Client.GetAsync($"/eth/v1/builder/header/{Slot}/{ParentHash}/{Pubkey}");
            var result = await Invoke<GetHeaderResponseDTO>(response);
            return result;
        }

        public async Task<(HttpResponseMessage Response, BlindedBlockResponse Entity)> GetPayload(
            BlindedBlockRequest request
        )
        {
            var payLoad = BuildRequestContent(request);
            var response = Client.PostAsync($"/eth/v1/builder/blinded_blocks", payLoad.Content);
            var result = await Invoke<BlindedBlockResponse>(response);
            return result;
        }

        public async Task<(HttpResponseMessage Response, int Entity)> GetStatus()
        {
            var response = Client.GetAsync($"eth/v1/builder/status/");
            var result = await Invoke<int>(response);
            return result;
        }

        public async Task<HttpResponseMessage> Register(List<RegisterValidatorRequestDTO> request)
        {
            var payLoad = base.BuildRequestContent(request);
            var response = Client.PostAsync($"/eth/v1/builder/validators", payLoad.Content);
            var result = await Invoke<object>(response);
            return result.Message;
        }
    }
}
