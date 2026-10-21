namespace FlowMetric.Core.Benchmark;

public class BenchmarkResult
{
    public int ThreadCount { get; set; }
    public double ElapsedMilliseconds { get; set; }
    public double OperationsPerSecond { get; set; }
}