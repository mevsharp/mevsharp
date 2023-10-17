using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using Nethermind.Logging;
using System;

namespace MEVSharp.UI.API.Formatter
{
    public sealed class PrefixConsoleFormatter : ConsoleFormatter, IDisposable
    {
        private readonly IDisposable? _optionsReloadToken;
        private CustomOptions _formatterOptions;

        public PrefixConsoleFormatter(IOptionsMonitor<CustomOptions> options)
            // Case insensitive
            : base("customName") =>
            (_optionsReloadToken, _formatterOptions) =
                (options.OnChange(ReloadLoggerOptions), options.CurrentValue);

        private void ReloadLoggerOptions(CustomOptions options) =>
            _formatterOptions = options;

        public override void Write<TState>(
            in LogEntry<TState> logEntry,
            IExternalScopeProvider? scopeProvider,
            TextWriter textWriter)
        {
            string? message =
                logEntry.Formatter?.Invoke(
                    logEntry.State, logEntry.Exception);

            if (message is null)
            {
                return;
            }

            CustomLogicGoesHere(textWriter);
            textWriter.WriteLine(message);
        }

        private void CustomLogicGoesHere(TextWriter textWriter)
        {
            var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var prefix = (string.IsNullOrEmpty(_formatterOptions.CustomPrefix) ? " " : $" {_formatterOptions.CustomPrefix} ");

            var text = $"{time}{prefix}";
            textWriter.Write(text);
        }

        public void Dispose() => _optionsReloadToken?.Dispose();
    }
}
