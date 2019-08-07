using System;
using Microsoft.Extensions.Logging;

namespace Quartz.DependencyInjection.Tests.Mocks
{
    class LoggerMock : ILogger
    {
        public LoggerMock()
        {
            Reset();
        }

        /// <summary>
        /// Reset captured log data.
        /// </summary>
        public void Reset()
        {
            LogLevel = null;
            EventId = null;
            Exception = null;
            Message = null;
        }

        /// <summary>
        /// Assert that the values are as expected.
        /// </summary>
        /// <param name="logLevel">Log Level</param>
        /// <param name="eventId">Event ID</param>
        /// <param name="exception">Exception</param>
        /// <param name="message">Formatted Message</param>
        public void Assert(LogLevel? logLevel, EventId? eventId, Exception exception, string message)
        {
            Xunit.Assert.Equal(LogLevel, LogLevel);
            Xunit.Assert.Equal(eventId, EventId);
            Xunit.Assert.Equal(exception, Exception);
            Xunit.Assert.Equal(message, Message);
            Xunit.Assert.True(IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug));
            using (var scope = BeginScope("Test"))
                Xunit.Assert.IsAssignableFrom<IDisposable>(scope);
        }

        /// <summary>
        /// Last LogLevel called.
        /// </summary>
        public LogLevel? LogLevel { get; private set; }

        /// <summary>
        /// Last Event Id.
        /// </summary>
        public EventId? EventId { get; private set; }

        /// <summary>
        /// Last Exception passed in.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Last Message after running through the formatter.
        /// </summary>
        public string Message { get; set; }

        #region Implementation of ILogger

        /// <summary>Writes a log entry.</summary>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">Id of the event.</param>
        /// <param name="state">The entry to be written. Can be also an object.</param>
        /// <param name="exception">The exception related to this entry.</param>
        /// <param name="formatter">Function to create a <c>string</c> message of the <paramref name="state" /> and <paramref name="exception" />.</param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            LogLevel = logLevel;
            EventId = eventId;
            Exception = exception;
            Message = formatter(state, exception);
        }

        /// <summary>
        /// Checks if the given <paramref name="logLevel" /> is enabled.
        /// </summary>
        /// <param name="logLevel">level to be checked.</param>
        /// <returns><c>true</c> if enabled.</returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        /// <summary>Begins a logical operation scope.</summary>
        /// <param name="state">The identifier for the scope.</param>
        /// <returns>An IDisposable that ends the logical operation scope on dispose.</returns>
        public IDisposable BeginScope<TState>(TState state)
        {
            return NullDisposable.Instance;
        }

        #endregion

        private class NullDisposable : IDisposable
        {
            internal static readonly IDisposable Instance = new NullDisposable();

            public void Dispose()
            { }
        }
    }
}
