using MEVSharp.Application.Models;
using MEVSharp.Features.Http.Clients.Dtos;
using MEVSharp.Tests.Integrations.Features.Shared;
using Newtonsoft.Json;

namespace MEVSharp.Tests.Integrations.Features.SSZTests
{
    public class SSZTest : IClassFixture<IntegrationMockHttpContextTestBase>
    {
        ISignatureVerification signatureVerification;

        public SSZTest(IntegrationMockHttpContextTestBase fixture)
        {
            signatureVerification = fixture.GetService<ISignatureVerification>();
        }

        [Fact]
        public void should_be_able_to_verify_domain_signingroot_and_signature()
        {
            string jsonResponse = File.ReadAllText("Assets/get_header_response_goerli.json");
            var dto = JsonConvert.DeserializeObject<GetHeaderResponseDTO>(jsonResponse);
            var success = signatureVerification.Verify(dto);
            Assert.True(success);
        }

        [Fact]
        public void should_not_be_able_to_verify_domain_signingroot_and_signature()
        {
            string jsonResponse = File.ReadAllText("Assets/get_header_response_invalid.json");
            var dto = JsonConvert.DeserializeObject<GetHeaderResponseDTO>(jsonResponse);
            var success = signatureVerification.Verify(dto);
            Assert.False(success);
        }
    }
}
