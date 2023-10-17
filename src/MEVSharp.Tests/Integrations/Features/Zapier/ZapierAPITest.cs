using MEVSharp.Features.Http.Clients.Resources.Telegram;
using MEVSharp.Features.Http.Clients.Resources.Zapier;
using MEVSharp.Features.Http.Clients.Services;
using MEVSharp.Tests.Integrations.Features.Shared;
using Microsoft.Extensions.Logging;

namespace MEVSharp.Tests.Integrations.Features.BuilderAPITests
{
    public class ZapierAPITest : IClassFixture<IntegrationMockHttpContextTestBase>
    {
        private readonly INotificationService service;

        public ZapierAPITest(IntegrationMockHttpContextTestBase fixture)
        {
            var telegramLogger = fixture.GetService<ILogger<TelegramResource>>();
            var zapierLogger = fixture.GetService<ILogger<ZapierResource>>();


            var zapierClient = new HttpClient() { BaseAddress = new Uri("https://hooks.zapier.com") };
            IZapierResource zapierResource =  new ZapierResource(zapierClient, zapierLogger, "", "");

            var telegramClient = new HttpClient() { BaseAddress = new Uri("https://api.telegram.org") };
            ITelegramResource telegramResource = new TelegramResource(telegramClient, telegramLogger, "", "");

            service = new NotificationService()
                .Add(zapierResource)
                .Add(telegramResource);
        }

        [Fact]
        public async Task Test01()
        {
            var message = Guid.NewGuid().ToString();
            await service.Notify(message);
        }
    }
}
