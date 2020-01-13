using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Quartz;

namespace WaterMangoApp.Jobs
{
    public class JobListener : IJobListener
    {
        public string Name => "Water Plant Service";
        
        public async Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            Debug.WriteLine($"Job Vetoed : {context.JobDetail.Key.Name}");
        }

        public async Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            Debug.WriteLine($"Job Is to be Executed : {context.JobDetail.Key.Name}");
        }

        public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default(CancellationToken))
        {
            Debug.WriteLine($"Job Executed : {context.JobDetail.Key.Name}");
        }

    }
}