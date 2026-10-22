namespace FlowMetric.Core.Workloads;

public class ContentionWorkload : IWorkload
{
    private readonly object _lock = new();
    private long _counter;

    public string Name => "Contention (lock on shared counter)";

    public void Execute()
    {
        // Measuring work with a critical section
        lock (_lock)
        {
            _counter++;
        }
    }
}