using System;
using System.Security.Cryptography;

namespace FlowMetric.Core.Workloads;

public class Sha256Workload : IWorkload
{
    private readonly byte[] _data;

    public Sha256Workload()
    {
        var random = new Random(67);            // Fixed seed for repeatability

        _data = new byte[1024];
        random.NextBytes(_data);
    }

    public string Name => "SHA256 Hashing (CPU-Bound)";

    public long Execute()
    {
        byte[] hash = SHA256.HashData(_data);
        return BitConverter.ToInt64(hash, 0);
    }
}