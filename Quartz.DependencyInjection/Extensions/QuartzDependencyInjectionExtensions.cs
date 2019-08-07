using System;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.Logging;
using Quartz.Spi;
using Scrutor;
using Quartz.DependencyInjection;
using Quartz.DependencyInjection.Interfaces;
using Quartz.DependencyInjection.Logging;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /*
     * Best Scrutor Reference: https://andrewlock.net/using-scrutor-to-automatically-register-your-services-with-the-asp-net-core-di-container/
     */

    public static class QuartzDependencyInjectionExtensions
    {
        /// <summary>
        /// Add Quartz.NET support to Core DI.
        ///
        /// Registers interfaces IJob, IAddScheduledJob, IAddSchedulerListener, IAddTriggerListener, and IAddJobListener found in the assembly selectors functions.
        /// </summary>
        /// <param name="serviceCollection">Service Collection</param>
        /// <param name="assemblySelectors">Scrutor Assembly selectors to use for scanning.</param>
        /// <returns>Service Collection</returns>
        public static IServiceCollection AddQuartz(this IServiceCollection serviceCollection, params Func<IAssemblySelector, IImplementationTypeSelector>[] assemblySelectors)
        {
            return serviceCollection.AddQuartz(null, assemblySelectors);
        }

        /// <summary>
        /// Add Quartz.NET support to Core DI.
        ///
        /// Registers interfaces IJob, IAddScheduledJob, IAddSchedulerListener, IAddTriggerListener, and IAddJobListener found in the assembly selectors functions.
        /// </summary>
        /// <param name="serviceCollection">Service Collection</param>
        /// <param name="config">Name/Value Configuration for Quartz.NET</param>
        /// <param name="assemblySelectors">Scrutor Assembly selectors to use for scanning.</param>
        /// <returns>Service Collection</returns>
        public static IServiceCollection AddQuartz(this IServiceCollection serviceCollection, NameValueCollection config, params Func<IAssemblySelector, IImplementationTypeSelector>[] assemblySelectors)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            // Add Job Factory
            serviceCollection.AddSingleton<IJobFactory, JobFactory>();

            // Add Standard Scheduler
            serviceCollection.AddSingleton<IScheduler>(provider =>
            {
                var log = provider.GetRequiredService<ILoggerFactory>().CreateLogger("Quartz.NET");

                // Setup Logging
                LogProvider.SetCurrentLogProvider(provider.GetRequiredService<ILogProvider>());

                // Create Scheduler
                var schedulerFactory = config ==  null
                                        ? new StdSchedulerFactory()
                                        : new StdSchedulerFactory(config);
                var scheduler = schedulerFactory.GetScheduler().Result;

                // Setup Job Factory
                scheduler.JobFactory = provider.GetRequiredService<IJobFactory>();

                // Add Job Listeners
                try
                {
                    foreach (var jobListener in provider.GetServices<IAddJobListener>())
                        try
                        {
                            var matches = jobListener.GetMatchers();
                            scheduler.ListenerManager.AddJobListener(jobListener.Listener, matches == null ? new IMatcher<JobKey>[] {GroupMatcher<JobKey>.AnyGroup()} : matches.ToArray());
                            log.LogInformation($"Successfully added Job Listener as defined in {jobListener.GetType()}");
                        }
                        catch (Exception e)
                        {
                            log.LogError(e, $"Failed to add Job Listener for {jobListener.GetType()}");
                        }
                }
                catch (Exception e)
                {
                    log.LogError(e, "Failed to get IAddJobListener's from DI.");
                }

                // Add Trigger Listeners
                try
                {
                    foreach (var triggerListener in provider.GetServices<IAddTriggerListener>())
                        try
                        {
                            var matches = triggerListener.GetMatchers();
                            scheduler.ListenerManager.AddTriggerListener(triggerListener.Listener, matches == null ? new IMatcher<TriggerKey>[] { GroupMatcher<TriggerKey>.AnyGroup() } : matches.ToArray());
                            log.LogInformation($"Successfully added Trigger Listener as defined in {triggerListener.GetType()}");
                        }
                        catch (Exception e)
                        {
                            log.LogError(e, $"Failed to add Trigger Listener for {triggerListener.GetType()}");
                        }
                }
                catch (Exception e)
                {
                    log.LogError(e, "Failed to get IAddTriggerListener's from DI.");
                }

                // Add Schedule Listeners
                try
                {
                    foreach (var schedulerListener in provider.GetServices<IAddSchedulerListener>())
                        try
                        {
                            scheduler.ListenerManager.AddSchedulerListener(schedulerListener.Listener);
                            log.LogInformation($"Successfully added Scheduler Listener as defined in {schedulerListener.GetType()}");
                        }
                        catch (Exception e)
                        {
                            log.LogError(e, $"Failed to add Scheduler Listener for {schedulerListener.GetType()}");
                        }
                }
                catch (Exception e)
                {
                    log.LogError(e, "Failed to get IAddSchedulerListener's from DI.");
                }

                // Load Scheduled Jobs
                try
                {
                    foreach (var jobScheduler in provider.GetServices<IAddScheduledJob>())
                        try
                        {
                            scheduler.ScheduleJob(jobScheduler.GetJob(), jobScheduler.GetTrigger());
                            log.LogInformation($"Successfully scheduled job as defined in {jobScheduler.GetType()}");
                        }
                        catch (Exception e)
                        {
                            log.LogError(e, $"Failed to schedule job for {jobScheduler.GetType()}");
                        }
                }
                catch (Exception e)
                {
                    log.LogError(e, "Failed to get IAddScheduledJob's from DI.");
                }

                // Return Scheduler
                return scheduler;
            });

            // Add Logging Provider
            serviceCollection.AddSingleton<ILogProvider, CoreLogProvider>();

            // Register jobs & listeners found in assemblies using Scrutor
            foreach (var selector in assemblySelectors)
            {
                serviceCollection.Scan(s => selector(s)
                    .AddClasses(c => c.AssignableTo<IAddJobListener>())
                    .AsSelfWithInterfaces()
                    .WithSingletonLifetime()
                );
                serviceCollection.Scan(s => selector(s)
                    .AddClasses(c => c.AssignableTo<IAddTriggerListener>())
                    .AsSelfWithInterfaces()
                    .WithSingletonLifetime()
                );
                serviceCollection.Scan(s => selector(s)
                    .AddClasses(c => c.AssignableTo<IAddSchedulerListener>())
                    .AsSelfWithInterfaces()
                    .WithSingletonLifetime()
                );
                serviceCollection.Scan(s => selector(s)
                    .AddClasses(c => c.AssignableTo<IAddScheduledJob>())
                    .AsSelfWithInterfaces()
                    .WithSingletonLifetime()
                );
                serviceCollection.Scan(s => selector(s)
                    .AddClasses(c => c.AssignableTo<IJob>())
                    .AsSelfWithInterfaces() 
                    .WithSingletonLifetime()
                );
            }

            // Return service collection
            return serviceCollection;
        }
    }
}
