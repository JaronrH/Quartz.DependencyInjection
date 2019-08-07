using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Quartz.DependencyInjection.Interfaces;

namespace Quartz.DependencyInjection.Tests.Listeners
{
    class TriggerListener : IAddTriggerListener, ITriggerListener
    {
        public TriggerListener()
        {
            Called = new List<string>();
        }

        /// <summary>
        /// List of called Listener Functions
        /// </summary>
        public IList<string> Called { get; }

        #region Implementation of IAddTriggerListener

        /// <summary>
        /// Matching criteria for listener.  Return null to match on all triggers.
        /// </summary>
        public IEnumerable<IMatcher<TriggerKey>> GetMatchers()
        {
            return null;
        }

        /// <summary>
        /// Trigger Listener to add.
        /// </summary>
        public ITriggerListener Listener => this;

        #endregion

        #region Implementation of ITriggerListener

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler" /> when a <see cref="T:Quartz.ITrigger" />
        /// has fired, and it's associated <see cref="T:Quartz.IJobDetail" />
        /// is about to be executed.
        /// <para>
        /// It is called before the <see cref="M:Quartz.ITriggerListener.VetoJobExecution(Quartz.ITrigger,Quartz.IJobExecutionContext,System.Threading.CancellationToken)" /> method of this
        /// interface.
        /// </para>
        /// </summary>
        /// <param name="trigger">The <see cref="T:Quartz.ITrigger" /> that has fired.</param>
        /// <param name="context">
        ///     The <see cref="T:Quartz.IJobExecutionContext" /> that will be passed to the <see cref="T:Quartz.IJob" />'s<see cref="M:Quartz.IJob.Execute(Quartz.IJobExecutionContext)" /> method.
        /// </param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        public Task TriggerFired(ITrigger trigger, IJobExecutionContext context,
            CancellationToken cancellationToken = new CancellationToken())
        {
            Called.Add($"TriggerFired(triggerName={trigger.Key.Name}; triggerGroup={trigger.Key.Group}; jobName={context.JobDetail.Key.Name}; jobGroup={context.JobDetail.Key.Group})");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler" /> when a <see cref="T:Quartz.ITrigger" />
        /// has fired, and it's associated <see cref="T:Quartz.IJobDetail" />
        /// is about to be executed.
        /// <para>
        /// It is called after the <see cref="M:Quartz.ITriggerListener.TriggerFired(Quartz.ITrigger,Quartz.IJobExecutionContext,System.Threading.CancellationToken)" /> method of this
        /// interface.  If the implementation vetoes the execution (via
        /// returning <see langword="true" />), the job's execute method will not be called.
        /// </para>
        /// </summary>
        /// <param name="trigger">The <see cref="T:Quartz.ITrigger" /> that has fired.</param>
        /// <param name="context">The <see cref="T:Quartz.IJobExecutionContext" /> that will be passed to
        /// the <see cref="T:Quartz.IJob" />'s<see cref="M:Quartz.IJob.Execute(Quartz.IJobExecutionContext)" /> method.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>Returns true if job execution should be vetoed, false otherwise.</returns>
        public Task<bool> VetoJobExecution(ITrigger trigger, IJobExecutionContext context,
            CancellationToken cancellationToken = new CancellationToken())
        {
            Called.Add($"VetoJobExecution(triggerName={trigger.Key.Name}; triggerGroup={trigger.Key.Group}; jobName={context.JobDetail.Key.Name}; jobGroup={context.JobDetail.Key.Group})");
            return Task.FromResult(false);
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler" /> when a <see cref="T:Quartz.ITrigger" />
        /// has misfired.
        /// <para>
        /// Consideration should be given to how much time is spent in this method,
        /// as it will affect all triggers that are misfiring.  If you have lots
        /// of triggers misfiring at once, it could be an issue it this method
        /// does a lot.
        /// </para>
        /// </summary>
        /// <param name="trigger">The <see cref="T:Quartz.ITrigger" /> that has misfired.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        public Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = new CancellationToken())
        {
            Called.Add($"TriggerMisfired(triggerName={trigger.Key.Name}; triggerGroup={trigger.Key.Group})");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler" /> when a <see cref="T:Quartz.ITrigger" />
        /// has fired, it's associated <see cref="T:Quartz.IJobDetail" />
        /// has been executed, and it's <see cref="M:Quartz.Spi.IOperableTrigger.Triggered(Quartz.ICalendar)" /> method has been
        /// called.
        /// </summary>
        /// <param name="trigger">The <see cref="T:Quartz.ITrigger" /> that was fired.</param>
        /// <param name="context">
        /// The <see cref="T:Quartz.IJobExecutionContext" /> that was passed to the
        /// <see cref="T:Quartz.IJob" />'s<see cref="M:Quartz.IJob.Execute(Quartz.IJobExecutionContext)" /> method.
        /// </param>
        /// <param name="triggerInstructionCode">
        /// The result of the call on the <see cref="T:Quartz.ITrigger" />'s<see cref="M:Quartz.Spi.IOperableTrigger.Triggered(Quartz.ICalendar)" />  method.
        /// </param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        public Task TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode,
            CancellationToken cancellationToken = new CancellationToken())
        {
            Called.Add($"TriggerComplete(triggerName={trigger.Key.Name}; triggerGroup={trigger.Key.Group}; jobName={context.JobDetail.Key.Name}; jobGroup={context.JobDetail.Key.Group})");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Get the name of the <see cref="T:Quartz.ITriggerListener" />.
        /// </summary>
        public string Name => GetType().Name;

        #endregion
    }
}