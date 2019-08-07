using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Quartz.DependencyInjection.Interfaces;

namespace Quartz.DependencyInjection.Tests.Listeners
{
    class JobListener : IAddJobListener, IJobListener
    {
        public JobListener()
        {
            Called = new List<string>();
        }

        /// <summary>
        /// List of called Listener Functions
        /// </summary>
        public IList<string> Called { get; }

        #region Implementation of IAddJobListener

        /// <summary>
        /// Matching criteria for listener.  Return null to match on all jobs.
        /// </summary>
        public IEnumerable<IMatcher<JobKey>> GetMatchers()
        {
            return null;
        }

        /// <summary>
        /// Job Listener to add.
        /// </summary>
        public IJobListener Listener => this;

        #endregion

        #region Implementation of IJobListener

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler" /> when a <see cref="T:Quartz.IJobDetail" />
        /// is about to be executed (an associated <see cref="T:Quartz.ITrigger" />
        /// has occurred).
        /// <para>
        /// This method will not be invoked if the execution of the Job was vetoed
        /// by a <see cref="T:Quartz.ITriggerListener" />.
        /// </para>
        /// </summary>
        /// <seealso cref="M:Quartz.IJobListener.JobExecutionVetoed(Quartz.IJobExecutionContext,System.Threading.CancellationToken)" />
        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            Called.Add($"JobToBeExecuted(jobName={context.JobDetail.Key.Name}; jobGroup={context.JobDetail.Key.Group})");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler" /> when a <see cref="T:Quartz.IJobDetail" />
        /// was about to be executed (an associated <see cref="T:Quartz.ITrigger" />
        /// has occurred), but a <see cref="T:Quartz.ITriggerListener" /> vetoed it's
        /// execution.
        /// </summary>
        /// <seealso cref="M:Quartz.IJobListener.JobToBeExecuted(Quartz.IJobExecutionContext,System.Threading.CancellationToken)" />
        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            Called.Add($"JobExecutionVetoed(jobName={context.JobDetail.Key.Name}; jobGroup={context.JobDetail.Key.Group})");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler" /> after a <see cref="T:Quartz.IJobDetail" />
        /// has been executed, and be for the associated <see cref="T:Quartz.Spi.IOperableTrigger" />'s
        /// <see cref="M:Quartz.Spi.IOperableTrigger.Triggered(Quartz.ICalendar)" /> method has been called.
        /// </summary>
        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException,
            CancellationToken cancellationToken = new CancellationToken())
        {
            Called.Add($"JobWasExecuted(jobName={context.JobDetail.Key.Name}; jobGroup={context.JobDetail.Key.Group})");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Get the name of the <see cref="T:Quartz.IJobListener" />.
        /// </summary>
        public string Name => GetType().Name;

        #endregion
    }
}