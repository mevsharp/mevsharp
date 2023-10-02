using MEVSharp.Application.Factories;
using MEVSharp.Application.Models;
using MEVSharp.Features.Http.Clients.Dtos;
using MEVSharp.Features.Http.Clients.Dtos.BlindedBlocks;
using Microsoft.Extensions.Logging;
using System.Net;

namespace MEVSharp.Application.Providers
{
    public class RelayProvider : IRelayProvider
    {
        private readonly ILogger logger;

        public IAPIBuilderFactory Factory { get; }

        public RelayProvider(IAPIBuilderFactory factory, ILogger<RelayProvider> logger)
        {
            this.Factory = factory;
            this.logger = logger;
        }

        public async Task<IEnumerable<IRelayResult<EthHeader>>> GetHeaders(
            string slot,
            string parentHash,
            string pubkey
        )
        {
            List<Task<IRelayResult<EthHeader>>> items = new();

            try
            {
                foreach (var item in Factory.Clients)
                {
                    items.Add(item.Value.GetHeader(slot, parentHash, pubkey));
                }
                await Task.WhenAll(items);
                if (items.Any(x => x.IsCompletedSuccessfully))
                    return items.Where(x => x.IsCompletedSuccessfully).Select(x => x.Result);
                else
                    return Enumerable.Empty<IRelayResult<EthHeader>>();
            }
            catch (Exception e)
            {
                if (items.Any(x => x.IsCompletedSuccessfully))
                    return items.Where(x => x.IsCompletedSuccessfully).Select(x => x.Result);
                else
                    return Enumerable.Empty<IRelayResult<EthHeader>>();
            }
        }

        public async Task<IEnumerable<IRelayResult<HttpStatusCode>>> GetStatuses()
        {
            List<Task<IRelayResult<HttpStatusCode>>> items = new();
            try
            {
                foreach (var item in Factory.Clients)
                {
                    items.Add(item.Value.GetStatus());
                }
                await Task.WhenAll(items);
                return items.Select(x => x.Result);
            }
            catch (Exception e)
            {
                return items.Select(x => x.Result);

            }
        }

        public async Task<IEnumerable<IRelayResult<HttpResponseMessage>>> RegisterValidator(
            List<RegisterValidatorRequestDTO> registerValidatorRequestInner,
            int operationIndex = 0
        )
        {
            List<Task<IRelayResult<HttpResponseMessage>>> items = new();

            try
            {
                foreach (var item in Factory.Clients)
                {
                    items.Add(
                        item.Value.RegisterValidator(registerValidatorRequestInner, operationIndex)
                    );
                }
                await Task.WhenAll(items);

                foreach (var item in items)
                {
                    var text = (
                        item.Result.Result == null
                            ? ((int)HttpStatusCode.BadGateway).ToString()
                            : ((int)item.Result.Result.StatusCode).ToString()
                    );
                    logger.LogDebug($"{item.Result.Client.Resource.Client.BaseAddress}: {text}");
                }

                //if (items.Any(x => x.Result. x.Result.Result != null))
                return items.Where(x => x.IsCompletedSuccessfully && x.Result is not null && x.Result.Result is not null).Select(x => x.Result);
                //else
                //return Enumerable.Empty<IRelayResult<HttpResponseMessage>>();
            }
            catch (TaskCanceledException e)
            {
                logger.LogDebug($"RegisterValidator TaskCanceledException {e.Message}");
                //if (items.Any(x => x.Result.Result != null))
                return items.Where(x => x.IsCompletedSuccessfully && x.Result is not null && x.Result.Result is not null).Select(x => x.Result);
                //else
                //    return Enumerable.Empty<IRelayResult<HttpResponseMessage>>();
            }
            catch (Exception e)
            {
                logger.LogDebug($"RegisterValidator Exception {e.Message}");
                //if (items.Any(x => x.IsCompletedSuccessfully))
                return items.Where(x => x.IsCompletedSuccessfully && x.Result is not null && x.Result.Result is not null).Select(x => x.Result);
                //else
                //    return Enumerable.Empty<IRelayResult<HttpResponseMessage>>();
            }
        }

        public async Task<
            IEnumerable<IRelayResult<Features.Http.Clients.Dtos.BlindedBlocks.BlindedBlockResponse>>
        > SubmitBlindedBlocks(BlindedBlockRequest request)
        {
            List<
                Task<IRelayResult<Features.Http.Clients.Dtos.BlindedBlocks.BlindedBlockResponse>>
            > items = new();

            try
            {
                foreach (var item in Factory.Clients)
                {
                    items.Add(item.Value.SubmitBlindedBlock(request));
                }
                await Task.WhenAll(items);
                if (items.Any(x => x.IsCompletedSuccessfully))
                    return items.Where(x => x.IsCompletedSuccessfully).Select(x => x.Result);
                else
                    return Enumerable.Empty<
                        IRelayResult<Features.Http.Clients.Dtos.BlindedBlocks.BlindedBlockResponse>
                    >();
            }
            catch (Exception e)
            {
                if (items.Any(x => x.IsCompletedSuccessfully))
                    return items.Where(x => x.IsCompletedSuccessfully).Select(x => x.Result);
                else
                    return Enumerable.Empty<
                        IRelayResult<Features.Http.Clients.Dtos.BlindedBlocks.BlindedBlockResponse>
                    >();
            }
        }
    }
}
