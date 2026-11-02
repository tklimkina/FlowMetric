namespace FlowMetric.Core.Workloads;

public class ContentionWorkload : IWorkload
{
    private readonly object _lock = new();
    private long _counter;

    public string Name => "Contention (lock on shared counter)";

    // Measuring work with a critical section
    public long Execute()
    {
        // Calculations to create workload
        double sum = 0;
        for (int i = 0; i < 500; i++)
        {
            sum += Math.Sqrt(i);
        }

        // Concurent measures
        for (int i = 0; i < 5; i++)
        {
            lock (_lock)
            {
                _counter++;
            }
        }

        return _counter;
    }
}