using System.Threading.Tasks;
using Quartz;

namespace Quartz.DependencyInjection.Tests.Jobs
{
    public class HelloJob : IJob
    {
        public HelloJob()
        {
            _counter = 0;
        }

        public int Counter { get { lock (_lock) return _counter; } }

        private int _counter { get; set; }

        private readonly object _lock = new object();

        Task IJob.Execute(IJobExecutionContext context)
        {
            lock (_lock) _counter++;
            return Task.CompletedTask;
        }
    }
}