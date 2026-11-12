using System.Collections.Concurrent;
using System.Threading;

namespace FlowMetric.Core.Workloads;

public class ConcurrentQueueWorkload : IWorkload
{
    private readonly ConcurrentQueue<int> _queue = new();
    private int _counter;

    public string Name => "ConcurrentQueue (Enqueue/Dequeue)";

    public long Execute()
    {
        int operation = Interlocked.Increment(ref _counter);

        // Producer/consumer emulation/ Odd & even
        if (operation % 2 == 0)
        {
            _queue.Enqueue(operation);
        }
        else
        {
            _queue.TryDequeue(out _);
        }

        return _queue.Count;
    }
}