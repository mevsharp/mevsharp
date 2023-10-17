using MEVSharp.Features.Http.Clients.Dtos;
using MEVSharp.Features.Http.Clients.Dtos.BlindedBlocks;
using MEVSharp.Features.Http.Clients.Resources;
using MEVSharp.Features.Http.Clients.Resources.APIBuilder;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MEVSharp.Tests.Integrations.Features
{
    internal class BuilderAPIResourceMOCK : ResourceBase, IBuilderAPIResource
    {
        private readonly ILogger logger;

        public BuilderAPIResourceMOCK(HttpClient client, ILogger logger)
            : base(client, logger)
        {
            this.logger = logger;
        }

        public async Task<(
            HttpResponseMessage Response,
            IEnumerable<GetHeaderResponseDTO> Entity
        )> GetHeader(string Slot, string ParentHash, string Pubkey)
        {
            var response = Client.GetAsync($"/eth/v1/builder/header/{Slot}/{ParentHash}/{Pubkey}");
            var result = await Invoke<IEnumerable<GetHeaderResponseDTO>>(response);
            return result;
        }

        public Task<GetHeaderResponseDTO> GetHeaderAsync(
            string slot,
            string parentHash,
            string pubkey,
            int operationIndex = 0,
            CancellationToken cancellationToken = default
        )
        {
            throw new NotImplementedException();
        }

        public Task RegisterValidatorAsync(
            List<RegisterValidatorRequestDTO> registerValidatorRequestInner,
            int operationIndex = 0,
            CancellationToken cancellationToken = default
        )
        {
            throw new NotImplementedException();
        }

        public Task StatusAsync(
            int operationIndex = 0,
            CancellationToken cancellationToken = default
        )
        {
            throw new NotImplementedException();
        }

        public async Task<BlindedBlockResponse> SubmitBlindedBlockAsync(
            BlindedBlockRequest submitBlindedBlockRequest,
            string ethConsensusVersion = null,
            int operationIndex = 0,
            CancellationToken cancellationToken = default
        )
        {
            string jsonResponse = File.ReadAllText("Assets/blinded_blocks_response_goerli.json");
            var dto = JsonConvert.DeserializeObject<BlindedBlockResponse>(jsonResponse);
            return dto;
        }

        Task<HttpResponseMessage> IBuilderAPIResource.RegisterValidatorAsync(
            List<RegisterValidatorRequestDTO> registerValidatorRequestInner,
            int operationIndex,
            CancellationToken cancellationToken
        )
        {
            throw new NotImplementedException();
        }

        Task<HttpResponseMessage> IBuilderAPIResource.StatusAsync(
            int operationIndex,
            CancellationToken cancellationToken
        )
        {
            throw new NotImplementedException();
        }
    }
}
