using MEVSharp.Features.Http.Clients.Dtos.BlindedBlocks;
using Nethermind.Core;
using Nethermind.Core.Crypto;
using Nethermind.Core.Extensions;
using Nethermind.Int256;
using Nethermind.Serialization.Rlp;
using Nethermind.State.Proofs;

namespace MEVSharp.Application.Models
{
    public interface ICalculateBlockHash
    {
        bool IsValid();
        BlindedBlockRequest Request { get; }
        BlindedBlockResponse Response { get; }
        string CalculatedHash { get; }
    }

    public class CalculateBlockHash : ICalculateBlockHash
    {
        public BlindedBlockRequest Request { get; private set; }

        public BlindedBlockResponse Response { get; private set; }
        public string CalculatedHash { get; private set; }

        public CalculateBlockHash(BlindedBlockRequest request, BlindedBlockResponse response)
        {
            this.Request = request;
            this.Response = response;
            Build();
        }

        private void Build()
        {
            var transactions = Response.Data.transactions
                .Select(
                    t =>
                        Rlp.Decode<Transaction>(
                            Bytes.FromHexString(t),
                            RlpBehaviors.SkipTypedWrapping
                        )
                )
                .ToArray();

            IEnumerable<Withdrawal> items = Response?.Data?.withdrawals?.Select(
                x =>
                    new Withdrawal()
                    { 
                        Address = new Address(x.address),
                        AmountInGwei = ulong.Parse(x.amount),
                        Index = ulong.Parse(x.index),
                        ValidatorIndex = ulong.Parse(x.validator_index)
                    }
            );

            var parentHash = new Keccak(Response.Data.parent_hash);
            var feeRecipient = new Address(Response.Data.fee_recipient);
            var stateRoot = new Keccak(Response.Data.state_root);
            var receiptsRoot = new Keccak(Response.Data.receipts_root);
            var logsBloom = new Bloom(Bytes.FromHexString(Response.Data.logs_bloom));
            var totalDifficulty = new UInt256(0);
            var blockNumber = long.Parse(Response.Data.block_number);
            var gasLimit = long.Parse(Response.Data.gas_limit);
            var gasUsed = long.Parse(Response.Data.gas_used);
            var time = ulong.Parse(Response.Data.timestamp);
            var extraData = Bytes.FromHexString(Response.Data.extra_data);
            var prevRandao = new Keccak(Response.Data.prev_randao);
            var baseFeePerGas = UInt256.Parse(Response.Data.base_fee_per_gas);
            var txRoot = new TxTrie(transactions).RootHash;
            var withdrawalsRoot = Response.Data.withdrawals is null
                ? null
                : new WithdrawalTrie(items).RootHash;
            var blockHash = new Keccak(Response.Data.BlockHash);

            var header = new BlockHeader(
                parentHash,
                Keccak.OfAnEmptySequenceRlp,
                feeRecipient,
                UInt256.Zero,
                blockNumber,
                gasLimit,
                time,
                extraData
            )
            {
                Hash = blockHash,
                ReceiptsRoot = receiptsRoot,
                StateRoot = stateRoot,
                Bloom = logsBloom,
                GasUsed = gasUsed,
                BaseFeePerGas = baseFeePerGas,
                Nonce = 0,
                MixHash = prevRandao,
                Author = feeRecipient,
                IsPostMerge = true,
                TotalDifficulty = null,
                TxRoot = txRoot,
                WithdrawalsRoot = withdrawalsRoot,
            };

            Block block = new(header, transactions, Array.Empty<BlockHeader>(), items);
            CalculatedHash = block.Hash.ToString();
        }

        public bool IsValid()
        {
            var isSuccess = Request.Message.Body.ExecutionPayloadHeader.BlockHash == CalculatedHash;
            return isSuccess;
        }
    }
}
