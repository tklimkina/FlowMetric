using System.Threading;

namespace FlowMetric.Core.Workloads;

public class InterlockedWorkload : IWorkload
{
    private long _counter;

    public string Name => "Lock-Free (Interlocked)";

    public long Execute()
    {
        double sum = 0;
        for (int i = 0; i < 500; i++)
        {
            sum += Math.Sqrt(i);
        }

        for (int i = 0; i < 5; i++)
        {
            Interlocked.Increment(ref _counter);
        }

        return _counter;
    }
}