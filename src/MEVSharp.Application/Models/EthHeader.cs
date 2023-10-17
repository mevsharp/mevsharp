using MEVSharp.Application.Configurations;
using MEVSharp.Application.Exceptions;
using MEVSharp.Features.Http.Clients.Dtos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Numerics;

namespace MEVSharp.Application.Models
{
    public class EthHeader : EthBase
    {
        public decimal Value { get; private set; }
        public string RelayPubKey { get; }
        public string ParentHash { get; }
        public string BlockHash => DTO.Data.Message.header.block_hash;

        private readonly ILogger logger;
        public readonly GetHeaderResponseDTO DTO;
        private readonly AppSettings appSettings;

        public EthHeader(
            ILogger logger,
            string relayPubKey,
            string parentHash,
            GetHeaderResponseDTO dto,
            AppSettings appSettings
        )
        {
            this.logger = logger;
            this.RelayPubKey = relayPubKey;
            this.ParentHash = parentHash;
            this.DTO = dto;
            this.appSettings = appSettings;
            Build();
        }

        public bool IsValidVersion()
        {
            return DTO.Version.ToLower() == "capella";
        }

        private void ThrowValidationException(string message)
        {
            logger.LogError(message);
            throw new EthHeaderBuildValidationException(this, message);
        }

        private void Build()
        {
            this.Value = Utils.ConvertWeiToEth(BigInteger.Parse(DTO.Data.Message.value));

            logger.LogInformation(
                $"getHeader response received from relay with pubkey: {RelayPubKey}. blockHash: {DTO.Data.Message.header.block_hash}, blockNumber: {DTO.Data.Message.header.block_number}, txRoot: {DTO.Data.Message.header.transactions_root}, value: {Value}"
            );
            logger.LogDebug($"getHeader response payload: {JsonConvert.SerializeObject(DTO.Data)}");
            if (DTO.Data.Message.header == null)
            {
                ThrowValidationException("Header is null");
            }

            if (!isValid())
            {
                ThrowValidationException("Invalid header");
            }

            if (DTO.Data.Message.pubkey == null || DTO.Data.Message.header.parent_hash == null)
            {
                ThrowValidationException("Pubkey or Parent Hash is null");
            }
            if (DTO.Data.Message.pubkey.Length != 98)
            {
                ThrowValidationException(
                    $"Invalid pubkey length: {DTO.Data.Message.pubkey.Length}"
                );
            }
            if (DTO.Data.Message.header.parent_hash.Length != 66)
            {
                ThrowValidationException(
                    $"Invalid parent_hash length: {DTO.Data.Message.header.parent_hash.Length}"
                );
            }
            if (
                Value <= 0
                || DTO.Data.Message.header.transactions_root
                    == "0x7ffe241ea60187fdb0187bfa22de35d1f9bed7ab061d9401fd47e34a54fbede1"
            )
            {
                ThrowValidationException("Invalid bid with 0 or lower value or no transactions");
            }
            var pointAtInfinityPubkey = new byte[48];
            if (
                HexMate.Convert.FromHexString(Utils.Remove0x(DTO.Data.Message.pubkey))
                == pointAtInfinityPubkey
            )
            {
                ThrowValidationException("Relay pubkey is point-at-infinity");
            }

            if (DTO.Data.Message.pubkey != RelayPubKey)
            {
                ThrowValidationException($"Pubkey in response does not match Relay pubkey: {DTO.Data.Message.pubkey} != {RelayPubKey}");
            }

            logger.LogDebug("Bid Received. Value: " + Value + " ETH");

            if (Value < appSettings.MinBid)
            {
                ThrowValidationException(
                    $"Bid below minimum value, ignoring bid. Relay bid: {Value} ETH, Minimum bid: {appSettings.MinBid} ETH"
                );
            }

            if (DTO.Data.Message.header.parent_hash != ParentHash)
            {
                ThrowValidationException($"Parent hash mismatch between proposer and relay header. {ParentHash} != {DTO.Data.Message.header.parent_hash}");
            }
        }

        private bool isValid()
        {
            if (
                DTO.Data.Message != null
                || DTO.Data.Message.header != null
                || DTO.Data.Message.header.block_hash == BitConverter.ToString(new byte[32])
            )
            {
                return true;
            }
            logger.LogError("Invalid header");
            return false;
        }
    }
}
