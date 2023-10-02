using MEVSharp.Features.Http.Clients.Resources.APIBuilder;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace MEVSharp.Features.Http.Clients.Resources
{
    public abstract class ResourceBase : IResourceBase
    {
        private readonly ILogger logger;

        public HttpClient Client { get; }

        public ResourceBase(HttpClient client, ILogger logger)
        {
            Client = client;
            this.logger = logger;
        }

        #region Privates
        protected async Task<(HttpResponseMessage Message, T Entity)> Invoke<T>(
            Task<HttpResponseMessage> response
        )
        {
            HttpResponseMessage result = null;
            try
            {
                result = await response;
                if (!result.IsSuccessStatusCode)
                {
                    var badResponse = await result.Content.ReadAsStringAsync();
                    logger.LogError(
                        $"Bad response from relay {result.RequestMessage.RequestUri.AbsoluteUri}: {badResponse}"
                    );
                    return (result, default(T));
                }
                var json = await result.Content.ReadAsStringAsync();

                var entity = await BuildResponse<T>(result);
                return (result, entity);
            }
            catch (TaskCanceledException e)
            {
                return (result, default(T));
            }
            catch (Exception e)
            {
                logger.LogError(e.ToString(), "Error while invoking resource");
                return (result, default(T));
            }
        }

        protected async Task<T> BuildResponse<T>(HttpResponseMessage response)
        {
            var responsePayLoad = await response.Content.ReadAsStringAsync();
            var entity = JsonConvert.DeserializeObject<T>(responsePayLoad);
            return entity;
        }

        protected (HttpContent Content, string Json) BuildRequestContent(object request)
        {
            var payload = JsonConvert.SerializeObject(request);
            StringContent content = new StringContent(payload, Encoding.UTF8, "application/json");
            return (content, payload);
        }
        #endregion
    }
}
