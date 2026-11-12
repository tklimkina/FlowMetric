using FlowMetric.Core.Info;
using System.IO;
using System.Text.Json;

namespace FlowMetric.Core.Benchmark;

public static class BenchmarkExport
{
    public static void SaveToJson(BenchmarkReport report, string filePath)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        string json = JsonSerializer.Serialize(report, options);
        File.WriteAllText(filePath, json);
    }
}