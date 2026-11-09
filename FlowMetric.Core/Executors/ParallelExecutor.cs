using System.Threading.Tasks;
using FlowMetric.Core.Workloads;

namespace FlowMetric.Core.Executors;

public class ParallelExecutor : IExecutor
{
    public string Name => "Parallel.ForEach";

    public long Run(IWorkload workload, int degreeOfParallelism)
    {
        long totalResult = 0;
        var options = new ParallelOptions { MaxDegreeOfParallelism = degreeOfParallelism };
        object lockObj = new object();
        int itterations = workload.itemsCount;

        Parallel.For(0, itterations, options,
            () => 0L,
            (i, loopState, localSum) =>
            {
                localSum += workload.Execute();
                return localSum;
            },
            localSum =>
            {
                lock (lockObj)
                {
                    totalResult += localSum;
                }
            });

        return totalResult;
    }
}