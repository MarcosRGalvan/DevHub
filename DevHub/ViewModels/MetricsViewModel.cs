using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DevHub.ViewModels
{
    public partial class MetricsViewModel : ObservableObject
    {
        private readonly DispatcherTimer _timer;
        private ManagementObjectSearcher? _cpuSearcher;
        private double _totalRamGb;

        [ObservableProperty]
        private double _cpuUsage;

        [ObservableProperty]
        private string _ramUsageStr = "Calculando...";

        [ObservableProperty]
        private double _ramPercentage;

        public MetricsViewModel()
        {
            InicializarMetricasSistema();

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;
        }

        private void InicializarMetricasSistema()
        {
            try
            {
                // Se inicializa el buscador de WMI para la carga de CPU
                _cpuSearcher = new ManagementObjectSearcher("select LoadPercentage from Win32_Processor");

                // Obtenemos la memoria RAM total física del equipo (en bytes) y la convertimos a GB
                var ramSearcher = new ManagementObjectSearcher("select TotalVisibleMemorySize from Win32_OperatingSystem");
                foreach (var obj in ramSearcher.Get())
                {
                    double totalRamKb = Convert.ToDouble(obj["TotalVisibleMemorySize"]);
                    _totalRamGb = totalRamKb / (1024 * 1024.0);
                    break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al inicializar componentes de hardware: {ex.Message}");
                _totalRamGb = 16.0;
            }
        }

        public void StartMonitoring() => _timer.Start();
        public void StopMonitoring() => _timer.Stop();

        private void Timer_Tick(object? sender, object e)
        {
            double cpuCargaActual = 0;
            try
            {
                if (_cpuSearcher != null)
                {
                    foreach (var obj in _cpuSearcher.Get())
                    {
                        var valorCpu = obj["LoadPercentage"];
                        if (valorCpu != null)
                        {
                            cpuCargaActual = Convert.ToDouble(valorCpu, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        break;
                    }
                }
            }
            catch (Exception ex) 
            {
                Debug.WriteLine($"Fallo al leer CPU: {ex.Message}");
            }
            CpuUsage = cpuCargaActual;

            try
            {
                var osSearcher = new ManagementObjectSearcher("select FreePhysicalMemory from Win32_OperatingSystem");
                foreach (var obj  in osSearcher.Get())
                {
                    double freeRamKb = Convert.ToDouble(obj["FreePhysicalMemory"]);
                    double freeRamGb = freeRamKb / (1024.0 * 1024.0);

                    double usedRamGb = _totalRamGb - freeRamGb;

                    RamPercentage = (usedRamGb / _totalRamGb) * 100;

                    RamUsageStr = $"{usedRamGb:F1} GB usados de {_totalRamGb:F0} GB";
                    break;
                }
            }
            catch
            {
                RamUsageStr = "Error al leer memoria";
            }
        }
    }
}
