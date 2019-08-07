namespace Quartz.DependencyInjection.Interfaces
{
    /// <summary>
    /// Interface for adding a Scheduler Listener
    /// </summary>
    public interface IAddSchedulerListener
    {
        /// <summary>
        /// Scheduler Listener to add.
        /// </summary>
        ISchedulerListener Listener { get; }
    }
}