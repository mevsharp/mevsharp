using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MEVSharp.Features.Http.Clients.Resources.Zapier
{

    public interface IZapierResource : INotificationResource
    {
        Task<(HttpResponseMessage HttpResponse, string TextResponse, ZapierResponse Data)> CatchRawHook(string payload);
    }
    public class ZapierResource : ResourceBase, IZapierResource
    {
        private readonly HttpClient client;
        private readonly string id;
        private readonly string secret;

        public ZapierResource(HttpClient client, ILogger logger, string id, string secret) : base(client, logger)
        {
            this.client = client;
            this.id = id;
            this.secret = secret;
        }

        public async Task<(HttpResponseMessage HttpResponse, string TextResponse, ZapierResponse Data)> CatchRawHook(string v)
        {

            var payload = base.BuildStringContent(v);
            var response = await client.PostAsync($"/hooks/catch/{id}/{secret}/", payload);
            var responsePayload = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)            {
                
                var dto = JsonSerializer.Deserialize<ZapierResponse>(responsePayload);
                return (response, responsePayload, dto);
            }
            return (response, responsePayload ,null);
        }

        public async Task<HttpResponseMessage> Notify(string message)
        {
            return (await this.CatchRawHook(message)).HttpResponse;
        }
    }

    public class ZapierResponse
    {
        [JsonPropertyName("attempt")]
        public string Attempt { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("request_id")]
        public string RequestId { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
    }


}
