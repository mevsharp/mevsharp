using Newtonsoft.Json;

namespace MEVSharp.Features.Http.Clients.Dtos.BlindedBlocks
{
    public class BlindedBlockResponse
    {
        public string version { get; set; }

        [JsonProperty("data")]
        public BlindedBlocksDataResponseDTO Data { get; set; }
    }

    public class BlindedBlocksDataResponseDTO
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

        [JsonProperty("block_hash")]
        public string BlockHash { get; set; }
        public string[] transactions { get; set; }
        public WithdrawalResponse[] withdrawals { get; set; }
    }

    public class WithdrawalResponse
    {
        public string index { get; set; }
        public string validator_index { get; set; }
        public string address { get; set; }
        public string amount { get; set; }
    }
}
