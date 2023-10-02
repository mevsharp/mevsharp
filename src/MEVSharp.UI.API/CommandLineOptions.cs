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
        
        //public bool Version { get; set; }
        public bool Help { get; set; }
        public bool LogNoVersion { get; set; }
        public bool RelayCheck { get; set; }
        public decimal MinBid { get; set; }
    }

    public class CommandLineOptionsBinder : BinderBase<CommandLineOptions>
    {
        private readonly Option<IEnumerable<string>> relay;
        private readonly Option<string> relays;
        private readonly Option<IEnumerable<string>> listen;
        private readonly Option<string> network;
        private readonly Option<string> genesisForkVersion;
        private readonly Option<decimal> minBid;
        private readonly Option<bool> relayCheck;
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
            Option<decimal> minBid,
            Option<bool> relayCheck,
            Option<int> requestHeader,
            Option<int> requestPayload,
            Option<int> requestReg,
            Option<bool> logNoVersion,
            Option<string> logLevel
,
            Option<string> logService
        //Option<bool> verionOpt
        )
        {
            this.logLevel = logLevel;
            this.relay = relay;
            this.relays = relays;
            this.listen = listen;
            this.network = network;
            this.genesisForkVersion = genesisForkVersion;
            this.minBid = minBid;
            this.relayCheck = relayCheck;
            this.requestHeader = requestHeader;
            this.requestPayload = requestPayload;
            this.requestReg = requestReg;
            this.logNoVersion = logNoVersion;
            this.logService = logService;
            //this.versionOpt = verionOpt;
        }

        protected override CommandLineOptions GetBoundValue(BindingContext bindingContext)
        {
            var model = new CommandLineOptions()
            {
                LogService = GetValue(bindingContext, logService),
                LogLevel = GetValue(bindingContext, logLevel),
                GenesisForkVersion = GetValue(bindingContext, genesisForkVersion),
                Relay = GetValue(bindingContext, relay),
                Relays = GetValue(bindingContext, relays),
                Listen = GetValue(bindingContext, listen),
                Network = GetValue(bindingContext, network),
                MinBid = GetValue(bindingContext, minBid),
                RelayCheck = GetValue(bindingContext, relayCheck),
                RequestTimeoutGetHeader = GetValue(bindingContext, requestHeader),
                RequestTimeoutGetPayload = GetValue(bindingContext, requestPayload),
                RequestTimeoutRegVal = GetValue(bindingContext, requestReg)
                //Version = GetValue(bindingContext, versionOpt),
            };
            return model;
        }

        private T GetValue<T>(BindingContext context, Option<T> option)
        {
            return context.ParseResult.GetValueForOption(option);
        }
    }
}
