using Nethermind.Core;

namespace MEVSharp.Application.Dtos
{
    public class BlindedBlocksResponse
    {
        public string Version { get; set; }
        public Data Data { get; set; }
    }

    public class Data
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
        public string[] transactions { get; set; }
        public IEnumerable<Withdrawal> withdrawals { get; set; }
    }

    public class WithdrawalResponse
    {
        public string Index { get; set; }
        public string ValidatorIndex { get; set; }
        public string Address { get; set; }
        public string Amount { get; set; }
    }
}
