global using Xunit;

public class Rootobject
{
    public Message message { get; set; }
    public string signature { get; set; }
}

public class Message
{
    public string slot { get; set; }
    public string proposer_index { get; set; }
    public string parent_root { get; set; }
    public string state_root { get; set; }
    public Body body { get; set; }
}

public class Body
{
    public string randao_reveal { get; set; }
    public Eth1_Data eth1_data { get; set; }
    public string graffiti { get; set; }
    public Proposer_Slashings[] proposer_slashings { get; set; }
    public Attester_Slashings[] attester_slashings { get; set; }
    public Attestation[] attestations { get; set; }
    public Deposit[] deposits { get; set; }
    public Voluntary_Exits[] voluntary_exits { get; set; }
    public Sync_Aggregate sync_aggregate { get; set; }
    public Execution_Payload_Header execution_payload_header { get; set; }
    public Bls_To_Execution_Changes[] bls_to_execution_changes { get; set; }
}

public class Eth1_Data
{
    public string deposit_root { get; set; }
    public string deposit_count { get; set; }
    public string block_hash { get; set; }
}

public class Sync_Aggregate
{
    public string sync_committee_bits { get; set; }
    public string sync_committee_signature { get; set; }
}

public class Execution_Payload_Header
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

public class Proposer_Slashings
{
    public Signed_Header_1 signed_header_1 { get; set; }
    public Signed_Header_2 signed_header_2 { get; set; }
}

public class Signed_Header_1
{
    public Message1 message { get; set; }
    public string signature { get; set; }
}

public class Message1
{
    public string slot { get; set; }
    public string proposer_index { get; set; }
    public string parent_root { get; set; }
    public string state_root { get; set; }
    public string body_root { get; set; }
}

public class Signed_Header_2
{
    public Message2 message { get; set; }
    public string signature { get; set; }
}

public class Message2
{
    public string slot { get; set; }
    public string proposer_index { get; set; }
    public string parent_root { get; set; }
    public string state_root { get; set; }
    public string body_root { get; set; }
}

public class Attester_Slashings
{
    public Attestation_1 attestation_1 { get; set; }
    public Attestation_2 attestation_2 { get; set; }
}

public class Attestation_1
{
    public string[] attesting_indices { get; set; }
    public string signature { get; set; }
    public Data data { get; set; }
}

public class Data
{
    public string slot { get; set; }
    public string index { get; set; }
    public string beacon_block_root { get; set; }
    public Source source { get; set; }
    public Target target { get; set; }
}

public class Source
{
    public string epoch { get; set; }
    public string root { get; set; }
}

public class Target
{
    public string epoch { get; set; }
    public string root { get; set; }
}

public class Attestation_2
{
    public string[] attesting_indices { get; set; }
    public string signature { get; set; }
    public Data1 data { get; set; }
}

public class Data1
{
    public string slot { get; set; }
    public string index { get; set; }
    public string beacon_block_root { get; set; }
    public Source1 source { get; set; }
    public Target1 target { get; set; }
}

public class Source1
{
    public string epoch { get; set; }
    public string root { get; set; }
}

public class Target1
{
    public string epoch { get; set; }
    public string root { get; set; }
}

public class Attestation
{
    public string aggregation_bits { get; set; }
    public string signature { get; set; }
    public Data2 data { get; set; }
}

public class Data2
{
    public string slot { get; set; }
    public string index { get; set; }
    public string beacon_block_root { get; set; }
    public Source2 source { get; set; }
    public Target2 target { get; set; }
}

public class Source2
{
    public string epoch { get; set; }
    public string root { get; set; }
}

public class Target2
{
    public string epoch { get; set; }
    public string root { get; set; }
}

public class Deposit
{
    public string[] proof { get; set; }
    public Data3 data { get; set; }
}

public class Data3
{
    public string pubkey { get; set; }
    public string withdrawal_credentials { get; set; }
    public string amount { get; set; }
    public string signature { get; set; }
}

public class Voluntary_Exits
{
    public Message3 message { get; set; }
    public string signature { get; set; }
}

public class Message3
{
    public string epoch { get; set; }
    public string validator_index { get; set; }
}

public class Bls_To_Execution_Changes
{
    public Message4 message { get; set; }
    public string signature { get; set; }
}

public class Message4
{
    public string validator_index { get; set; }
    public string from_bls_pubkey { get; set; }
    public string to_execution_address { get; set; }
}
