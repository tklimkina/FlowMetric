using ConcurrencyExplorer.Core.Workloads;
using FlowMetric.Core.Benchmark;
using FlowMetric.Core.Executors;
using FlowMetric.Core.Plugins;
using FlowMetric.Core.Workloads;
using System.IO;
using System.Windows;

namespace FlowMetric
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public System.Collections.ObjectModel.ObservableCollection<BenchmarkResult> ResultsCollection { get; set; } = new();

        private readonly List<IWorkload> _workloads = new()
    {
        new GuidWorkload(),
        new ContentionWorkload(),
        new InterlockedWorkload(),
        new Sha256Workload(),
        new FalseSharingWorkload(),
        new ReaderWriterLockSlimWorkload(),
        new ConcurrentDictionaryWorkload(),
        new FileIOWorkload()
    };
        private readonly List<IExecutor> _executors = new()
    {
        new ParallelExecutor(),
        new ChannelExecutor(),
        new TaskExecutor(),
        new DataflowExecutor(),
        new ThreadPoolExecutor()
    };

        public MainWindow()
        {

            InitializeComponent();

            string pluginsFolder = Path.Combine(AppContext.BaseDirectory, "Plugins");
            var pluginWorkloads = PluginLoader.LoadPlugins(pluginsFolder);
            _workloads.AddRange(pluginWorkloads);

            WorkloadSelector.ItemsSource = _workloads.Select(w => w.Name);
            WorkloadSelector.SelectedIndex = 0;

            ExecutorSelector.ItemsSource = _executors.Select(w => w.Name); ;
            ExecutorSelector.SelectedIndex = 0;

            ResultsGrid.ItemsSource = ResultsCollection;

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


            // Error range
            double[] yErrors = results.Select(r => r.ConfidenceIntervalMargin).ToArray();
            var errorBars = flowPlot.Plot.Add.ErrorBar(xs, ys, yErrors);
            errorBars.LineWidth = 1;
            errorBars.CapSize = 7;
            errorBars.Color = ScottPlot.Colors.DeepPink;

            // Draw a plot
            var scatter = flowPlot.Plot.Add.Scatter(xs, ys);
            scatter.LineWidth = 3;
            scatter.MarkerSize = 10;
            scatter.Color = ScottPlot.Colors.CornflowerBlue;

            flowPlot.Plot.Axes.Bottom.Label.Text = "Threads count";
            flowPlot.Plot.Axes.Left.Label.Text = "Operations per second (OPS)";

            var selectedWorkloadName = (string)WorkloadSelector.SelectedItem;
            flowPlot.Plot.Title($"Workload: {selectedWorkloadName}");

            flowPlot.Plot.Axes.AutoScale();
            flowPlot.Refresh();

            StatusText.Text = "Completed";

            // GC Metrics
            ResultsCollection.Clear();
            foreach (var result in results)
            {
                ResultsCollection.Add(result);
            }
        }
    }
}