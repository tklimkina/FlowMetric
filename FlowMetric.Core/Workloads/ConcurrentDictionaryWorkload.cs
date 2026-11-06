using System.Collections.Concurrent;
using System.Threading;

namespace FlowMetric.Core.Workloads;

public class ConcurrentDictionaryWorkload : IWorkload
{
    private readonly ConcurrentDictionary<int, int> _dict = new();

    private int _counter;

    public string Name => "ConcurrentDictionary (AddOrUpdate)";

    public long Execute()
    {
        int key = Interlocked.Increment(ref _counter) % 100; // Ограничиваем до 100 ключей, чтобы создать конкуренцию
        int value = key * 2;

        _dict.AddOrUpdate(key, value, (k, v) => v + 1);

        return _dict.Count;
    }
}