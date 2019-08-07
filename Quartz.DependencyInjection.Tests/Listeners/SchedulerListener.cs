using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Quartz.DependencyInjection.Interfaces;

namespace Quartz.DependencyInjection.Tests.Listeners
{
    class SchedulerListener : IAddSchedulerListener, ISchedulerListener
    {
        public SchedulerListener()
        {
            Called = new List<string>();
        }

        /// <summary>
        /// List of called Listener Functions
        /// </summary>
        public IList<string> Called { get; }

        #region Implementation of IAddSchedulerListener

        /// <summary>
        /// Scheduler Listener to add.
        /// </summary>
        public ISchedulerListener Listener => this;

        #endregion

        #region Implementation of ISchedulerListener

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler" /> when a <see cref="T:Quartz.IJobDetail" />
        /// is scheduled.
        /// </summary>
        public Task JobScheduled(ITrigger trigger, CancellationToken cancellationToken = new CancellationToken())
        {
            Called.Add($"JobScheduled(triggerName={trigger.Key.Name}; triggerGroup={trigger.Key.Group})");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler" /> when a <see cref="T:Quartz.IJobDetail" />
        /// is unscheduled.
        /// </summary>
        /// <seealso cref="M:Quartz.ISchedulerListener.SchedulingDataCleared(System.Threading.CancellationToken)" />
        public Task JobUnscheduled(TriggerKey triggerKey, CancellationToken cancellationToken = new CancellationToken())
        {
            Called.Add($"JobUnscheduled(triggerName={triggerKey.Name}; triggerGroup={triggerKey.Group})");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler" /> when a <see cref="T:Quartz.ITrigger" />
        /// has reached the condition in which it will never fire again.
        /// </summary>
        public Task TriggerFinalized(ITrigger trigger, CancellationToken cancellationToken = new CancellationToken())
        {
            Called.Add($"TriggerFinalized(triggerName={trigger.Key.Name}; triggerGroup={trigger.Key.Group})");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler" /> a <see cref="T:Quartz.ITrigger" />s has been paused.
        /// </summary>
        public Task TriggerPaused(TriggerKey triggerKey, CancellationToken cancellationToken = new CancellationToken())
        {
            Called.Add($"TriggerPaused(triggerName={triggerKey.Name}; triggerGroup={triggerKey.Group})");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler" /> a group of
        /// <see cref="T:Quartz.ITrigger" />s has been paused.
        /// </summary>
        /// <remarks>
        /// If a all groups were paused, then the <see param="triggerName" /> parameter
        /// will be null.
        /// </remarks>
        /// <param name="triggerGroup">The trigger group.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        public Task TriggersPaused(string triggerGroup, CancellationToken cancellationToken = new CancellationToken())
        {
            Called.Add($"TriggersPaused(triggerGroup={triggerGroup})");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler" /> when a <see cref="T:Quartz.ITrigger" />
        /// has been un-paused.
        /// </summary>
        public Task TriggerResumed(TriggerKey triggerKey, CancellationToken cancellationToken = new CancellationToken())
        {
            Called.Add($"TriggerResumed(triggerName={triggerKey.Name}; triggerGroup={triggerKey.Group})");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler" /> when a
        /// group of <see cref="T:Quartz.ITrigger" />s has been un-paused.
        /// </summary>
        /// <remarks>
        /// If all groups were resumed, then the <see param="triggerName" /> parameter
        /// will be null.
        /// </remarks>
        /// <param name="triggerGroup">The trigger group.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        public Task TriggersResumed(string triggerGroup, CancellationToken cancellationToken = new CancellationToken())
        {
            Called.Add($"TriggersResumed(triggerGroup={triggerGroup})");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler" /> when a <see cref="T:Quartz.IJobDetail" />
        /// has been added.
        /// </summary>
        public Task JobAdded(IJobDetail jobDetail, CancellationToken cancellationToken = new CancellationToken())
        {
            Called.Add($"JobAdded(jobName={jobDetail.Key.Name}; jobGroup={jobDetail.Key.Group})");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler" /> when a <see cref="T:Quartz.IJobDetail" />
        /// has been deleted.
        /// </summary>
        public Task JobDeleted(JobKey jobKey, CancellationToken cancellationToken = new CancellationToken())
        {
            Called.Add($"JobDeleted(jobName={jobKey.Name}; jobGroup={jobKey.Group})");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler" /> when a <see cref="T:Quartz.IJobDetail" />
        /// has been  paused.
        /// </summary>
        public Task JobPaused(JobKey jobKey, CancellationToken cancellationToken = new CancellationToken())
        {
            Called.Add($"JobPaused(jobName={jobKey.Name}; jobGroup={jobKey.Group})");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler" /> when a <see cref="T:Quartz.IJobDetail" />
        /// has been interrupted.
        /// </summary>
        public Task JobInterrupted(JobKey jobKey, CancellationToken cancellationToken = new CancellationToken())
        {
            Called.Add($"JobInterrupted(jobName={jobKey.Name}; jobGroup={jobKey.Group})");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler" /> when a
        /// group of <see cref="T:Quartz.IJobDetail" />s has been  paused.
        /// <para>
        /// If all groups were paused, then the <see param="jobName" /> parameter will be
        /// null. If all jobs were paused, then both parameters will be null.
        /// </para>
        /// </summary>
        /// <param name="jobGroup">The job group.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        public Task JobsPaused(string jobGroup, CancellationToken cancellationToken = new CancellationToken())
        {
            Called.Add($"JobsPaused(jobGroup={jobGroup})");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler" /> when a <see cref="T:Quartz.IJobDetail" />
        /// has been  un-paused.
        /// </summary>
        public Task JobResumed(JobKey jobKey, CancellationToken cancellationToken = new CancellationToken())
        {
            Called.Add($"JobResumed(jobName={jobKey.Name}; jobGroup={jobKey.Group})");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler" /> when a <see cref="T:Quartz.IJobDetail" />
        /// has been  un-paused.
        /// </summary>
        /// <param name="jobGroup">The job group.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        public Task JobsResumed(string jobGroup, CancellationToken cancellationToken = new CancellationToken())
        {
            Called.Add($"JobsResumed(jobGroup={jobGroup})");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler" /> when a serious error has
        /// occurred within the scheduler - such as repeated failures in the <see cref="T:Quartz.Spi.IJobStore" />,
        /// or the inability to instantiate a <see cref="T:Quartz.IJob" /> instance when its
        /// <see cref="T:Quartz.ITrigger" /> has fired.
        /// </summary>
        public Task SchedulerError(string msg, SchedulerException cause,
            CancellationToken cancellationToken = new CancellationToken())
        {
            Called.Add($"SchedulerError(msg={msg}; exception={cause})");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler" /> to inform the listener
        /// that it has move to standby mode.
        /// </summary>
        public Task SchedulerInStandbyMode(CancellationToken cancellationToken = new CancellationToken())
        {
            Called.Add("SchedulerInStandbyMode()");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler" /> to inform the listener
        /// that it has started.
        /// </summary>
        public Task SchedulerStarted(CancellationToken cancellationToken = new CancellationToken())
        {
            Called.Add("SchedulerStarted()");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler" /> to inform the listener that it is starting.
        /// </summary>
        public Task SchedulerStarting(CancellationToken cancellationToken = new CancellationToken())
        {
            Called.Add("SchedulerStarting()");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler" /> to inform the listener
        /// that it has Shutdown.
        /// </summary>
        public Task SchedulerShutdown(CancellationToken cancellationToken = new CancellationToken())
        {
            Called.Add("SchedulerShutdown()");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler" /> to inform the listener
        /// that it has begun the shutdown sequence.
        /// </summary>
        public Task SchedulerShuttingdown(CancellationToken cancellationToken = new CancellationToken())
        {
            Called.Add("SchedulerShuttingdown()");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler" /> to inform the listener
        /// that all jobs, triggers and calendars were deleted.
        /// </summary>
        public Task SchedulingDataCleared(CancellationToken cancellationToken = new CancellationToken())
        {
            Called.Add("SchedulingDataCleared()");
            return Task.CompletedTask;
        }

        #endregion
    }
}