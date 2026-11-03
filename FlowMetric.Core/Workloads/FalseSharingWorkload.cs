namespace FlowMetric.Core.Workloads;

public class FalseSharingWorkload : IWorkload
{
    private long[] _counters;
    private int _threadId;

    public FalseSharingWorkload()
    {
        _counters = new long[16];
    }

    public string Name => "False Sharing (Bad)";

    public void SetThreadId(int threadId)
    {
        _threadId = threadId;
    }

    public long Execute()
    {
        for (int i = 0; i < 100; i++)
        {
            _counters[_threadId]++;
        }

        return _counters[_threadId];
    }
}