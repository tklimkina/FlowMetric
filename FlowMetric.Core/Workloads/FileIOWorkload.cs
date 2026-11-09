using System.IO;

namespace FlowMetric.Core.Workloads;

public class FileIOWorkload : IWorkload
{
    private readonly string _tempDir;
    private readonly byte[] _testData;
    private int _fileCounter;

    public FileIOWorkload()
    {
        // Tests' directory
        _tempDir = Path.Combine(Path.GetTempPath(), "FlowMetric_IO_Test");
        Directory.CreateDirectory(_tempDir);

        // Test data
        _testData = new byte[1024 * 1024];
        new Random(42).NextBytes(_testData);
    }

    public string Name => "File IO (Async Read/Write)";
    public int itemsCount => 200;

    public long Execute()
    {
        string fileName = Path.Combine(_tempDir, $"test_{Interlocked.Increment(ref _fileCounter)}.tmp");

        try
        {
            File.WriteAllBytesAsync(fileName, _testData).Wait();

            byte[] result = File.ReadAllBytesAsync(fileName).Result;

            return result.Length;
        }
        finally
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }
    }
}