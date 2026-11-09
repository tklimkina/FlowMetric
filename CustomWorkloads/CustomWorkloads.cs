using FlowMetric.Core.Workloads;

namespace CustomWorkloads;

// Imitates a long operation
public class SleepWorkload : IWorkload
{
    public string Name => "Custom: Thread.Sleep(1ms)";

    public long Execute()
    {
        Thread.Sleep(1);
        return 1;
    }
}

// Math workload. Checks if number is prime
public class PrimeCheckWorkload : IWorkload
{
    public string Name => "Custom: Prime Check (CPU-Heavy)";

    public long Execute()
    {
        int count = 0;
        for (int i = 2; i < 10000; i++)
        {
            if (IsPrime(i)) count++;
        }
        return count;
    }

    private static bool IsPrime(int n)
    {
        if (n < 2) return false;
        for (int i = 2; i <= Math.Sqrt(n); i++)
        {
            if (n % i == 0) return false;
        }
        return true;
    }
}