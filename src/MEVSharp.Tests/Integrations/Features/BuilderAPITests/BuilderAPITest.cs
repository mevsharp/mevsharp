using MEVSharp.Application.Models;
using MEVSharp.Features.Http.Clients;
using MEVSharp.Features.Http.Clients.Dtos;
using MEVSharp.Features.Http.Clients.Dtos.BlindedBlocks;
using MEVSharp.Features.Http.Clients.Resources;
using MEVSharp.Tests.Integrations.Features.Shared;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace MEVSharp.Tests.Integrations.Features.BuilderAPITests
{
    public class BuilderAPITest : IClassFixture<IntegrationMockHttpContextTestBase>
    {
        private readonly IMevSharpClient client;

        public BuilderAPITest(IntegrationMockHttpContextTestBase fixture)
        {
            var logger = fixture.GetService<ILogger<HttpClient>>();
            client = new MevSharpClient(new MevSharpResource(fixture.Client, logger));
        }

        [Fact]
        public void calculate_and_verify_blockhash()
        {
            var requestRaw = File.ReadAllText("Assets/blinded_blocks_goerli.json");
            var responseRaw = File.ReadAllText("Assets/blinded_blocks_response_goerli.json");
            var request = JsonConvert.DeserializeObject<BlindedBlockRequest>(requestRaw);
            var response = JsonConvert.DeserializeObject<BlindedBlockResponse>(responseRaw);

            ICalculateBlockHash calculate = new CalculateBlockHash(request, response);
            var isValid = calculate.IsValid();
            Assert.True(isValid);
        }

        #if DEBUG
        [Fact]
        public async Task getStatus_api_should_reply_with_200_ok()
        {
            var response = await client.Resource.GetStatus();
            HttpStatusCode expected = HttpStatusCode.OK;
            Assert.Equal((int)expected, response.Entity);
        }
        #endif

        [Fact]
        public async Task should_not_throw_an_ParentHashMismatchException()
        {
            string slot = "1";
            string parentHash =
                "0x08f492d858e1fa8a548cefe94f90d5073de7a65afd569388ad2f2a337db908a4";
            string pubkey =
                "0xa7ab7a996c8584251c8f925da3170bdfd6ebc75d50f5ddc4050a6fdc77f2a3b5fce2cc750d0865e05d7228af97d69561";
            var response = await client.Resource.GetHeader(slot, parentHash, pubkey);
            Assert.NotNull(response);
        }

        #if DEBUG
        [Fact]
        public async Task register_api_should_reply_with_200_ok()
        {
            var json = File.ReadAllText("Assets/register_validator.json");
            var entity = JsonConvert.DeserializeObject<List<RegisterValidatorRequestDTO>>(json);
            var response = await client.Resource.Register(entity);
            Assert.True(response.StatusCode == HttpStatusCode.OK);
        }
        #endif

        [Fact]
        public async Task BlindedBlock()
        {
            var json2 = File.ReadAllText("Assets/get_header_response_goerli.json");
            var headerResponse = JsonConvert.DeserializeObject<GetHeaderResponseDTO>(json2);

            string slot = "5996137";
            string parentHash = headerResponse.Data.Message.header.parent_hash;
            string pubkey = headerResponse.Data.Message.pubkey;
            var getHeaderResponse = await client.Resource.GetHeader(slot, parentHash, pubkey);
            Assert.True(getHeaderResponse.Response.IsSuccessStatusCode);
            var json = File.ReadAllText("Assets/blinded_blocks_goerli.json");
            var entity = JsonConvert.DeserializeObject<BlindedBlockRequest>(json);
            var response = await client.Resource.GetPayload(entity);
            Assert.NotNull(response);
        }
    }
}
