using ConcurrencyExplorer.Core.Workloads;
using FlowMetric.Core.Benchmark;
using FlowMetric.Core.Executors;
using System.Text;
using System.Windows;
using System.Linq;

namespace FlowMetric
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void OnStartBenchmark(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "Measuring...";

            var workload = new GuidWorkload();
            var executor = new ParallelExecutor();
            var runner = new BenchmarkRunner(executor, workload);

            var results = runner.Run(1, 16, 1);

            flowPlot.Plot.Clear();

            // Plot's axes
            double[] xs = results.Select(r => (double)r.ThreadCount).ToArray();
            double[] ys = results.Select(r => r.OperationsPerSecond).ToArray();

            // Draw a plot
            var scatter = flowPlot.Plot.Add.Scatter(xs, ys);
            scatter.LineWidth = 3;
            scatter.MarkerSize = 10;

            flowPlot.Plot.Axes.Bottom.Label.Text = "Threads count";
            flowPlot.Plot.Axes.Left.Label.Text = "Operations per second (OPS)";
            flowPlot.Plot.Title($"Workload: {workload.Name}");

            flowPlot.Plot.Axes.AutoScale();
            flowPlot.Refresh();

            StatusText.Text = "Completed";
        }
    }
}