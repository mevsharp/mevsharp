using Newtonsoft.Json;

namespace MEVSharp.Features.Http.Clients.Dtos
{
    public class RegisterValidatorResponseDTO
    {
        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }
    }

    public class RegisterValidatorRequestDTO
    {
        [JsonProperty("message")]
        public RegisterValidatorRequestMessageDTO Message { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }
    }

    public class RegisterValidatorRequestMessageDTO
    {
        [JsonProperty("fee_recipient")]
        public string FeeRecipient { get; set; }

        [JsonProperty("gas_limit")]
        public string GasLimit { get; set; }

        [JsonProperty("timestamp")]
        public string TimeStamp { get; set; }

        [JsonProperty("pubkey")]
        public string PubKey { get; set; }
    }
}
