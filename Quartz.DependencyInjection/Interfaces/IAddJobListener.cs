using System.Collections.Generic;

namespace Quartz.DependencyInjection.Interfaces
{
    /// <summary>
    /// Interface for adding a Job Listener
    /// </summary>
    public interface IAddJobListener
    {
        /// <summary>
        /// Matching criteria for listener.  Return null to match on all jobs.
        /// </summary>
        IEnumerable<IMatcher<JobKey>> GetMatchers();

        /// <summary>
        /// Job Listener to add.
        /// </summary>
        IJobListener Listener { get; }
    }
}
