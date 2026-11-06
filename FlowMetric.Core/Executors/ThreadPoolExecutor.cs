using System.Threading;
using FlowMetric.Core.Workloads;

namespace FlowMetric.Core.Executors;

public class ThreadPoolExecutor : IExecutor
{
    public string Name => "ThreadPool (QueueUserWorkItem)";

    private const int TotalItems = 10000;

    public long Run(IWorkload workload, int degreeOfParallelism)
    {
        long totalResult = 0;

        using var countdown = new CountdownEvent(degreeOfParallelism);

        int chunkSize = TotalItems / degreeOfParallelism;

        for (int i = 0; i < degreeOfParallelism; i++)
        {
            int start = i * chunkSize;
            int end = (i == degreeOfParallelism - 1) ? TotalItems : start + chunkSize;

            ThreadPool.QueueUserWorkItem(_ =>
            {
                long localSum = 0;
                try
                {
                    for (int j = start; j < end; j++)
                    {
                        localSum += workload.Execute();
                    }

                    Interlocked.Add(ref totalResult, localSum);
                }
                finally
                {
                    countdown.Signal();
                }
            });
        }
        countdown.Wait();

        return totalResult;
    }
}