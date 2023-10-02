using Newtonsoft.Json;

namespace MEVSharp.Features.Http.Clients.Dtos
{
    public class GetHeaderResponseDTO
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; } = new Data();
    }

    public class Data
    {
        [JsonProperty("message")]
        public Message Message { get; set; } = new Message();

        [JsonProperty("signature")]
        public string Signature { get; set; }
    }

    public class Message
    {
        public Header header { get; set; } = new Header();
        public string value { get; set; }
        public string pubkey { get; set; }
    }

    public class Header
    {
        public string parent_hash { get; set; }
        public string fee_recipient { get; set; }
        public string state_root { get; set; }
        public string receipts_root { get; set; }
        public string logs_bloom { get; set; }
        public string prev_randao { get; set; }
        public string block_number { get; set; }
        public string gas_limit { get; set; }
        public string gas_used { get; set; }
        public string timestamp { get; set; }
        public string extra_data { get; set; }
        public string base_fee_per_gas { get; set; }
        public string block_hash { get; set; }
        public string transactions_root { get; set; }
        public string withdrawals_root { get; set; }
    }
}
