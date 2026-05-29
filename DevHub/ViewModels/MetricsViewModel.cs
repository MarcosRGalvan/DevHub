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
        private ManagementObjectSearcher? _gpuSearcher;
        private double _totalRamGb;

        [ObservableProperty]
        private double _cpuUsage;

        [ObservableProperty]
        private string _ramUsageStr = "Calculando...";

        [ObservableProperty]
        private double _ramPercentage;

        [ObservableProperty]
        private double _gpuIntegratedUsage;

        [ObservableProperty]
        private double _gpuDedicatedUsage;

        public MetricsViewModel()
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
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

                _gpuSearcher = new ManagementObjectSearcher("select Name, UtilizationPercentage from Win32_PerfFormattedData_GPUPerformanceCounters_GPUEngine");

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
            // Lectura del uso de CPU
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

            // Lectura del uso de memoria RAM
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

            // Lectura del uso de las GPU's
            double maxIntegrated = 0;
            double maxDedicated = 0;
            try
            {
                if (_gpuSearcher != null)
                {
                    string? firstLuid = null;
                    string? secondLuid = null;

                    foreach (var obj in _gpuSearcher.Get())
                    {

                        var nameToken = obj["Name"]?.ToString()?.ToLower() ?? "";
                        var valueObj = obj["UtilizationPercentage"];

                        if (valueObj != null && !string.IsNullOrEmpty(nameToken))
                        {
                            double usage = Convert.ToDouble(valueObj);

                            string luidToken = "";
                            int luidIndex = nameToken.IndexOf("luid_");
                            if (luidIndex != -1)
                            {
                                int endIdx = nameToken.IndexOf("_phys", luidIndex);
                                if (endIdx != -1)
                                {
                                    luidToken = nameToken.Substring(luidIndex, endIdx - luidIndex);
                                }
                            }

                            if (!string.IsNullOrEmpty(luidToken))
                            {
                                // Asignación dinámica de las GPU's según el orden de detección de los LUIDs
                                if (firstLuid == null)
                                {
                                    firstLuid = luidToken;
                                }
                                else if (secondLuid == null && luidToken != firstLuid)
                                {
                                    secondLuid = luidToken;
                                }


                                if (nameToken.Contains("3d") || nameToken.Contains("copy"))
                                {
                                    if (luidToken == firstLuid)
                                    {
                                        if (usage > maxIntegrated) maxIntegrated = usage;
                                    }
                                    else if (luidToken == secondLuid)
                                    {
                                        if (usage > maxDedicated) maxDedicated = usage;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error mapeando LUIDs de la GPU: {ex.Message}");
            }

            GpuIntegratedUsage = Math.Clamp(maxIntegrated, 0, 100);
            GpuDedicatedUsage = Math.Clamp(maxDedicated, 0, 100);
        }
    }
}
