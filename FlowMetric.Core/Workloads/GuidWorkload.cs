using FlowMetric.Core.Workloads;
using System;

namespace ConcurrencyExplorer.Core.Workloads;

public class GuidWorkload : IWorkload
{
    public string Name => "GUID Generation (CPU-Light)";

    public long Execute()
    {
        var guid = Guid.NewGuid();
        return guid.GetHashCode();
    }
}