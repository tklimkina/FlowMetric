This document describes the methodology FlowMetric uses to calculate performance precisely. It covers the following aspects:
1. Warmup
2. JIT Optimization Control
3. Statistic Measurement
4. GC Control
### Execution Flow
Before each measurement, BenchmarkRunner first executes a warmup phase to stabilize the JIT compiler. Then, it forces the garbage collector to exclude its interference into the measuring process. After that, BenchmarkRunner measures performance 20 times, calculates the median, and shows it as the final result for that specific configuration.
This entire statistical measurement process is repeated for each thread count, ranging from 1 to 32. As a result, we obtain a reliable median value for every level of concurrency, which is then plotted on the performance chart to clearly visualize how the workload scales.
### Warmup
As the modern JIT uses tiered compilation<sup>[[1]](#ref-1)</sup>, warmup is needed for correct measurements of performance. That's required to evaluate steady-state production behavior instead of the Tier0 fast-compiled version. .NET promotes methods to Tier1 after 30 calls<sup>[[2]](#ref-2)</sup>, so FlowMetric uses 32 warmup iterations (with a margin).
The same practice is used by other benchmark projects. <sup>[[3]](#ref-3)</sup>
### JIT Optimization Control
Even if the returned values are not needed, they must be returned because the JIT compiler optimizes away unused variables (Dead Code Elimination). That's why every workload method must return something to ensure the code is actually executed and not optimized out<sup>[[4]](#ref-4)</sup>.
### Statistic Measurement
Each launch of a benchmark warms the required method up and then calls it 20 more times for every thread count. So, operations per second (OPS) are measured 20 times for all amounts of threads. FlowMetric tests from 1 to 32 threads. Then a median is calculated for every thread count.

The median is used because the arithmetic mean is sensitive to outliers, while the median is robust against them. In the context of one launch measurements are dependent and 20 is a statistically small number. In this case Student's t-distribution is used<sup>[[5]](#ref-5)</sup>. FlowMetric two-sided Student's t-distribution with 95% confidence interval<sup>[[6]](#ref-6)</sup> . This 95% confidence interval is visualized as error bars on the performance chart.
### GC Control
If the garbage collector is triggered while a measurement is performed, it may create noise. Because of this, garbage collection should be forced before every run <sup>[[5]](#ref-5)</sup>.  Nevertheless, one call of the garbage collector is not enough because it activates finalizers, which take some time to complete their work and also leave some garbage after that. For this reason after the call of GC.Collect(), GC.WaitForPendingFinalizers() is to be used, and the next GC should be forced one more time<sup>[[7]](#ref-7)</sup>.

### References
<a id="ref-1"></a>[1] **Tiered Compilation in .NET** — [Microsoft Docs](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-core-3-0#tiered-compilation)
<a id="ref-2"></a>[2] **Tiered Compilation Policy** — [Tiered Compilation](https://github.com/dotnet/runtime/blob/main/docs/design/features/tiered-compilation.md#tiered-compilation-policy-21-rtm)
<a id="ref-3"></a>[3] **JIT Compiler Warmup** — [BenchmarkDotNet: How It Works](https://benchmarkdotnet.org/articles/guides/how-it-works.html)
<a id="ref-4"></a>[4] **Java Performance: The Definitive Guide** — Scott Oaks. O'Reilly Media, 2014. ISBN: 978-1-449-35845-7, Microbenchmarks (p. 11).
<a id="ref-5"></a>[5] **Statistically Rigorous Java Performance Evaluation** — Andy Georges, Dries Buytaert, Lieven Eeckhout. ACM SIGPLAN Notices (OOPSLA '07), 2007. DOI: [10.1145/1297105.1297033](https://doi.org/10.1145/1297105.1297033)
<a id="ref-6"></a>[6] **Student's t-distribution** — [Wikipedia](https://en.wikipedia.org/wiki/Student%27s_t-distribution)
<a id="ref-7"></a>[7] **Pro .NET Memory Management** — Konrad Kokosa. Apress, 2018. ISBN: 978-1-4842-4026-7, Finalization Internals, p. 757. [Official Book Website](https://prodotnetmemory.com/)
