using Microsoft.Extensions.Logging;

namespace MEVSharp.Application.Configurations
{
    public class AppSettings
    {
        public int RequestTimeoutGetheader { get; set; }
        public int RequestTimeoutGetpayload { get; set; }
        public int RequestTimeoutRegval { get; set; }
        public int HttpRetryDelay { get; set; }
        public int HttpRetryCount { get; set; }
        public bool VerifySignature { get; set; }
        public bool RelayCheck { get; set; }
        public bool NotificationTest { get; set; }
        public string Network { get; set; } = string.Empty;
        public LogLevel LogLevel { get; set; }
        public string GenesisForkVersion { get; set; }
        public string ZapierID { get; set; } = string.Empty;
        public string ZapierSecret { get; set; } = string.Empty;
        public string TelegramAPI { get; set; } = string.Empty;
        public string TelegramChatID { get; set; } = string.Empty;
        public List<string> RelayUrls { get; set; } = new List<string>();
        public List<String> HostPort { get; set; } = new List<string>();

        public decimal MinBid { get; set; }

        public AppSettings AddHostPort(string hostPort)
        {
            HostPort.Add(hostPort);
            return this;
        }

        public AppSettings ClearHosts()
        {
            HostPort.Clear();
            return this;
        }

        public AppSettings SetLoglevel(string v)
        {
            if (!Enum.TryParse<LogLevel>(v, true, out LogLevel _loglevel))
            {
                throw new Exception($"Invalid log level: {v}");
            }
            this.LogLevel = _loglevel;
            return this;
              
        }
    }
}
