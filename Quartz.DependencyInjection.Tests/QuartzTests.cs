using System;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Quartz.DependencyInjection.Tests.Jobs;
using Quartz.DependencyInjection.Tests.Listeners;
using Quartz.DependencyInjection.Tests.Mocks;
using Xunit;

namespace Quartz.DependencyInjection.Tests
{
    public class QuartzTests
    {
        /// <summary>
        /// ILogger Mock
        /// </summary>
        private readonly ILogger _log = new LoggerMock();

        protected virtual IServiceProvider BuildContainer()
        {
            // Build Service Collection
            var serviceCollection = new ServiceCollection()
                    .AddSingleton<ILoggerFactory>(s =>
                    {
                        var loggerFactoryMock = new Mock<ILoggerFactory>();
                        loggerFactoryMock
                            .Setup(m => m.CreateLogger(It.IsAny<string>()))
                            .Returns(_log);
                        return loggerFactoryMock.Object;
                    })
                    .AddSingleton<IApplicationLifetime, ApplicationLifetimeMock>()
                    .AddQuartz(s => s.FromAssemblyOf<HelloJob>())
                ;
            return serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public void NullServiceCollectionTest()
        {
            ServiceCollection serviceCollection = null;
            Assert.Throws<ArgumentNullException>(() => serviceCollection.AddQuartz());
        }

        [Fact]
        public void NullContainerTest()
        {
            IServiceProvider container = null;
            Assert.Throws<ArgumentNullException>(() => container.StartQuartz());
        }

        [Fact]
        public void NegativeTimeoutTest()
        {
            var container = BuildContainer();
            Assert.Throws<ArgumentOutOfRangeException>(() => container.StartQuartz(-1));
        }

        [Fact]
        public void HelloWorldJobTest()
        {
            var container = BuildContainer();
            container.StartQuartz();
            var helloJob = container.GetRequiredService<HelloJob>();
            var jobListener = container.GetRequiredService<JobListener>();
            var triggerListener = container.GetRequiredService<TriggerListener>();
            var schedulerListener = container.GetRequiredService<SchedulerListener>();
            Thread.Sleep(2000);
            Assert.Equal(2, helloJob.Counter);
            Assert.Equal(4, jobListener.Called.Count);
            Assert.Contains("JobToBeExecuted(jobName=job1; jobGroup=group1)", jobListener.Called);
            Assert.Contains("JobWasExecuted(jobName=job1; jobGroup=group1)", jobListener.Called);
            Assert.Equal(6, triggerListener.Called.Count);
            Assert.Contains("TriggerFired(triggerName=trigger1; triggerGroup=group1; jobName=job1; jobGroup=group1)", triggerListener.Called);
            Assert.Contains("VetoJobExecution(triggerName=trigger1; triggerGroup=group1; jobName=job1; jobGroup=group1)", triggerListener.Called);
            Assert.Contains("TriggerComplete(triggerName=trigger1; triggerGroup=group1; jobName=job1; jobGroup=group1)", triggerListener.Called);
            Assert.Equal(6, schedulerListener.Called.Count);
            Assert.Contains("JobAdded(jobName=job1; jobGroup=group1)", schedulerListener.Called);
            Assert.Contains("JobScheduled(triggerName=trigger1; triggerGroup=group1)", schedulerListener.Called);
            Assert.Contains("SchedulerStarting()", schedulerListener.Called);
            Assert.Contains("SchedulerStarted()", schedulerListener.Called);
            Assert.Contains("TriggerFinalized(triggerName=trigger1; triggerGroup=group1)", schedulerListener.Called);
            Assert.Contains("JobDeleted(jobName=job1; jobGroup=group1)", schedulerListener.Called);
            container.GetRequiredService<IApplicationLifetime>().StopApplication();
            Assert.Contains("SchedulerShutdown()", schedulerListener.Called);
        }
    }
}
