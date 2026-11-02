using FlowMetric.Core.Workloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowMetric.Core.Executors
{
    public interface IExecutor
    {
        string Name { get; }
        long Run(IWorkload workload, int degreeOfParallelism);
    }
}
