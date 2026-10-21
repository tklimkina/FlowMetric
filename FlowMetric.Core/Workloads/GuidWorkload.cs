using FlowMetric.Core.Workloads;
using System;

namespace ConcurrencyExplorer.Core.Workloads;

public class GuidWorkload : IWorkload
{
    public string Name => "GUID Generation (CPU-Light)";

    public void Execute()
    {
        var guid = Guid.NewGuid();
        _ = guid.ToString();
    }
}