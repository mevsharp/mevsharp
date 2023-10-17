using System.Text.Json.Serialization;

namespace MEVSharp.Features.Http.Clients.Dtos
{
    public class RegisterValidatorRequestInnerRequest
    {
        [JsonPropertyName("message")]
        public RegisterValidatorRequestMessageRequest Message { get; set; }

        [JsonPropertyName("signature")]
        public string Signature { get; set; }

        public RegisterValidatorRequestInnerRequest()
        {
            Message = new RegisterValidatorRequestMessageRequest();
            Signature = string.Empty;
        }
    }

    public class RegisterValidatorRequestMessageRequest
    {
        [JsonPropertyName("fee_recipient")]
        public string FeeRecipient { get; set; }

        [JsonPropertyName("gas_limit")]
        public string GasLimit { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }

        [JsonPropertyName("pubkey")]
        public string PubKey { get; set; }

        public RegisterValidatorRequestMessageRequest()
        {
            FeeRecipient = string.Empty;
            GasLimit = string.Empty;
            Timestamp = string.Empty;
            PubKey = string.Empty;
        }
    }
}
