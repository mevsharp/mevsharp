using MEVSharp.Features.Http.Clients.Dtos;
using MEVSharp.Features.Http.Clients.Dtos.BlindedBlocks;
using MEVSharp.Features.Http.Clients.Resources.APIBuilder;
using Microsoft.Extensions.Logging;

namespace MEVSharp.Features.Http.Clients.Resources
{
    public class BuilderAPIResource : ResourceBase, IBuilderAPIResource
    {
        private readonly HttpClient client;
        private readonly int requestTimeoutGetheader;
        private readonly int requestTimeoutGetpayload;
        private readonly int requestTimeoutRegval;
        private readonly ILogger logger;

        private readonly CancellationTokenSource headerTc;
        private readonly CancellationTokenSource payloadTc;
        private readonly CancellationTokenSource validationTc;

        public BuilderAPIResource(
            HttpClient client,
            int RequestTimeoutGetheader,
            int RequestTimeoutGetpayload,
            int RequestTimeoutRegval,
            ILogger logger
        )
            : base(client, logger)
        {
            this.client = client;
            requestTimeoutGetheader = RequestTimeoutGetheader;
            requestTimeoutGetpayload = RequestTimeoutGetpayload;
            requestTimeoutRegval = RequestTimeoutRegval;
            this.logger = logger;

            headerTc = new CancellationTokenSource(requestTimeoutGetheader);
            payloadTc = new CancellationTokenSource(RequestTimeoutGetpayload);
            validationTc = new CancellationTokenSource(RequestTimeoutRegval);
        }

        public async Task<(
            HttpResponseMessage Response,
            IEnumerable<GetHeaderResponseDTO> Entity
        )> GetHeader(
            string Slot,
            string ParentHash,
            string Pubkey,
            CancellationToken cancellationToken = default
        )
        {
            var payload = Client.GetAsync(
                $"/eth/v1/builder/header/{Slot}/{ParentHash}/{Pubkey}",
                headerTc.Token
            );
            var result = await Invoke<IEnumerable<GetHeaderResponseDTO>>(payload);
            return result;
        }

        public async Task<GetHeaderResponseDTO> GetHeaderAsync(
            string slot,
            string parentHash,
            string pubkey,
            int operationIndex = 0,
            CancellationToken cancellationToken = default
        )
        {
            var payload = Client.GetAsync(
                $"/eth/v1/builder/header/{slot}/{parentHash}/{pubkey}",
                headerTc.Token
            );
            var result = await Invoke<GetHeaderResponseDTO>(payload);
            return result.Entity;
        }

        public async Task<HttpResponseMessage> RegisterValidatorAsync(
            List<RegisterValidatorRequestDTO> registerValidatorRequestInner,
            int operationIndex = 0,
            CancellationToken cancellationToken = default
        )
        {
            var content = base.BuildRequestContent(registerValidatorRequestInner);
            var payload = Client.PostAsync(
                $"/eth/v1/builder/validators",
                content.Content,
                validationTc.Token
            );
            var result = await Invoke<object>(payload);
            return result.Message;
        }

        public async Task<HttpResponseMessage> StatusAsync(
            int operationIndex = 0,
            CancellationToken cancellationToken = default
        )
        {
            var payload = Client.GetAsync($"/eth/v1/builder/status", cancellationToken);
            var result = await Invoke<object>(payload);
            return result.Message;
        }

        public async Task<BlindedBlockResponse> SubmitBlindedBlockAsync(
            BlindedBlockRequest submitBlindedBlockRequest,
            string ethConsensusVersion = null,
            int operationIndex = 0,
            CancellationToken cancellationToken = default
        )
        {
            var content = base.BuildRequestContent(submitBlindedBlockRequest);
            logger.LogDebug($"url: /eth/v1/builder/blinded_blocks. Payload: {content.Json}");
            var payload = Client.PostAsync(
                $"/eth/v1/builder/blinded_blocks",
                content.Content,
                payloadTc.Token
            );
            var result = await Invoke<BlindedBlockResponse>(payload);
            return result.Entity;
        }
    }
}
