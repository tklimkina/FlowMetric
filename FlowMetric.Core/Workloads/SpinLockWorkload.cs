namespace FlowMetric.Core.Workloads;

public class SpinLockWorkload : IWorkload
{
    private SpinLock _spinLock = new(false);
    private int _value;

    public string Name => "SpinLock (Busy Wait)";

    public long Execute()
    {
        bool lockTaken = false;
        try
        {
            _spinLock.Enter(ref lockTaken);
            _value++;
            return _value;
        }
        finally
        {
            if (lockTaken)
            {
                _spinLock.Exit();
            }
        }
    }
}