using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using Quartz.DependencyInjection.Logging;
using Quartz.DependencyInjection.Tests.Mocks;
using Xunit;
using LogLevel = Quartz.Logging.LogLevel;

namespace Quartz.DependencyInjection.Tests
{
    public class CoreLogProviderTests
    {
        /// <summary>
        /// ILogger Mock
        /// </summary>
        private readonly LoggerMock _log;

        /// <summary>
        /// Core Log Provider to test against.
        /// </summary>
        private readonly CoreLogProvider _provider;

        public CoreLogProviderTests()
        {
            _log = new LoggerMock();
            var loggerFactoryMock = new Mock<ILoggerFactory>();
            loggerFactoryMock
                .Setup(m => m.CreateLogger(It.IsAny<string>()))
                .Returns(_log);
            _provider = new CoreLogProvider(loggerFactoryMock.Object);
        }

        [Fact]
        public void LoggerTests()
        {
            var logger = _provider.GetLogger("XUnit");

            // No Formatting Function
            Assert.True(logger(LogLevel.Debug, null, null, new object[] { "Message" }));
            _log.Assert(null, null, null, null);

            // Trace Log Entry
            Assert.True(logger(LogLevel.Trace, () => "Test {0}", null, new object[] { "Message" }));
            _log.Assert(Microsoft.Extensions.Logging.LogLevel.Trace, 0, null, "XUnit Test Message");

            // Debug Log Entry
            Assert.True(logger(LogLevel.Debug, () => "Test {0}", null, new object[] { "Message" }));
            _log.Assert(Microsoft.Extensions.Logging.LogLevel.Debug, 0, null, "XUnit Test Message");

            // Info Log Entry
            Assert.True(logger(LogLevel.Info, () => "Test {0}", null, new object[] { "Message" }));
            _log.Assert(Microsoft.Extensions.Logging.LogLevel.Information, 0, null, "XUnit Test Message");

            // Warn Log Entry
            Assert.True(logger(LogLevel.Warn, () => "Test {0}", null, new object[] { "Message" }));
            _log.Assert(Microsoft.Extensions.Logging.LogLevel.Warning, 0, null, "XUnit Test Message");

            // Error Log Entry
            Assert.True(logger(LogLevel.Error, () => "Test {0}", null, new object[] { "Message" }));
            _log.Assert(Microsoft.Extensions.Logging.LogLevel.Error, 0, null, "XUnit Test Message");

            // Fatal Log Entry
            Assert.True(logger(LogLevel.Fatal, () => "Test {0}", null, new object[] { "Message" }));
            _log.Assert(Microsoft.Extensions.Logging.LogLevel.Critical, 0, null, "XUnit Test Message");

            // Exception
            var e = new Exception("Something Bad");
            Assert.True(logger(LogLevel.Fatal, () => "Test {0}", e, new object[] { "Message" }));
            _log.Assert(Microsoft.Extensions.Logging.LogLevel.Critical, 0, null, $"XUnit Test Message -> {e}");

            // Invalid LogLevel
            Assert.Throws<ArgumentOutOfRangeException>(() => logger((LogLevel)6, () => "Test {0}", e, new object[] { "Message" }));
        }

        [Fact]
        public void FormatStructuredMessageTests()
        {
            // No Format Params
            Assert.Equal("Test", CoreLogProvider.FormatStructuredMessage("Test", new object[0], out var matches));
            Assert.Empty(matches);

            // With Params
            Assert.Equal("Test 123.45", CoreLogProvider.FormatStructuredMessage("Test {0:0.00}", new object[1] { 123.451 }, out matches));
            Assert.Empty(matches);

            // With Pattern Matched Params
            Assert.Equal("Test 123.451", CoreLogProvider.FormatStructuredMessage("Test {number}", new object[1] { 123.451 }, out matches));
            Assert.Single(matches);
            Assert.Equal("number", matches.First());

            // Invalid Format String exception
            Assert.Throws<FormatException>(() => CoreLogProvider.FormatStructuredMessage("Test {0:Q2}", new object[1] { 123.451 }, out matches));
        }

        [Fact]
        public void OpenNestedContextTest()
        {
            using (var context = _provider.OpenNestedContext("XUnit"))
            {
                Assert.NotNull(context);
                Assert.IsAssignableFrom<IDisposable>(context);
            }
        }

        [Fact]
        public void OpenMappedContextTest()
        {
            using (var context = _provider.OpenMappedContext("XUnit", "XUnit"))
            {
                Assert.NotNull(context);
                Assert.IsAssignableFrom<IDisposable>(context);
            }
        }

        [Fact]
        public void ReplaceFirstTests()
        {
            Assert.Equal("Test Message", CoreLogProvider.ReplaceFirst("Test {msg}", "{msg}", "Message"));
            Assert.Equal("Test {msg}", CoreLogProvider.ReplaceFirst("Test {msg}", "{message}", "Message"));
        }
    }
}
