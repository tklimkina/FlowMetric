using ConcurrencyExplorer.Core.Workloads;
using FlowMetric.Core.Benchmark;
using FlowMetric.Core.Executors;
using FlowMetric.Core.Info;
using FlowMetric.Core.Plugins;
using FlowMetric.Core.Workloads;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace FlowMetric
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MainWindow : Window
    {
        public System.Collections.ObjectModel.ObservableCollection<BenchmarkResult> ResultsCollection { get; set; } = new();
        private int _opr = 0;

        private readonly List<IWorkload> _workloads = new()
    {
        new GuidWorkload(),
        new ContentionWorkload(),
        new InterlockedWorkload(),
        new Sha256Workload(),
        new FalseSharingWorkload(),
        new ReaderWriterLockSlimWorkload(),
        new ConcurrentDictionaryWorkload(),
        new FileIOWorkload(),
        new ConcurrentQueueWorkload(),
        new SpinLockWorkload()
    };
        private readonly List<IExecutor> _executors = new()
    {
        new ParallelExecutor(),
        new ChannelExecutor(),
        new TaskExecutor(),
        new DataflowExecutor(),
        new ThreadPoolExecutor(),
        new ParallelLINQExecutor()
    };

        public MainWindow()
        {

            InitializeComponent();

            try
            {
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
                StatusText.Text = "Priority set to High";
            }
            catch (Exception)
            {
                StatusText.Text = "Running with Normal Priority (Run as Admin for High)";
            }

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
            _opr = workload.itemsCount;

            var results = await Task.Run(() =>
            {
                var runner = new BenchmarkRunner(executor, workload);

                return runner.Run(1, 32, 1);
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

        private void OnExportJson(object sender, RoutedEventArgs e)
        {
            if (ResultsCollection == null || ResultsCollection.Count == 0)
            {
                MessageBox.Show("Сначала запустите бенчмарк, чтобы были данные для экспорта.", "Нет данных", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var currentWorkload = (string)WorkloadSelector.SelectedItem;
            var currentExecutor = (string)ExecutorSelector.SelectedItem;

            var report = new BenchmarkReport
            {
                Configuration = new BenchmarkConfiguration
                {
                    WorkloadName = currentWorkload,
                    ExecutorName = currentExecutor,
                    WarmupIterations = 32,
                    MeasurementIterations = 20,
                    OperationsPerRun = _opr
                },
                Results = new System.Collections.Generic.List<BenchmarkResult>(ResultsCollection)
            };

            // Filename includes types of a workload and of an executor
            string safeWorkloadName = currentWorkload.Replace(" ", "_").Replace("(", "").Replace(")", "");
            string safeExecutorName = currentExecutor.Replace(" ", "_").Replace("(", "").Replace(")", "");
            string fileName = $"FlowMetric_Result_{safeWorkloadName}_{safeExecutorName}_{DateTime.Now:yyyyMMdd_HHmmss}.json";

            string userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string fullPath = Path.Combine(userPath, "Downloads", fileName);

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
                BenchmarkExport.SaveToJson(report, fullPath);

                StatusText.Text = $"Exported: {fileName}";
                MessageBox.Show($"Results are exported into {fullPath}", "Exported", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"While exporting, the error has occurred:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}