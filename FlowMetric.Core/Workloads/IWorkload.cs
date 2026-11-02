using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowMetric.Core.Workloads
{
    public interface IWorkload
    {
        string Name { get; }
        long Execute();
    }
}
