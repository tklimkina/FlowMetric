using System.Threading.Tasks;
using FlowMetric.Core.Workloads;

namespace FlowMetric.Core.Executors;

public class ParallelExecutor : IExecutor
{
    public string Name => "Parallel.ForEach";

    public void Run(IWorkload workload, int degreeOfParallelism)
    {
        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = degreeOfParallelism
        };

        // A lot of parallel workloads
        Parallel.For(0, 10000, options, i =>
        {
            workload.Execute();
        });
    }
}