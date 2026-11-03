using ConcurrencyExplorer.Core.Workloads;
using FlowMetric.Core.Benchmark;
using FlowMetric.Core.Executors;
using FlowMetric.Core.Workloads;
using System.CodeDom.Compiler;
using System.Linq;
using System.Text;
using System.Windows;

namespace FlowMetric
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<IWorkload> _workloads = new()
    {
        new GuidWorkload(),
        new ContentionWorkload(),
        new InterlockedWorkload(),
        new Sha256Workload()
    };
        private readonly List<IExecutor> _executors = new()
    {
        new ParallelExecutor(),
        new ChannelExecutor(),
        new TaskExecutor()
    };
        public MainWindow()
        {

            InitializeComponent();
            WorkloadSelector.ItemsSource = _workloads.Select(w => w.Name);
            WorkloadSelector.SelectedIndex = 0;

            ExecutorSelector.ItemsSource = _executors.Select(w => w.Name); ;
            ExecutorSelector.SelectedIndex = 0;
        }
        private async void OnStartBenchmark(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "Measuring...";

            var wSelectedName = (string)WorkloadSelector.SelectedItem;
            var eSelectedName = (string)ExecutorSelector.SelectedItem;
            var workload = _workloads.First(w => w.Name == wSelectedName);
            var executor = _executors.First(w => w.Name == eSelectedName);

            var results = await Task.Run(() =>
            {
                var runner = new BenchmarkRunner(executor, workload);

                return runner.Run(1, 16, 1);
            });

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

            var selectedWorkloadName = (string)WorkloadSelector.SelectedItem;
            flowPlot.Plot.Title($"Workload: {selectedWorkloadName}");

            flowPlot.Plot.Axes.AutoScale();
            flowPlot.Refresh();

            StatusText.Text = "Completed";
        }
    }
}