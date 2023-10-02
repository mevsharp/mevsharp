using MEVSharp.Application.Models;
using MEVSharp.Features.Http.Clients;
using MEVSharp.Features.Http.Clients.Dtos;
using MEVSharp.Features.Http.Clients.Dtos.BlindedBlocks;
using MEVSharp.Features.Http.Clients.Resources.APIBuilder;
using System.Net;

namespace MEVSharp.Tests.Integrations.Features.Shared
{
    internal class ApiRelayMock : IApiRelay
    {
        public IBuilderAPIResource Resource => throw new NotImplementedException();

        public IBuilderAPIClient Client => throw new NotImplementedException();

        public Task<IRelayResult<EthHeader>> GetHeader(
            string slot,
            string parentHash,
            string pubkey
        )
        {
            throw new NotImplementedException();
        }

        public Task<IRelayResult<HttpStatusCode>> GetStatus()
        {
            throw new NotImplementedException();
        }

        public Task<IRelayResult<HttpResponseMessage>> RegisterValidator(
            List<RegisterValidatorRequestDTO> registerValidatorRequestInner,
            int operationIndex = 0
        )
        {
            throw new NotImplementedException();
        }

        public Task<IRelayResult<BlindedBlockResponse>> SubmitBlindedBlock(
            BlindedBlockRequest request
        )
        {
            throw new NotImplementedException();
        }
    }
}
