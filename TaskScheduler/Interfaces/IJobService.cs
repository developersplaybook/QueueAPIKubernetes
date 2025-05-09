using TaskScheduler.Models;
using System.Threading.Tasks;

namespace TaskScheduler.Interfaces
{
    public interface IJobService
    {
        Task TriggerJobToggle(JobNames jobType);
    }
}
