using MEVSharp.Features.Http.Clients.Dtos.BlindedBlocks;
using Microsoft.Extensions.Logging;

namespace MEVSharp.Application.Models
{
    public class EthSubmitBlindedBlock : EthBase
    {
        private readonly ILogger logger;
        private IApiRelay relay;
        private readonly BlindedBlockRequest blockRequest;

        public EthSubmitBlindedBlock(
            ILogger logger,
            IApiRelay relay,
            BlindedBlockRequest blockRequest
        )
        {
            this.logger = logger;
            this.relay = relay;
            this.blockRequest = blockRequest;
        }

        public async Task<BlindedBlockResponse> SubmitBlindedBlock()
        {
            if (
                blockRequest.Message == null
                || blockRequest.Message.Body == null
                || blockRequest.Message.Body.ExecutionPayloadHeader == null
            )
            {
                if (
                    string.IsNullOrEmpty(blockRequest.Message.Body.ExecutionPayloadHeader.BlockHash)
                )
                {
                    logger.LogCritical(
                        "No bid for this getPayload payload found. Was getHeader called before?"
                    );
                }
            }
            try
            {
                var responsePayload = await relay.Client.Resource.SubmitBlindedBlockAsync(
                    blockRequest
                );
                if (responsePayload == null)
                {
                    logger.LogError("Response with empty data!");
                }

                var blockResponse = responsePayload;

                if (
                    responsePayload.version == "capella"
                    || !base.IsValidBlock(blockResponse.Data.BlockHash)
                )
                {
                    logger.LogError("Response with empty data!");
                }

                if (
                    blockRequest.Message.Body.ExecutionPayloadHeader.BlockHash
                    != blockResponse.Data.BlockHash
                )
                {
                    logger.LogError("requestBlockHash does not equal responseBlockHash!");
                }
                return responsePayload;
            }
            catch (Exception e)
            {
                logger.LogCritical(e, "Error while submitting blinded block");
                throw;
            }
        }
    }
}
