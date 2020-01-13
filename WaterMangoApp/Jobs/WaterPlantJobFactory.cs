using System;
using Quartz;
using Quartz.Simpl;
using Quartz.Spi;

namespace WaterMangoApp.Jobs
{
    public class WaterPlantJobFactory : SimpleJobFactory
    {
        private IServiceProvider _provider;

        public WaterPlantJobFactory(IServiceProvider provider)
        {
            _provider = provider;      
        }
        
        public override IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            // Here we will use our own service provider that will inject all the dependencies required
            try
            {
                return (IJob)this._provider.GetService(bundle.JobDetail.JobType);
            }
            catch (Exception ex)
            {
               
                throw new SchedulerException(string.Format("Problem while instantiating job '{0}' from the Aspnet Core IOC.", bundle.JobDetail.Key));
            }
        }
    }
}