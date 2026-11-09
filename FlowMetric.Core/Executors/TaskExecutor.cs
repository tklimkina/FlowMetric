using System;
using System.Threading.Tasks;
using FlowMetric.Core.Workloads;

namespace FlowMetric.Core.Executors;

public class TaskExecutor : IExecutor
{
    public string Name => "Task.Run (Manual Tasks)";

    public long Run(IWorkload workload, int degreeOfParallelism)
    {

        int TotalItems = workload.itemsCount;

        long totalResult = 0;
        object lockObj = new object();

        int chunkSize = TotalItems / degreeOfParallelism;
        var tasks = new Task[degreeOfParallelism];

        for (int i = 0; i < degreeOfParallelism; i++)
        {
            int start = i * chunkSize;
            int end = (i == degreeOfParallelism - 1) ? TotalItems : start + chunkSize;

            tasks[i] = Task.Run(() =>
            {
                long localSum = 0;

                for (int j = start; j < end; j++)
                {
                    localSum += workload.Execute();
                }

                lock (lockObj)
                {
                    totalResult += localSum;
                }
            });
        }

        Task.WaitAll(tasks);

        return totalResult;
    }
}