using System.Threading;

namespace FlowMetric.Core.Workloads;

public class ReaderWriterLockSlimWorkload : IWorkload
{
    private readonly ReaderWriterLockSlim _lock = new();
    private int _value;
    private int _readCount;

    private readonly Random _random = new(42);

    public string Name => "ReaderWriterLockSlim";

    public long Execute()
    {
        long result = 0;

        // 80% is probability of reading, 20% is probability of writing
        if (_random.Next(100) < 80)
        {
            _lock.EnterReadLock();
            try
            {
                result = _value;
                _readCount++;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
        else
        {
            _lock.EnterWriteLock();
            try
            {
                _value++;
                result = _value;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        return result;
    }
}