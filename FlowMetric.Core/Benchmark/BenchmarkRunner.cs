using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FlowMetric.Core.Executors;
using FlowMetric.Core.Workloads;

namespace FlowMetric.Core.Benchmark;

public class BenchmarkRunner
{
    private readonly IExecutor _executor;
    private readonly IWorkload _workload;

    // Константы методологии
    private const int WarmupIterations = 32;     // Tier 1 guaranteed
    private const int MeasurementIterations = 20; 
    private int OperationsPerRun;

    public BenchmarkRunner(IExecutor executor, IWorkload workload)
    {
        _executor = executor;
        _workload = workload;
        OperationsPerRun = workload.itemsCount;
    }

    public List<BenchmarkResult> Run(int startThreads, int endThreads, int step)
    {
        var results = new List<BenchmarkResult>();

        for (int threads = startThreads; threads <= endThreads; threads += step)
        {
            // Warm up
            long warmupCount = 0;
            for (int i = 0; i < WarmupIterations; i++)
            {
                warmupCount += _executor.Run(_workload, threads);
            }

            // Measurement
            var measurements = new List<double>();
            bool gcOccurredInAnyIteration = false;

            // GC resource count
            int totalGen0 = 0;
            int totalGen1 = 0;
            int totalGen2 = 0;
            long totalAllocatedBytes = 0;

            for (int i = 0; i < MeasurementIterations; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                int gen0Before = GC.CollectionCount(0);
                int gen1Before = GC.CollectionCount(1);
                int gen2Before = GC.CollectionCount(2);
                long allocatedBefore = GC.GetTotalAllocatedBytes(precise: true);

                var sw = Stopwatch.StartNew();
                long result = _executor.Run(_workload, threads);
                sw.Stop();

                int gen0After = GC.CollectionCount(0);
                int gen1After = GC.CollectionCount(1);
                int gen2After = GC.CollectionCount(2);

                long allocatedAfter = GC.GetTotalAllocatedBytes(precise: true);

                bool gcOccurred = gen0After > gen0Before ||
                                  gen1After > gen1Before ||
                                  gen2After > gen2Before;

                if (gcOccurred) gcOccurredInAnyIteration = true;

                totalGen0 += (gen0After - gen0Before);
                totalGen1 += (gen1After - gen1Before);
                totalGen2 += (gen2After - gen2Before);
                totalAllocatedBytes += (allocatedAfter - allocatedBefore);


                double opsPerSec = OperationsPerRun / sw.Elapsed.TotalSeconds;
                measurements.Add(opsPerSec);

                // to prevent JIT from deleting the result
                warmupCount += result;
            }

            // Statistic calculations
            double median = CalculateMedian(measurements);
            double stdDev = CalculateStandardDeviation(measurements);
            double cov = stdDev / median;

            // 95% Confidence Interval for Student's t distribution
            double tValue = GetTValue(MeasurementIterations);
            double ciMargin = tValue * (stdDev / Math.Sqrt(MeasurementIterations));

            results.Add(new BenchmarkResult
            {
                ThreadCount = threads,
                OperationsPerSecond = median,
                StandardDeviation = stdDev,
                CoefficientOfVariation = cov,
                ConfidenceIntervalMargin = ciMargin,
                GcOccurredDuringMeasurement = gcOccurredInAnyIteration,
                ActualIterations = MeasurementIterations,

                Gen0Collections = totalGen0,
                Gen1Collections = totalGen1,
                Gen2Collections = totalGen2,
                TotalAllocatedBytes = totalAllocatedBytes
            });
        }

        foreach (var r in results)
        {
            Debug.WriteLine($"Threads: {r.ThreadCount} | Allocated: {r.TotalAllocatedBytes / 1024} KB | GC0: {r.Gen0Collections}");
        }

        return results;
    }

    private double CalculateMedian(List<double> values)
    {
        var sorted = values.OrderBy(x => x).ToList();
        int mid = sorted.Count / 2;
        return sorted.Count % 2 == 0 ? (sorted[mid - 1] + sorted[mid]) / 2.0 : sorted[mid];
    }

    private double CalculateStandardDeviation(List<double> values)
    {
        double mean = values.Average();
        double sumOfSquares = values.Sum(val => Math.Pow(val - mean, 2));
        return Math.Sqrt(sumOfSquares / (values.Count - 1));
    }

    // Table for the t-distribution
    private double GetTValue(int n)
    {
        int degreesOfFreedom = n - 1;

        if (degreesOfFreedom >= 30) return 1.960;
        if (degreesOfFreedom == 29) return 2.045;
        if (degreesOfFreedom == 24) return 2.064;
        if (degreesOfFreedom == 19) return 2.093;
        if (degreesOfFreedom == 14) return 2.145;
        if (degreesOfFreedom == 9) return 2.262;

        // Default interpolation
        return 2.145;
    }
}