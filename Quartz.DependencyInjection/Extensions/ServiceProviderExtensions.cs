using System;
using Microsoft.AspNetCore.Hosting;
using Quartz;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Start the Scheduler by fetching the IScheduler implementation from Core DI.
        ///
        /// If the IApplicationLifetime service is available, will register shutdown as well for ASP.NET Core applications.
        /// </summary>
        /// <param name="provider">Core DI Provider.</param>
        /// <param name="timeoutMs">Timeout in Ms to wait when shutting down scheduler.</param>
        /// <returns></returns>
        public static IServiceProvider StartQuartz(this IServiceProvider provider, int timeoutMs = 30000)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (timeoutMs < 0)
                throw new ArgumentOutOfRangeException(nameof(timeoutMs), "Timeout cannot be a negative value.");
            var scheduler = provider.GetRequiredService<IScheduler>();
            var lifetime = provider.GetService<IApplicationLifetime>();
            lifetime?.ApplicationStopping.Register(() => scheduler.Shutdown(waitForJobsToComplete: true).Wait(timeoutMs));
            scheduler.Start();
            return provider;
        }
    }
}