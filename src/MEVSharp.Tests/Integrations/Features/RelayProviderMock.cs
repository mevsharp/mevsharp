using MEVSharp.Application.Configurations;
using MEVSharp.Application.Factories;
using MEVSharp.Application.Models;
using MEVSharp.Application.Providers;
using MEVSharp.Features.Http.Clients;
using MEVSharp.Features.Http.Clients.Dtos;
using MEVSharp.Features.Http.Clients.Dtos.BlindedBlocks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace MEVSharp.Tests.Integrations.Features
{
    public class RelayProviderMock : IRelayProvider
    {
        private readonly ILogger logger;
        private readonly IBuilderAPIClient client;
        private readonly AppSettings appSettings;
        private readonly IAPIBuilderFactory factory;

        public RelayProviderMock(
            IAPIBuilderFactory factory,
            ILogger logger,
            IBuilderAPIClient client,
            AppSettings appSettings
        )
        {
            this.logger = logger;
            this.client = client;
            this.appSettings = appSettings;
            this.factory = factory;
        }

        public IAPIBuilderFactory Factory => factory;

        public async Task<IEnumerable<IRelayResult<EthHeader>>> GetHeaders(
            string slot,
            string parentHash,
            string pubkey
        )
        {
            string jsonResponse = File.ReadAllText("Assets/get_header_response_goerli.json");
            var dto = JsonConvert.DeserializeObject<GetHeaderResponseDTO>(jsonResponse);

            var pubKey = client.Resource.Client.BaseAddress.ToString().Split("@")[0].Split("://")[
                1
            ];

            var model = new EthHeader(logger, pubKey, parentHash, dto, appSettings);
            List<IRelayResult<EthHeader>> items = new();
            items.Add(new RelayResult<EthHeader>(client, model));
            items.Add(new RelayResult<EthHeader>(client, model));
            items.Add(new RelayResult<EthHeader>(client, model));
            return items;
        }

        public async Task<IEnumerable<IRelayResult<HttpStatusCode>>> GetStatuses()
        {
            List<IRelayResult<HttpStatusCode>> items = new();
            var httpClient = new HttpClient() { BaseAddress = new Uri("http://localhost.com") };
            BuilderAPIClient client = new BuilderAPIClient(
                new BuilderAPIResourceMOCK(httpClient, logger)
            );

            items.Add(new RelayResult<HttpStatusCode>(client, HttpStatusCode.OK));
            items.Add(new RelayResult<HttpStatusCode>(client, HttpStatusCode.BadGateway));
            items.Add(new RelayResult<HttpStatusCode>(client, HttpStatusCode.Unauthorized));
            return items;
        }

        public async Task<IEnumerable<IRelayResult<EthStatus>>> RegisterValidator(
            List<RegisterValidatorRequestInnerRequest> registerValidatorRequestInner,
            int operationIndex = 0
        )
        {
            List<IRelayResult<EthStatus>> items = new();

            items.Add(new RelayResult<EthStatus>(null, new EthStatus(HttpStatusCode.OK)));
            return items;
        }

        public async Task<IEnumerable<IRelayResult<HttpResponseMessage>>> RegisterValidator(
            List<RegisterValidatorRequestDTO> registerValidatorRequestInner,
            int operationIndex = 0
        )
        {
            HttpResponseMessage okResponse = new HttpResponseMessage(HttpStatusCode.OK);
            HttpResponseMessage BadGatwayResponse = new HttpResponseMessage(HttpStatusCode.BadGateway);

            List<IRelayResult<HttpResponseMessage>> items = new();
            items.Add(new RelayResult<HttpResponseMessage>(null, okResponse));
            items.Add(new RelayResult<HttpResponseMessage>(null, okResponse));
            items.Add(new RelayResult<HttpResponseMessage>(null, okResponse));
            return items;
        }

        public async Task<IEnumerable<IRelayResult<BlindedBlockResponse>>> SubmitBlindedBlocks(
            BlindedBlockRequest request
        )
        {
            List<IRelayResult<BlindedBlockResponse>> items = new();
            string jsonResponse = File.ReadAllText("Assets/blinded_blocks_response_goerli.json");
            var dto = JsonConvert.DeserializeObject<BlindedBlockResponse>(jsonResponse);
            var httpClient = new HttpClient() { BaseAddress = new Uri("http://localhost.com") };
            BuilderAPIClient client = new BuilderAPIClient(
                new BuilderAPIResourceMOCK(httpClient, logger)
            );
            items.Add(new RelayResult<BlindedBlockResponse>(client, dto));
            items.Add(new RelayResult<BlindedBlockResponse>(client, dto));
            items.Add(new RelayResult<BlindedBlockResponse>(client, dto));
            return items;
        }
    }
}
