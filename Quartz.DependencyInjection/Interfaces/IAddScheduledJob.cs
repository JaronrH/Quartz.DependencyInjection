namespace Quartz.DependencyInjection.Interfaces
{
    /// <summary>
    /// Interface for creating a new Job
    /// </summary>
    public interface IAddScheduledJob
    {
        /// <summary>
        /// Job Setup (JobBuilder.Create)
        /// </summary>
        IJobDetail GetJob();

        /// <summary>
        /// Trigger Setup (TriggerBuilder.Create)
        /// </summary>
        ITrigger GetTrigger();
    }
}