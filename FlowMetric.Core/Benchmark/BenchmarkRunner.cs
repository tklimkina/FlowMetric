using System.Diagnostics;
using FlowMetric.Core.Executors;
using FlowMetric.Core.Workloads;

namespace FlowMetric.Core.Benchmark;

public class BenchmarkRunner
{
    private readonly IExecutor _executor;
    private readonly IWorkload _workload;

    public BenchmarkRunner(IExecutor executor, IWorkload workload)
    {
        _executor = executor;
        _workload = workload;
    }

    public List<BenchmarkResult> Run(int startThreads, int endThreads, int step)
    {
        var results = new List<BenchmarkResult>();

        for (int threads = startThreads; threads <= endThreads; threads += step)
        {
            var sw = Stopwatch.StartNew();

            _executor.Run(_workload, threads);

            sw.Stop();


            //10 000 iterations of ParallelExecutor
            double opsPerSec = 10000 / (sw.Elapsed.TotalSeconds > 0 ? sw.Elapsed.TotalSeconds : 1);

            results.Add(new BenchmarkResult
            {
                ThreadCount = threads,
                ElapsedMilliseconds = sw.ElapsedMilliseconds,
                OperationsPerSecond = opsPerSec
            });
        }

        return results;
    }
}