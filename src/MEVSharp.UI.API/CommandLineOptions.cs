using System.CommandLine;
using System.CommandLine.Binding;

namespace MEVSharp.UI.API
{
    public class CommandLineOptions
    {
        public int RequestTimeoutGetHeader { get; set; }
        public int RequestTimeoutGetPayload { get; set; }
        public int RequestTimeoutRegVal { get; set; }
        public IEnumerable<string> Relay { get; set; }
        public string Relays { get; set; }
        public IEnumerable<string> Listen { get; set; }
        public string Network { get; set; }
        public string GenesisForkVersion { get; set; }
        public string ZapierID { get; set; }
        public string ZapierSecret { get; set; }
        public string TelegramAPI { get; set; }
        public string TelegramChatID { get; set; }
        public string LogType { get; set; }
        public string LogService { get; set; }
        public string LogLevel { get; set; }

        public LogLevel GetLogLevel()
        {
            if(!Enum.TryParse<LogLevel>(this.LogLevel, true, out LogLevel _loglevel))
            {
                throw new Exception($"{this.LogLevel} is not a valid log level.");
                
            }
            return _loglevel;
        }
        
        public bool Help { get; set; }
        public bool LogNoVersion { get; set; }
        public bool RelayCheck { get; set; }
        public bool NotificationTest { get; set; }
        public decimal MinBid { get; set; }
    }

    public class CommandLineOptionsBinder : BinderBase<CommandLineOptions>
    {
        private readonly Option<IEnumerable<string>> relay;
        private readonly Option<string> relays;
        private readonly Option<IEnumerable<string>> listen;
        private readonly Option<string> network;
        private readonly Option<string> genesisForkVersion;
        private readonly Option<string> zapierID;
        private readonly Option<string> zapierSecret;
        private readonly Option<string> telegramAPI;
        private readonly Option<string> telegramChatID;
        private readonly Option<decimal> minBid;
        private readonly Option<bool> relayCheck;
        private readonly Option<bool> notificationTest;
        private readonly Option<int> requestHeader;
        private readonly Option<int> requestPayload;
        private readonly Option<int> requestReg;
        private readonly Option<string> logLevel;
        private readonly Option<string> logService;
        private readonly Option<bool> logNoVersion;
        private readonly Option<bool> versionOpt;

        public CommandLineOptionsBinder(
            Option<IEnumerable<string>> relay,
            Option<string> relays,
            Option<IEnumerable<string>> listen,
            Option<string> network,
            Option<string> genesisForkVersion,
            Option<string> zapierID,
            Option<string> zapierSecret,
            Option<string> telegramAPI,
            Option<string> telegramChatID,
            Option<decimal> minBid,
            Option<bool> relayCheck,
            Option<bool> notificationTest,
            Option<int> requestHeader,
            Option<int> requestPayload,
            Option<int> requestReg,
            Option<bool> logNoVersion,
            Option<string> logLevel
,
            Option<string> logService
        )
        {
            this.logLevel = logLevel;
            this.relay = relay;
            this.relays = relays;
            this.listen = listen;
            this.network = network;
            this.genesisForkVersion = genesisForkVersion;
            this.zapierID = zapierID;
            this.zapierSecret = zapierSecret;
            this.telegramAPI = telegramAPI;
            this.telegramChatID = telegramChatID;
            this.minBid = minBid;
            this.relayCheck = relayCheck;
            this.notificationTest = notificationTest;
            this.requestHeader = requestHeader;
            this.requestPayload = requestPayload;
            this.requestReg = requestReg;
            this.logNoVersion = logNoVersion;
            this.logService = logService;
        }

        protected override CommandLineOptions GetBoundValue(BindingContext bindingContext)
        {
            var model = new CommandLineOptions()
            {
                LogService = GetValue(bindingContext, logService),
                LogLevel = GetValue(bindingContext, logLevel),
                GenesisForkVersion = GetValue(bindingContext, genesisForkVersion),
                ZapierID = GetValue(bindingContext, zapierID),
                ZapierSecret = GetValue(bindingContext, zapierSecret),
                TelegramAPI = GetValue(bindingContext, telegramAPI),
                TelegramChatID = GetValue(bindingContext, telegramChatID),
                Relay = GetValue(bindingContext, relay),
                Relays = GetValue(bindingContext, relays),
                Listen = GetValue(bindingContext, listen),
                Network = GetValue(bindingContext, network),
                MinBid = GetValue(bindingContext, minBid),
                RelayCheck = GetValue(bindingContext, relayCheck),
                NotificationTest = GetValue(bindingContext, notificationTest),
                RequestTimeoutGetHeader = GetValue(bindingContext, requestHeader),
                RequestTimeoutGetPayload = GetValue(bindingContext, requestPayload),
                RequestTimeoutRegVal = GetValue(bindingContext, requestReg)
            };
            return model;
        }

        private T GetValue<T>(BindingContext context, Option<T> option)
        {
            return context.ParseResult.GetValueForOption(option);
        }
    }
}
