﻿namespace MEVSharp.UI.API.Formatter
{
    public static class ConsoleLoggerExtensions
    {
        public static ILoggingBuilder AddCustomFormatter(
            this ILoggingBuilder builder,
            Action<CustomOptions> configure) =>
            builder.AddConsole(options => options.FormatterName = "customName")
                .AddConsoleFormatter<PrefixConsoleFormatter, CustomOptions>(configure);
    }
}
