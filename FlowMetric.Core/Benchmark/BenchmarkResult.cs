using System.Text.Json.Serialization;

namespace FlowMetric.Core.Benchmark;

public class BenchmarkResult
{
    // Parallel Metrics
    public int ThreadCount { get; set; }
    public double OperationsPerSecond { get; set; }
    public double StandardDeviation { get; set; }
    public double CoefficientOfVariation { get; set; }

    public double ConfidenceIntervalMargin { get; set; } 

    public bool GcOccurredDuringMeasurement { get; set; }
    public int ActualIterations { get; set; }
    
    // GC Metrics
    public int Gen0Collections { get; set; }
    public int Gen1Collections { get; set; }
    public int Gen2Collections { get; set; }
    public long TotalAllocatedBytes { get; set; }

    [JsonIgnore]
    public string AllocatedKB => $"{TotalAllocatedBytes / 1024:N0} KB";
    [JsonIgnore]
    public string OpsFormatted => $"{OperationsPerSecond:N0}";
    [JsonIgnore]
    public string CIFormatted => $"± {ConfidenceIntervalMargin:N0}";

}