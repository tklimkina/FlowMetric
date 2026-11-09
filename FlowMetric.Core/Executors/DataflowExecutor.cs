using System.Threading.Tasks.Dataflow;
using FlowMetric.Core.Workloads;

namespace FlowMetric.Core.Executors;

public class DataflowExecutor : IExecutor
{
    public string Name => "TPL Dataflow (Pipeline)";

    public long Run(IWorkload workload, int degreeOfParallelism)
    {
        int itt = workload.itemsCount;
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
                Interlocked.Add(ref totalResult, result);
            },
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 1 
            });

        // Lego Building
        inputBuffer.LinkTo(processBlock, new DataflowLinkOptions { PropagateCompletion = true });
        processBlock.LinkTo(outputBlock, new DataflowLinkOptions { PropagateCompletion = true });

        for (int i = 0; i < itt; i++)
        {
            inputBuffer.Post(i);
        }

        inputBuffer.Complete();
        outputBlock.Completion.Wait();

        return totalResult;
    }
}