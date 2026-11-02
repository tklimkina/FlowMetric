namespace FlowMetric.Core.Benchmark;

public class BenchmarkResult
{
    public int ThreadCount { get; set; }
    public double ElapsedMilliseconds { get; set; }
    public double OperationsPerSecond { get; set; }
    public double StandardDeviation { get; set; }
    public double CoefficientOfVariation { get; set; }

    public double ConfidenceIntervalMargin { get; set; } 

    public bool GcOccurredDuringMeasurement { get; set; }
    public int ActualIterations { get; set; }
}