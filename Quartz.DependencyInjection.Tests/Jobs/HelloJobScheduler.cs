using Quartz;
using Quartz.DependencyInjection.Interfaces;

namespace Quartz.DependencyInjection.Tests.Jobs
{
    class HelloJobScheduler : IAddScheduledJob
    {
        public IJobDetail GetJob()
        {
            return JobBuilder.Create<HelloJob>()
                .WithIdentity("job1", "group1")
                .Build();
        }

        public ITrigger GetTrigger()
        {
            return TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(1)
                    .WithRepeatCount(1) // Total will be this + 1 according to docs!
                )
                .Build();
        }
    }
}