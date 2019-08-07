using System.Threading;
using Microsoft.AspNetCore.Hosting;

namespace Quartz.DependencyInjection.Tests.Mocks
{
    class ApplicationLifetimeMock : IApplicationLifetime
    {
        public ApplicationLifetimeMock()
        {
            _applicationStartedSource.Cancel();
        }

        #region Implementation of IApplicationLifetime

        /// <summary>Requests termination of the current application.</summary>
        public void StopApplication()
        {
            _applicationStoppingSource.Cancel();
            _applicationStoppedSource.Cancel();
        }

        /// <summary>
        /// Triggered when the application host has fully started and is about to wait
        /// for a graceful shutdown.
        /// </summary>
        public CancellationToken ApplicationStarted => _applicationStartedSource.Token;
        private readonly CancellationTokenSource _applicationStartedSource = new CancellationTokenSource();

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// Requests may still be in flight. Shutdown will block until this event completes.
        /// </summary>
        public CancellationToken ApplicationStopping => _applicationStoppingSource.Token;
        private readonly CancellationTokenSource _applicationStoppingSource = new CancellationTokenSource();

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// All requests should be complete at this point. Shutdown will block
        /// until this event completes.
        /// </summary>
        public CancellationToken ApplicationStopped => _applicationStoppedSource.Token;
        private readonly CancellationTokenSource _applicationStoppedSource = new CancellationTokenSource();

        #endregion
    }
}