using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

namespace MicroAC.Performance.Tests.Monitor
{
    /// <summary>
    /// A simple tool to print CPU and RAM usage to excel across multiple performance test runs.
    /// Type anything into console to suspend monitoring.
    /// </summary>
    class Program
    {
        static readonly string ResultsFolder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build()
            .GetSection("ResultsFolder")
            .Value;

        static readonly TimeSpan Delay = TimeSpan.FromSeconds(2);

        static PerformanceMonitor PerformanceMonitor = new PerformanceMonitor();

        static async Task Main(string[] args)
        {
            if (string.IsNullOrEmpty(ResultsFolder))
                throw new ArgumentException("appsettings.json must contains 'ResultsFolder' property.");
            
            var resultsFile = GetResultsFilePath();

            await File.WriteAllLinesAsync(resultsFile, new string[] { "Time;CPU;RAM;" });

            while (true)
            {
                await Monitor(resultsFile);
                Console.WriteLine("Pausing execution.");
                resultsFile = GetResultsFilePath();
            }
        }

        static string GetResultsFilePath()
        {
            Console.WriteLine("Enter prefix for new results file:");

            var prefix = Console.ReadLine();
            var filePath = ResultsFolder
                + prefix
                + "_"
                + DateTime.Now.ToString().Replace(':', '.')
                + $"_monitorResults.csv";

            Console.WriteLine($"Results file path: {filePath}");

            return filePath;
        }

        async static Task Monitor(string resultsFile)
        {
            do
            {
                var cpuUsage = PerformanceMonitor.getCpuUsagePercent();
                var ramUsage = PerformanceMonitor.getRamUsagePercent();
                var time = DateTime.Now.ToString();

                Console.WriteLine($"{time} | CPU: {cpuUsage}% | RAM:{ramUsage}% |");
                await File.AppendAllLinesAsync(resultsFile, new string[] { $"{time};{cpuUsage};{ramUsage};" });
            }
            while (await DelayAndCheckForCancel());
        }

        async static Task<bool> DelayAndCheckForCancel()
        {
            var readTask = Task.Run(Console.ReadKey);
            var completedTask = await Task.WhenAny(readTask, Task.Delay(Delay));
            return !object.ReferenceEquals(readTask, completedTask);
        }
    }

    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    class PerformanceMonitor
    {
        PerformanceCounter CpuCounter = new("Processor", "% Processor Time", "_Total", true);

        PerformanceCounter RamCounter = new("Memory", "Available MBytes", true);

        const float TotalMb = 1024 * 24; // 24 GB

        public PerformanceMonitor() { }

        public int getCpuUsagePercent() =>
            (int)Math.Round(CpuCounter.NextValue());

        public double getRamUsagePercent()
             => Math.Round(100 * (TotalMb - RamCounter.NextValue()) / TotalMb);
    }
}
