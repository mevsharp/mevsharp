using MEVSharp.Application.Configurations;
using MEVSharp.Features.Http.Clients;
using MEVSharp.Features.Http.Clients.Dtos;
using MEVSharp.Features.Http.Clients.Dtos.BlindedBlocks;
using MEVSharp.Features.Http.Clients.Resources.APIBuilder;
using Microsoft.Extensions.Logging;
using System.Net;

namespace MEVSharp.Application.Models
{
    public class ApiRelay : IApiRelay
    {
        private readonly ILogger logger;
        public IBuilderAPIClient Client { get; }
        private readonly AppSettings appSettings;
        CancellationTokenSource sr;

        public ApiRelay(ILogger logger, IBuilderAPIClient client, AppSettings appSettings)
        {
            this.logger = logger;
            this.Client = client;
            this.appSettings = appSettings;
            sr = new CancellationTokenSource(appSettings.RequestTimeoutGetpayload);
        }

        public IBuilderAPIResource Resource => Client.Resource;

        public async Task<IRelayResult<EthHeader>> GetHeader(
            string slot,
            string parentHash,
            string pubkey
        )
        {
            try
            {
                var response = await Client.Resource.GetHeaderAsync(slot, parentHash, pubkey);

                logger.LogInformation(
                    $"getHeader request sent to relay {slot}, {parentHash} {pubkey}"
                );
                logger.LogInformation(
                    $"getHeader request sent to relay {Client.Resource.Client.BaseAddress}. Slot: {slot}, ParentHash: {parentHash}, Pubkey: {pubkey}"
                );

                var pubKey = Client.Resource.Client.BaseAddress.ToString().Split("@")[0].Split(
                    "://"
                )[1];
                var model = new EthHeader(logger, pubKey, parentHash, response, appSettings);
                return new RelayResult<EthHeader>(Client, model);
            }
            catch (HttpRequestException e)
            {
                logger.LogError($"{Client.Resource.Client.BaseAddress}  {e.StatusCode}");
                throw e;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<IRelayResult<HttpStatusCode>> GetStatus()
        {
            try
            {
                var response = await Client.Resource.StatusAsync(0, sr.Token);
                return new RelayResult<HttpStatusCode>(
                    Client,
                    (response != null ? response.StatusCode : HttpStatusCode.ServiceUnavailable)
                );
            }
            catch (TaskCanceledException e)
            {
                throw;
            }
        }

        public async Task<IRelayResult<HttpResponseMessage>> RegisterValidator(
            List<RegisterValidatorRequestDTO> registerValidatorRequestInner,
            int operationIndex = 0
        )
        {
            var response = await Client.Resource.RegisterValidatorAsync(
                registerValidatorRequestInner,
                operationIndex
            );
            return new RelayResult<HttpResponseMessage>(Client, response);
        }

        public async Task<IRelayResult<BlindedBlockResponse>> SubmitBlindedBlock(
            BlindedBlockRequest request
        )
        {
            try
            {
                var response = await Client.Resource.SubmitBlindedBlockAsync(request);
                return new RelayResult<BlindedBlockResponse>(Client, response);
            }
            catch (HttpProtocolException e)
            {
                logger.LogError($"{Client.Resource.Client.BaseAddress}  {e.Message}");
                throw e;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
