using System.Threading.Channels;
using System.Threading.Tasks;
using FlowMetric.Core.Workloads;

namespace FlowMetric.Core.Executors;

public class ChannelExecutor : IExecutor
{
    public string Name => "Channel (Producer-Consumer)";

    public long Run(IWorkload workload, int degreeOfParallelism)
    {
        int itt = workload.itemsCount;

        var channel = Channel.CreateUnbounded<int>();

        long totalResult = 0;
        object lockObj = new object();

        // Producer
        var producerTask = Task.Run(async () =>
        {
            for (int i = 0; i < itt; i++)
            {
                await channel.Writer.WriteAsync(i);
            }
            channel.Writer.Complete();
        });

        // Consumer
        var consumerTasks = new Task[degreeOfParallelism];

        for (int i = 0; i < degreeOfParallelism; i++)
        {
            consumerTasks[i] = Task.Run(async () =>
            {
                long localSum = 0;

                await foreach (var item in channel.Reader.ReadAllAsync())
                {
                    localSum += workload.Execute();
                }

                lock (lockObj)
                {
                    totalResult += localSum;
                }
            });
        }

        Task.WaitAll(consumerTasks);
        producerTask.Wait();

        return totalResult;
    }
}