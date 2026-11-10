using System.Linq;
using FlowMetric.Core.Workloads;

namespace FlowMetric.Core.Executors;

public class ParallelLINQExecutor : IExecutor
{
    public string Name => "PLINQ (Parallel LINQ)";

    public long Run(IWorkload workload, int degreeOfParallelism)
    {
        int TotalItems = workload.itemsCount;

        long totalResult = 0;

        foreach (var result in Enumerable.Range(0, TotalItems)
            .AsParallel()
            .WithDegreeOfParallelism(degreeOfParallelism)
            .Select(_ => workload.Execute()))
        {
            totalResult += result;
        }

        return totalResult;
    }
}