using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MEVSharp.Features.Http.Clients.Resources.Telegram
{
    public interface ITelegramResource : INotificationResource
    {
        Task<(HttpResponseMessage HttpResponse, string TextResponse, Rootobject Data)> SendMessage( string text);
    }
    public class TelegramResource : ResourceBase, ITelegramResource
    {
        private readonly HttpClient client;
        private readonly ILogger logger;
        private readonly string botApiKey;
        private readonly string chatId;

        public TelegramResource(HttpClient client, ILogger logger, string botApiKey, string chatId) : base(client, logger)
        {
            this.client = client;
            this.logger = logger;
            this.botApiKey = botApiKey;
            this.chatId = chatId;
        }

        public async Task<HttpResponseMessage> Notify(string message)
        {
           return (await this.SendMessage(message)).HttpResponse;
        }

        public async Task<(HttpResponseMessage HttpResponse, string TextResponse, Rootobject Data)> SendMessage(string text)
        {

            var url = $"/bot{botApiKey}/sendMessage?chat_id={chatId}&text={text}";

            
            var response = await client.GetAsync(url);
            var responsePayload = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var dto = JsonSerializer.Deserialize<Rootobject>(responsePayload);
                return (response, responsePayload, dto);
            }
            
            
            return (response, responsePayload, null);
        }
    }

    public class Rootobject
    {
        public bool ok { get; set; }
        public Result result { get; set; }
    }

    public class Result
    {
        public int message_id { get; set; }
        public From from { get; set; }
        public Chat chat { get; set; }
        public int date { get; set; }
        public string text { get; set; }
    }

    public class From
    {
        public long id { get; set; }
        public bool is_bot { get; set; }
        public string first_name { get; set; }
        public string username { get; set; }
    }

    public class Chat
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string username { get; set; }
        public string type { get; set; }
    }





}
