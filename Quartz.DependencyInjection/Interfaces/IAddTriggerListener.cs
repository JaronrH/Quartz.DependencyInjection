using System.Collections.Generic;

namespace Quartz.DependencyInjection.Interfaces
{
    /// <summary>
    /// Interface for adding a Job Listener
    /// </summary>
    public interface IAddTriggerListener
    {
        /// <summary>
        /// Matching criteria for listener.  Return null to match on all triggers.
        /// </summary>
        IEnumerable<IMatcher<TriggerKey>> GetMatchers();

        /// <summary>
        /// Trigger Listener to add.
        /// </summary>
        ITriggerListener Listener { get; }
    }
}