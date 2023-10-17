using MEVSharp.Application.Configurations;
using MEVSharp.Application.Exceptions;
using MEVSharp.Application.Models;
using MEVSharp.Application.Providers;
using MEVSharp.Features.Http.Clients.Dtos;
using MEVSharp.Features.Http.Clients.Dtos.BlindedBlocks;
using MEVSharp.Features.Http.Clients.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace MEVSharp.UI.API.Controllers
{
    [Route("eth/v1/builder/")]
    [ApiController]
    public class BuilderController : ControllerBase
    {
        private readonly INotificationService service;
        private readonly ISignatureVerification signatureVerification;
        private readonly IRelayProvider relayProvider;
        private readonly ILogger<EthSubmitBlindedBlock> ethSubmitLogger;
        private readonly ILogger<BuilderController> controllerLogger;
        private readonly AppSettings appSettings;
        long genesisTime = 0;
        long genesisTimeMainnet = 1606824023;
        long genesisTimeSepolia = 1655733600;
        long genesisTimeGoerli = 1614588812;
        long genesisTimeHolesky = 1694786100;
        long slotTime = 12;

        public BuilderController(
            INotificationService service,
            ISignatureVerification signatureVerification,
            IRelayProvider relayProvider,
            ILogger<EthSubmitBlindedBlock> ethSubmitLogger,
            ILogger<BuilderController> controllerLogger,
            AppSettings appSettings
        )
        {
            this.service = service;
            this.signatureVerification = signatureVerification;
            this.relayProvider = relayProvider;
            this.ethSubmitLogger = ethSubmitLogger;
            this.controllerLogger = controllerLogger;
            this.appSettings = appSettings;

            if (appSettings.Network == "mainnet")
            {
                genesisTime = genesisTimeMainnet;
            }
            else if (appSettings.Network == "sepolia")
            {
                genesisTime = genesisTimeSepolia;
            }
            else if (appSettings.Network == "goerli")
            {
                genesisTime = genesisTimeGoerli;
            }
            else if (appSettings.Network == "holesky")
            {
                genesisTime = genesisTimeHolesky;
            }
        }

        [HttpGet]
        [Route("status")]
        public async Task<IActionResult> Status()
        {
            var responses = await relayProvider.GetStatuses();
            controllerLogger.LogDebug("Checking status of relays");

            foreach (var item in responses)
            {
                controllerLogger.LogDebug(
                    $"Status Check: {item.Client.Resource.Client.BaseAddress} {item.Result}"
                );
            }

            if (responses.Any(x => x.Result == HttpStatusCode.OK))
            {
                return StatusCode(StatusCodes.Status200OK, 200);
            }
            else
            {
                return StatusCode(
                    StatusCodes.Status503ServiceUnavailable,
                    new { message = "No relays available" }
                );
            }
        }

        [HttpPost]
        [Route("validators")]
        public async Task<IActionResult> Register(
            [FromBody] List<RegisterValidatorRequestDTO> request
        )
        {
            try
            {
                controllerLogger.LogDebug("Registering validators on relays");
                var responses = await relayProvider.RegisterValidator(request);
                if (responses.Any(x => x.Result.IsSuccessStatusCode))
                    return Ok();
                else
                {
                    controllerLogger.LogDebug("Failed to register validators on all relays");
                    return StatusCode((int)HttpStatusCode.BadGateway);
                }

            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("header/{slot}/{parent_hash}/{pubkey}")]
        public async Task<IActionResult> GetHeader(
            [FromRoute] string slot,
            [FromRoute] string parent_hash,
            [FromRoute] string pubkey
        )
        {
            var _slot = long.Parse(slot);
            string message;
            long slotStartTimestamp = genesisTime + (_slot * slotTime);
            long msIntoSlot =
                DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - (slotStartTimestamp * 1000);
            controllerLogger.LogInformation(
                $"getHeader request started {msIntoSlot} milliseconds into slot {_slot} with parent_hash: {parent_hash} and pubkey: {pubkey}"
            );
            service.Notify($"Validator requesting new block header! 🎉 https://beaconcha.in/slot/{_slot}");

            try
            {
                var result = await relayProvider.GetHeaders(slot, parent_hash, pubkey);
                foreach (var item in result.OrderByDescending(x => x.Result.Value))
                {
                    bool success;
                    controllerLogger.LogInformation(
                        $"getHeader payload received from {item.Client.Resource.Client.BaseAddress}, forwarding to validator"
                    );
                    controllerLogger.LogDebug(
                        $"Payload: {JsonConvert.SerializeObject(item.Result.DTO)}"
                    );
                    if (appSettings.VerifySignature)
                    {
                        controllerLogger.LogDebug(
                            $"Verifying signature for {item.Result.BlockHash}"
                        );
                        success = signatureVerification.Verify(item.Result.DTO);
                    }
                    else
                    {
                        controllerLogger.LogDebug(
                            $"Skipping signature verification for {item.Result.BlockHash}"
                        );
                        success = true;
                    }

                    if (success)
                    {
                        message = $"Found and verified winning bid for {item.Result.BlockHash} worth {item.Result.Value} ETH on relay {item.Client.Resource.Client.BaseAddress}";
                        controllerLogger.LogInformation(message);
                        service.Notify($"Found valid bid! 🎉 {item.Result.Value} ETH on relay {item.Client.Resource.Client.BaseAddress}");
                        return Ok(item.Result.DTO);
                    }
                }
                message = $"Failed to retrieve a verifiable header for all relays with parent_hash: {parent_hash} and pubkey: {pubkey}";
                controllerLogger.LogError(message);
                service.Notify("No valid bid found, see logs for more details. 😢");
                return StatusCode(StatusCodes.Status502BadGateway);
            }
            catch (HttpRequestException e)
            {
                controllerLogger.LogError($"HttpRequestException: {e}");
                return StatusCode((int)e.StatusCode, e.Message);
            }
            catch (ParentHashMismatchException e)
            {
                message = $"ParentHashMismatchException: {e.Message}";
                controllerLogger.LogError(message);
                return StatusCode((int)HttpStatusCode.NotAcceptable, e.Message);
            }
            catch (EthHeaderBuildValidationException e)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("blinded_blocks")]
        public async Task<IActionResult> GetPayload([FromBody] BlindedBlockRequest request)
        {
            long slot = long.Parse(request.Message.slot);
            string blockHash = request.Message.Body.Eth1Data.BlockHash;
            string parentHash = request.Message.Body.ExecutionPayloadHeader.parent_hash;
            string message;
            long slotStartTimestamp = genesisTime + (slot * slotTime);
            long msIntoSlot =
                DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - (slotStartTimestamp * 1000);
            message =  $"submitBlindedBlocks started {msIntoSlot} milliseconds into slot {slot} with blockHash: {blockHash} and parentHash: {parentHash}";
            controllerLogger.LogInformation(message);
            controllerLogger.LogDebug(
                $"GetPayload request: " + Newtonsoft.Json.JsonConvert.SerializeObject(request)
            );
            try
            {
                var responses = await relayProvider.SubmitBlindedBlocks(request);
                foreach (var response in responses)
                {
                    if (response.Result == null)
                    {
                        controllerLogger.LogDebug(
                            $"submitBlindedBlocks payload received from {response.Client.Resource.Client.BaseAddress} was null"
                        );
                        continue;
                    }

                    controllerLogger.LogInformation(
                        $"submitBlindedBlocks response received from {response.Client.Resource.Client.BaseAddress}"
                    );
                    controllerLogger.LogDebug(
                        $"Response: {JsonConvert.SerializeObject(response.Result)}"
                    );

                    ICalculateBlockHash calculate = new CalculateBlockHash(
                        request,
                        response.Result
                    );
                    var isValid = calculate.IsValid();
                    if (isValid)
                    {
                        message = $"Block hash matched: {request.Message.Body.ExecutionPayloadHeader.BlockHash} == {calculate.CalculatedHash}, forwarding to validator";
                        controllerLogger.LogDebug(message);
                        return Ok(response.Result);
                    }
                    else
                    {
                        message = $"Block hash mismatch: {request.Message.Body.ExecutionPayloadHeader.BlockHash} != {calculate.CalculatedHash}, will not forward to validator";
                        controllerLogger.LogError(message);
                    }
                }
                controllerLogger.LogError("No valid submitBlindedBlocks response from any relay");
                return StatusCode(StatusCodes.Status502BadGateway);
            }
            catch (HttpRequestException e)
            {
                message = $"HttpRequestException: {e.Message}";
                controllerLogger.LogError(message);
                return StatusCode((int)e.StatusCode, e.Message);
            }
            catch (Exception e)
            {
                message = "Exception: {e.Message}";
                controllerLogger.LogError(message);
                return StatusCode((int)HttpStatusCode.NotAcceptable, e.Message);
            }
        }
    }
}
