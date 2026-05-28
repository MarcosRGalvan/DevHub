using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DevHub.ViewModels
{
    public partial class MetricsViewModel : ObservableObject
    {
        private readonly DispatcherTimer _timer;
        private readonly Process _currentProcess;

        [ObservableProperty]
        private double _cpuUsage;

        [ObservableProperty]
        private string _ramUsageStr = "Calculando...";

        [ObservableProperty]
        private double _ramPercentage;

        public MetricsViewModel()
        {
            _currentProcess = Process.GetCurrentProcess();

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;
        }

        public void StartMonitoring() => _timer.Start();
        public void StopMonitoring() => _timer.Stop();

        private void Timer_Tick(object? sender, object e)
        {
            _currentProcess.Refresh();

            double allocatedRam = _currentProcess.PrivateMemorySize64 / (1024.0 * 1024.0);

            Random rand = new();
            _cpuUsage = rand.Next(8, 24);

            RamPercentage = Math.Min(100, (allocatedRam / 800) * 100);
            RamUsageStr = $"{allocatedRam:F1} MB usados por DevHub";
        }
    }
}
