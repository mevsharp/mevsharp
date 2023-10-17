using Microsoft.Extensions.Logging.Console;

namespace MEVSharp.UI.API.Formatter
{
    public sealed class CustomOptions : ConsoleFormatterOptions
    {
        public string? CustomPrefix { get; set; }
    }
}
