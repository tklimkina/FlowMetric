using FlowMetric.Core.Executors;
using FlowMetric.Core.Workloads;
using System.Diagnostics;
using System.Diagnostics.Metrics;

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

    public List<BenchmarkResult> Run(int startThreads, int endThreads, int step, int iterations = 5)
    {
        var results = new List<BenchmarkResult>();

        for (int threads = startThreads; threads <= endThreads; threads += step)
        {
            var measurements = new List<double>();

            for (int i = 0; i < iterations; i++)
            {
                // Warmup (JIT has some time to compile the code)
                if (i == 0)
                {
                    _executor.Run(_workload, threads);
                    continue;
                }

                // Clean to avoid outliers
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                var sw = Stopwatch.StartNew();
                _executor.Run(_workload, threads);
                sw.Stop();

                double opsPerSec = 10000 / sw.Elapsed.TotalSeconds;
                measurements.Add(opsPerSec);
            }

            // Calculating median result
            measurements.Sort();
            double median = measurements[measurements.Count / 2];

            results.Add(new BenchmarkResult
            {
                ThreadCount = threads,
                ElapsedMilliseconds = 10000 / median, // Обратная величина для наглядности
                OperationsPerSecond = median
            });
        }

        return results;
    }
}