using FlowMetric.Core.Benchmark;
using System;
using System.Collections.Generic;

namespace FlowMetric.Core.Info;

public class BenchmarkReport
{
    public EnvironmentMetadata Metadata { get; set; } = new();
    public BenchmarkConfiguration Configuration { get; set; } = new();
    public List<BenchmarkResult> Results { get; set; } = new();
}

public class EnvironmentMetadata
{
    public string Timestamp { get; set; } = DateTime.UtcNow.ToString("o");
    public string DotNetVersion { get; set; } = Environment.Version.ToString();
    public string OS { get; set; } = Environment.OSVersion.ToString();
    public string Processor { get; set; } = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER") ?? "Unknown";
    public string Architecture { get; set; } = Environment.Is64BitProcess ? "X64" : "X86";
    public long TotalPhysicalMemoryGB { get; set; } = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / (1024 * 1024 * 1024);
}

public class BenchmarkConfiguration
{
    public string WorkloadName { get; set; } = string.Empty;
    public string ExecutorName { get; set; } = string.Empty;
    public int WarmupIterations { get; set; } = 32;
    public int MeasurementIterations { get; set; } = 20;
    public int OperationsPerRun { get; set; } = 0;
}