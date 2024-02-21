using MEVSharp.Application.Configurations;
using MEVSharp.Application.Providers;
using MEVSharp.Features.Http.Clients.Resources.Telegram;
using MEVSharp.Features.Http.Clients.Resources.Zapier;
using MEVSharp.Features.Http.Clients.Services;
using MEVSharp.UI.API.Formatter;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Data;
using System.Reflection;
using System.Text;

namespace MEVSharp.UI.API
{
    public class Program
    {
        public static CommandLineOptions CLISettings;
        public static AppSettings AppSettings;
        public static IHost host;
        public static INotificationService service;

        private static async Task<Command> BuildCommands(string version, string[] args)
        {
            var rootCommand = new RootCommand();

            var relayOpt = new Option<IEnumerable<string>>("--relay", () => AppSettings.RelayUrls, @"a single relay, can be specified multiple times (--relay scheme://pubkey@host1 --relay scheme://pubkey@host2 --relay scheme://pubkey@host3)");
            var relaysOpt = new Option<string>("--relays", () => "");
            var listenOpt = new Option<IEnumerable<string>>("--listen", () => AppSettings.HostPort, "");
            var networkOpt = new Option<string>("--network", () => AppSettings.Network);
            var genesisForkOpt = new Option<string>(
                "--genesis-fork-version",
                """use a custom genesis fork version (default: "")""");
            var minBidOpt = new Option<decimal>("--min-bid", () => AppSettings.MinBid);
            var relayCheckOpt = new Option<bool>("--relay-check", () => AppSettings.RelayCheck);
            var notificationTestOpt = new Option<bool>("--notification-test", () => AppSettings.NotificationTest);
            var requestGetHeaderOpt = new Option<int>(
                "--request-timeout-getheader",
                () => AppSettings.RequestTimeoutGetheader
            );
            var requestGetPayloadOpt = new Option<int>(
                "--request-timeout-getpayload",
                () => AppSettings.RequestTimeoutGetpayload
            );
            var requestRegValOpt = new Option<int>(
                "--request-timeout-regval",
                () => AppSettings.RequestTimeoutRegval
            );
            var genesisTimestampOpt = new Option<long>("--genesis-time", () => -1);
            var logNoVersionOpt = new Option<bool>("--log-no-version", () => false);
            new Option<string>("--log-type", () => "simple");
            var logServiceOpt = new Option<string>("--log-service", () => "");
            var logLevelOpt = new Option<string>("--loglevel",  ( () => "debug"), 
                $"{LogLevel.Trace}, " +
                $"{LogLevel.Debug}, " +
                $"{LogLevel.Information}, " +
                $"{LogLevel.Warning}," +
                $"{LogLevel.Error}," +
                $"{LogLevel.Critical}," +
                $"{LogLevel.None}");

            var zapierIDOpt = new Option<string>("--zapierID", () => AppSettings.ZapierID);
            var zapierSecretOpt = new Option<string>("--zapierSecret", () => AppSettings.ZapierSecret);
            var telegramAPIOpt = new Option<string>("--telegramAPI", () => AppSettings.TelegramAPI);
            var telegramChatIDOpt = new Option<string>("--telegramChatID", () => AppSettings.TelegramChatID);

            rootCommand.AddOption(relayOpt);
            rootCommand.AddOption(relaysOpt);
            rootCommand.AddOption(listenOpt);
            rootCommand.AddOption(networkOpt);
            rootCommand.AddOption(genesisForkOpt);
            rootCommand.AddOption(zapierIDOpt);
            rootCommand.AddOption(zapierSecretOpt);
            rootCommand.AddOption(telegramAPIOpt);
            rootCommand.AddOption(telegramChatIDOpt);
            rootCommand.AddOption(minBidOpt);
            rootCommand.AddOption(relayCheckOpt);
            rootCommand.AddOption(notificationTestOpt);
            rootCommand.AddOption(requestGetHeaderOpt);
            rootCommand.AddOption(requestGetPayloadOpt);
            rootCommand.AddOption(requestRegValOpt);
            rootCommand.AddOption(logNoVersionOpt);
            rootCommand.AddOption(logLevelOpt);
            rootCommand.AddOption(logServiceOpt);
            rootCommand.AddOption(genesisTimestampOpt);
            var binder = new CommandLineOptionsBinder(
                relayOpt,
                relaysOpt,
                listenOpt,
                networkOpt,
                genesisForkOpt,
                zapierIDOpt,
                zapierSecretOpt,
                telegramAPIOpt,
                telegramChatIDOpt,
                minBidOpt,
                relayCheckOpt,
                notificationTestOpt,
                requestGetHeaderOpt,
                requestGetPayloadOpt,
                requestRegValOpt,
                logNoVersionOpt,
                 logLevelOpt,
                 logServiceOpt,
                 genesisTimestampOpt
            );
            rootCommand.SetHandler(
                (options) =>
                {
                    var _options = (CommandLineOptions)options;

                   
                    if (_options.Help)
                    {
                        Environment.Exit(1);
                    }
                 
                  
                    if (!string.IsNullOrEmpty(_options.Relays))
                    {
                        List<string> items = _options.Relay.ToList();
                        items.AddRange(_options.Relays.Split(","));
                        options.Relay = items;
                    }

                    if (!Enum.TryParse<LogLevel>(options.LogLevel, true, out LogLevel _loglevel))
                    {
                        Log($"Invalid log level: {options.LogLevel} ({LogLevel.Trace}, {LogLevel.Debug}, {LogLevel.Information}, {LogLevel.Warning}, {LogLevel.Error}, {LogLevel.Critical}, {LogLevel.None})");
                        Environment.Exit(1);
                    }

            CLISettings = options;
                },
                binder
            );

            rootCommand.Description = "MevSharp Command Line Interface";
            var result = await rootCommand.InvokeAsync(args);
            return rootCommand;
        }
        private static void Log(string message)
        {
            var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            Console.WriteLine($"{time} {message}");
        }
        private static async Task ValidateConfiguration(string version, ILogger logger, IHost app)
        {

            #region VALIDATE CONFIGURATION

            Log($"MEVSharp {version}");
            if (AppSettings.RelayUrls.Count == 0)
            {
                Log("No relay URLs configured. Use --relay --relays or set environment variable RELAYS. Exiting...");
                Environment.Exit(1);
            }
            var uniqueRelayUrls = AppSettings.RelayUrls.Distinct().ToList();
            if (uniqueRelayUrls.Count != AppSettings.RelayUrls.Count)
            {
                AppSettings.RelayUrls = uniqueRelayUrls;
            }
            if (AppSettings.MinBid < 0)
            {
                Log("Minimum bid cannot be a negative number");
                Environment.Exit(1);
            }
            
            

            foreach (var relayUrl in AppSettings.RelayUrls)
            {
                if (!Uri.IsWellFormedUriString(relayUrl, UriKind.Absolute))
                {
                    Log($"Relay URL {relayUrl} is not a valid URL");
                    Environment.Exit(1);
                }
                if (new Uri(relayUrl).UserInfo == "")
                {
                    Log(
                        $"Relay URL {relayUrl} does not contain public key. Exiting..."
                    );
                    Environment.Exit(1);
                }
                var pointAtInfinityPubkey = new byte[48];
                var userInfoBytes = Encoding.UTF8.GetBytes(new Uri(relayUrl).UserInfo);
                if (userInfoBytes.SequenceEqual(pointAtInfinityPubkey))
                {
                    Log(
                        $"Relay URL {relayUrl} contains point at infinity public key. Exiting..."
                    );
                    Environment.Exit(1);
                }
            }
            if (AppSettings.MinBid > 100000)
            {
                Log("Minimum bid is too large. It is denominated in Ether");
                Environment.Exit(1);
            }
            foreach (var addressListening in AppSettings.HostPort)
            {
                try
                {
                    var uri = new UriBuilder(addressListening);
                }
                catch (UriFormatException e)
                {
                    Log(e.Message);
                    Environment.Exit(1);
                }
                catch (Exception e)
                {
                    Log(e.Message);
                    Environment.Exit(1);
                }
            }
            if (AppSettings.RelayCheck)
            {
                Log("Status check on all relays: ");
                var x = app.Services.GetService<IServiceProvider>();

                try
                {
                    using (var scope = x.CreateScope())
                    {
                        var relayProvider = scope.ServiceProvider.GetService<IRelayProvider>();
                        var responses = await relayProvider.GetStatuses();
                        if (responses.Any(x => x.Result != System.Net.HttpStatusCode.OK)) throw new Exception("Status check failed on a relay. Exiting...");
                    }
                }
                catch (Exception ex)
                {
                    Log(ex.Message);
                    Environment.Exit(1);
                }
            }
            if (!string.IsNullOrEmpty(AppSettings.GenesisForkVersion))
            {
                Log(
                    $"Custom Genesis Fork Version: {AppSettings.GenesisForkVersion}"
                );
            }
            else
            {
                Log($"Network: {AppSettings.Network}");
            }

            if ((!string.IsNullOrEmpty(AppSettings.ZapierID) && string.IsNullOrEmpty(AppSettings.ZapierSecret)) ||
                (string.IsNullOrEmpty(AppSettings.ZapierID) && !string.IsNullOrEmpty(AppSettings.ZapierSecret)))
            {
                Log("ZapierID and ZapierSecret must both be set or neither should be set");
                Environment.Exit(1);
            }
            if ((!string.IsNullOrEmpty(AppSettings.TelegramAPI) && string.IsNullOrEmpty(AppSettings.TelegramChatID)) ||
                (string.IsNullOrEmpty(AppSettings.TelegramAPI) && !string.IsNullOrEmpty(AppSettings.TelegramChatID)))
            {
                Log("TelegramAPI and TelegramChatID must both be set or neither should be set");
                Environment.Exit(1);
            }

            if (!string.IsNullOrEmpty(AppSettings.ZapierID))
            {
                var zapierClient = new HttpClient() { BaseAddress = new Uri("https://hooks.zapier.com") };
                IZapierResource zapierResource =  new ZapierResource(zapierClient, logger, AppSettings.ZapierID, AppSettings.ZapierSecret);
                if(AppSettings.NotificationTest)
                {
                    var message = "Zapier notification test successful";
                    var response = await zapierResource.Notify(message);
                    Log("Zapier: Enabled. Verify you received the notification saying \"Zapier notification test successful\"");
                }
                else
                {
                    Log("Zapier: Enabled.");
                }
            }

            if (!string.IsNullOrEmpty(AppSettings.TelegramAPI) && !string.IsNullOrEmpty(AppSettings.TelegramChatID))
            {
                var telegramClient = new HttpClient() { BaseAddress = new Uri("https://api.telegram.org") };
                ITelegramResource telegramResource = new TelegramResource(telegramClient, logger, AppSettings.TelegramAPI, AppSettings.TelegramChatID);
                if(AppSettings.NotificationTest)
                {
                    var message = "Telegram notification test successful";
                    var response = await telegramResource.Notify(message);
                    if(response.IsSuccessStatusCode)
                    {
                        Log("Telegram: Enabled. Received OK from Telegram, verify you received the notification saying \"Telegram notification test successful\"");
                    }
                    else
                    {
                        Log("Telegram notification failed to run successfully, please verify API and ChatID.");
                        Environment.Exit(1);
                    }
                }
            }

            Log($"Relays loaded: {AppSettings.RelayUrls.Count}");
            Log($"RequestTimeoutGetheader: {AppSettings.RequestTimeoutGetheader}");
            Log($"RequestTimeoutRegval: {AppSettings.RequestTimeoutRegval}");
            Log($"RequestTimeoutGetpayload: {AppSettings.RequestTimeoutGetpayload}");
            int counter = 0;
            foreach (var relayUrl in AppSettings.RelayUrls)
            {
                counter++;
                Log($"Relay{counter}: {relayUrl}");
            }
            
            Log($"MinBid: {AppSettings.MinBid} ETH");
            Log($"Set loglevel: {AppSettings.LogLevel}");

            #endregion
        }

        public static async Task Main(string[] args)
        {
            string version = typeof(Program).Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion;

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            AppSettings = configuration.GetSection("Settings").Get<AppSettings>();
            await BuildCommands(version, args);


            var environmentListenAddr = Environment.GetEnvironmentVariable("BOOST_LISTEN_ADDR");
          

            if (CLISettings is null && environmentListenAddr is null)
            {
                Log("Could not execute because the specified command or file was not found");
                Environment.Exit(1);
            }

            IHostBuilder builder;
            if (environmentListenAddr is not null)
            {
                var listenAddress = environmentListenAddr.Split(",");
                builder = CreateHostBuilder(args, listenAddress);
                
            }
            else
            {
                var listenAddress = (CLISettings.Listen.Any()
                      ? CLISettings.Listen.ToArray()
                      : CLISettings.Listen.ToArray()
              );
                builder = CreateHostBuilder(args, listenAddress);
            }

            builder.ConfigureLogging( builder =>
            {
                var loglevel = CLISettings.GetLogLevel();
                builder.ClearProviders();
                builder.AddCustomFormatter( x => x.CustomPrefix = CLISettings.LogService);
                builder.SetMinimumLevel(loglevel);
            });
            var app = builder.Build();
            var logger = app.Services.GetService<ILogger<Program>>();
            AppSettings = app.Services.GetService<AppSettings>();

            await ValidateConfiguration(version, logger, app);           
            host = app;
            foreach (var hostPort in AppSettings.HostPort)
            {
                Log($"Listening on: {hostPort}");
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args, params string[] listenUrl) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    if (listenUrl != null)
                        webBuilder.UseUrls(listenUrl);
                });
    }
}
