using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Quartz.Logging;
using LogLevel = Quartz.Logging.LogLevel;

namespace Quartz.DependencyInjection.Logging
{
    /// <summary>
    /// Logging Provider for Quartz.NET based on the Console Provider.
    ///
    /// To Use:
    ///   1) Register the provider in DI if needed (AddSingleton<ILogProvider, CoreLogProvider>())
    ///   2) Set Log Provider for Quartz.NET: LogProvider.SetCurrentLogProvider(container.GetRequiredService<ILogProvider>());
    /// </summary>
    public class CoreLogProvider : ILogProvider
    {
        private readonly ILogger _log;

        public CoreLogProvider(ILoggerFactory logFactory)
        {
            _log = logFactory.CreateLogger("Quartz.NET");
        }

        public Logger GetLogger(string name)
        {
            return (logLevel, messageFunc, exception, formatParameters) =>
            {
                if (messageFunc == null)
                {
                    return true;
                }

                var formattedMessage = FormatStructuredMessage(messageFunc(), formatParameters, out _);

                if (exception != null)
                {
                    formattedMessage = formattedMessage + " -> " + exception;
                }

                switch (logLevel)
                {
                    case LogLevel.Trace:
                        _log.Log(Microsoft.Extensions.Logging.LogLevel.Trace, "{0} {1}", name, formattedMessage);
                        break;
                    case LogLevel.Debug:
                        _log.Log(Microsoft.Extensions.Logging.LogLevel.Debug, "{0} {1}", name, formattedMessage);
                        break;
                    case LogLevel.Info:
                        _log.Log(Microsoft.Extensions.Logging.LogLevel.Information, "{0} {1}", name, formattedMessage);
                        break;
                    case LogLevel.Warn:
                        _log.Log(Microsoft.Extensions.Logging.LogLevel.Warning, "{0} {1}", name, formattedMessage);
                        break;
                    case LogLevel.Error:
                        _log.Log(Microsoft.Extensions.Logging.LogLevel.Error, "{0} {1}", name, formattedMessage);
                        break;
                    case LogLevel.Fatal:
                        _log.Log(Microsoft.Extensions.Logging.LogLevel.Critical, "{0} {1}", name, formattedMessage);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
                }

                return true;
            };
        }

        private static readonly Regex Pattern = new Regex(@"(?<!{){@?(?<arg>[^ :{}]+)(?<format>:[^}]+)?}", RegexOptions.Compiled);

        public static string ReplaceFirst(string text, string search, string replace)
        {
            var pos = text.IndexOf(search, StringComparison.Ordinal);
            if (pos < 0)
                return text; // Should never get here so can't write a test for it!
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public static string FormatStructuredMessage(string targetMessage, object[] formatParameters, out IEnumerable<string> patternMatches)
        {
            if (formatParameters.Length == 0)
            {
                patternMatches = Enumerable.Empty<string>();
                return targetMessage;
            }

            var processedArguments = new List<string>();
            patternMatches = processedArguments;

            foreach (Match match in Pattern.Matches(targetMessage))
            {
                var arg = match.Groups["arg"].Value;

                if (int.TryParse(arg, out _)) continue;
                var argumentIndex = processedArguments.IndexOf(arg);
                if (argumentIndex == -1)
                {
                    argumentIndex = processedArguments.Count;
                    processedArguments.Add(arg);
                }

                targetMessage = ReplaceFirst(targetMessage, match.Value,
                    "{" + argumentIndex + match.Groups["format"].Value + "}");
            }
            try
            {
                return string.Format(CultureInfo.InvariantCulture, targetMessage, formatParameters);
            }
            catch (FormatException ex)
            {
                throw new FormatException("The input string '" + targetMessage + "' could not be formatted using string.Format", ex);
            }
        }

        public IDisposable OpenNestedContext(string message)
        {
            return NullDisposable.Instance;
        }

        /// <summary>
        /// Opens a mapped diagnostics context. Not supported in EntLib logging.
        /// </summary>
        /// <param name="key">A key.</param>
        /// <param name="value">A value.</param>
        /// <returns>A disposable that when disposed removes the map from the context.</returns>
        public IDisposable OpenMappedContext(string key, string value)
        { 
            return NullDisposable.Instance;
        }

        private class NullDisposable : IDisposable
        {
            internal static readonly IDisposable Instance = new NullDisposable();

            public void Dispose()
            { }
        }
    }
}
