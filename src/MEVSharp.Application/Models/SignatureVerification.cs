using MEVSharp.Application.Configurations;
using MEVSharp.Features.Http.Clients.Dtos;
using MEVSharp.Nethermind.Crypto;
using Nethermind.Core2.Crypto;
using Nethermind.Core2.Types;
using SszSharp;
using System.Numerics;
using Root = Nethermind.Core2.Crypto.Root;
namespace MEVSharp.Application.Models
{
    public interface ISignatureVerification
    {
        bool Verify(GetHeaderResponseDTO v);
    }

    public class SignatureVerification : ISignatureVerification
    {
        private ExecutionPayloadHeader header;
        private BuilderBid builderBid;
        private readonly AppSettings appSettings;
        private byte[] DOMAIN_TYPE = new byte[] { 0x00, 0x00, 0x00, 0x01 };
        private Root GENESIS_VALIDATORS_ROOT =
            new Root(new byte[32]);
        private Domain domain;
        private LightClientUtility utility;

        public SignatureVerification(AppSettings appSettings)
        {
            utility = new LightClientUtility();
            this.appSettings = appSettings;
        }

        public bool Verify(GetHeaderResponseDTO v)
        {
            Build(v);
            var _headerRoot = headerRoot();
            var _signingRoot = SigningRoot(_headerRoot);
            var _success = VerifySignature(v, _signingRoot);
            return _success;
        }

        #region Privates
        private void Build(GetHeaderResponseDTO dto)
        {
            header = new ExecutionPayloadHeader
            {
                Root = HexMate.Convert.FromHexString(
                    Utils.Remove0x(dto.Data.Message.header.parent_hash)
                ),
                FeeRecipient = HexMate.Convert.FromHexString(
                    Utils.Remove0x(dto.Data.Message.header.fee_recipient)
                ),
                StateRoot = HexMate.Convert.FromHexString(
                    Utils.Remove0x(dto.Data.Message.header.state_root)
                ),
                ReceiptsRoot = HexMate.Convert.FromHexString(
                    Utils.Remove0x(dto.Data.Message.header.receipts_root)
                ),
                LogsBloom = HexMate.Convert.FromHexString(
                    Utils.Remove0x(dto.Data.Message.header.logs_bloom)
                ),
                PrevRandao = HexMate.Convert.FromHexString(
                    Utils.Remove0x(dto.Data.Message.header.prev_randao)
                ),
                BlockNumber = ulong.Parse(dto.Data.Message.header.block_number),
                GasLimit = ulong.Parse(dto.Data.Message.header.gas_limit),
                GasUsed = ulong.Parse(dto.Data.Message.header.gas_used),
                Timestamp = ulong.Parse(dto.Data.Message.header.timestamp),
                ExtraData = HexMate.Convert.FromHexString(
                    Utils.Remove0x(dto.Data.Message.header.extra_data)
                ),
                BaseFeePerGas = BigInteger.Parse(dto.Data.Message.header.base_fee_per_gas),
                BlockHash = HexMate.Convert.FromHexString(
                    Utils.Remove0x(dto.Data.Message.header.block_hash)
                ),
                TransactionsRoot = HexMate.Convert.FromHexString(
                    Utils.Remove0x(dto.Data.Message.header.transactions_root)
                ),
                WithdrawalsRoot = HexMate.Convert.FromHexString(
                    Utils.Remove0x(dto.Data.Message.header.withdrawals_root)
                )
            };
            builderBid = new BuilderBid
            {
                Header = header,
                Value = ulong.Parse(dto.Data.Message.value),
                Pubkey = HexMate.Convert.FromHexString(Utils.Remove0x(dto.Data.Message.pubkey))
            };

            byte[] genesisForkVersion = new byte[] { 0x00, 0x00, 0x00, 0x00 };
            if (appSettings.Network == "mainnet")
            {
                genesisForkVersion = HexMate.Convert.FromHexString(Utils.Remove0x("0x00000000"));
            }
            else if (appSettings.Network == "sepolia")
            {
                genesisForkVersion = HexMate.Convert.FromHexString(Utils.Remove0x("0x90000069"));
            }
            else if (appSettings.Network == "goerli")
            {
                genesisForkVersion = HexMate.Convert.FromHexString(Utils.Remove0x("0x00001020"));
            }
            ForkVersion GENESIS_FORK_VERSION = new ForkVersion(genesisForkVersion);

            domain = utility.ComputeDomain(
                new DomainType(DOMAIN_TYPE),
                GENESIS_FORK_VERSION,
                GENESIS_VALIDATORS_ROOT
            );
        }

        private byte[] headerRoot()
        {
            var containerType = SszSharp.SszContainer.GetContainer<BuilderBid>(
                SizePreset.MainnetPreset
            );
            var headerRoot = containerType.HashTreeRoot(builderBid);
            return headerRoot;
        }

        private Root SigningRoot(byte[] headerRoot)
        {
            var signRoot = new Root(headerRoot);
            var signingRoot = utility.ComputeSigningRoot(signRoot, domain);
            return signingRoot;
        }

        private bool VerifySignature(
            GetHeaderResponseDTO dto,
            Root signingRoot
        )
        {
            BlsPublicKey pubKey = new BlsPublicKey(dto.Data.Message.pubkey);
            var signature = new BlsSignature(
                HexMate.Convert.FromHexString(Utils.Remove0x(dto.Data.Signature))
            );
            var success = utility.crypto.BlsVerify(pubKey, signingRoot, signature);
            return success;
        }
        #endregion
    }
}
