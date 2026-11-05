using System.Threading.Tasks.Dataflow;
using FlowMetric.Core.Workloads;

namespace FlowMetric.Core.Executors;

public class DataflowExecutor : IExecutor
{
    public string Name => "TPL Dataflow (Pipeline)";

    private const int TotalItems = 10000;

    public long Run(IWorkload workload, int degreeOfParallelism)
    {
        long totalResult = 0;

        // Input unit
        var inputBuffer = new BufferBlock<int>();

        // Processing Unit
        var processBlock = new TransformBlock<int, long>(
            item => workload.Execute(),
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = degreeOfParallelism,
                BoundedCapacity = DataflowBlockOptions.Unbounded
            });

        // Final Unit
        var outputBlock = new ActionBlock<long>(
            result =>
            {
                // Атомарное сложение без блокировок
                Interlocked.Add(ref totalResult, result);
            },
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 1 // Агрегация в одном потоке (или тоже можно распараллелить)
            });

        // 4. Соединяем
        inputBuffer.LinkTo(processBlock, new DataflowLinkOptions { PropagateCompletion = true });
        processBlock.LinkTo(outputBlock, new DataflowLinkOptions { PropagateCompletion = true });

        // 5. Заполняем
        for (int i = 0; i < TotalItems; i++)
        {
            inputBuffer.Post(i);
        }

        inputBuffer.Complete();
        outputBlock.Completion.Wait();

        return totalResult;
    }
}