using FlowMetric.Core.Workloads;
using System.IO;
using System.Reflection;

namespace FlowMetric.Core.Plugins;

public static class PluginLoader
{
    public static List<IWorkload> LoadPlugins(string pluginsFolder)
    {
        var loadedWorkloads = new List<IWorkload>();

        if (!Directory.Exists(pluginsFolder))
        {
            Directory.CreateDirectory(pluginsFolder);
            return loadedWorkloads;
        }

        var dllFiles = Directory.GetFiles(pluginsFolder, "*.dll", SearchOption.TopDirectoryOnly);

        foreach (var dllPath in dllFiles)
        {
            try
            {
                var loadContext = new PluginLoadContext(dllPath);
                var assembly = loadContext.LoadFromAssemblyPath(Path.GetFullPath(dllPath));

                var workloadTypes = assembly.GetTypes()
                    .Where(t => typeof(IWorkload).IsAssignableFrom(t)
                                && !t.IsAbstract
                                && !t.IsInterface);

                foreach (var type in workloadTypes)
                {
                    try
                    {
                        if (Activator.CreateInstance(type) is IWorkload workload)
                        {
                            loadedWorkloads.Add(workload);
                            Console.WriteLine($"[Plugin] Loaded: {workload.Name} from {Path.GetFileName(dllPath)}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Plugin] Failed to instantiate {type.Name}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Plugin] Failed to load {Path.GetFileName(dllPath)}: {ex.Message}");
            }
        }

        return loadedWorkloads;
    }
}