using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Quartz;

namespace WaterMangoApp.Jobs
{
    public class TriggerListener  : ITriggerListener
    {
        public string Name => "Water Plant Trigger Listener";
        
        public async Task TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode, CancellationToken cancellationToken = default(CancellationToken))
        {
            Debug.WriteLine($"Trigger completed : {trigger.Key.Name}");
        }

        public async Task TriggerFired(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            Debug.WriteLine($"Trigger fired : {trigger.Key.Name}");
        }

        public async Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = default(CancellationToken))
        {
            Debug.WriteLine($"Trigger miss-fired : {trigger.Key.Name}");
        }

        public async Task<bool> VetoJobExecution(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return false;
        }
    }
}