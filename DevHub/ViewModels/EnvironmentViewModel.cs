using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevHub.Models;

namespace DevHub.ViewModels
{
    public partial class EnvironmentViewModel : ObservableObject
    {
        public ObservableCollection<EnvironmentAction> Actions { get; } = new();

        public EnvironmentViewModel()
        {
            CargarAccionesPredeterminadas();
        }

        private void CargarAccionesPredeterminadas()
        {
            Actions.Add(new EnvironmentAction
            {
                Title = "Abrir Terminal (PowerShell)",
                Description = "Lanza una nueva ventana de PowerShell en tu ruta de usuario.",
                IconGlyph = "\uE756",
                PathOrCommand = "powershell.exe",
                Arguments = ""
            });

            Actions.Add(new EnvironmentAction
            {
                Title = "Abrir VS Code",
                Description = "Abre Visual Studio Code de manera global.",
                IconGlyph = "\uE74C",
                PathOrCommand = "cmd.exe",
                Arguments = "/c start code"
            });

            Actions.Add(new EnvironmentAction
            {
                Title = "Mis documentos de Desarrollo",
                Description = "Abre el Explorador de Archivos en tu carpeta de proyectos.",
                IconGlyph = "\uE8B7",
                PathOrCommand = "explorer.exe",
                Arguments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            });
        }

        // 🛠️ EL COMANDO AHORA EJECUTA LA LÓGICA DIRECTAMENTE AQUÍ
        [RelayCommand]
        public void ExecuteAction(EnvironmentAction? action)
        {
            if (action is null || string.IsNullOrEmpty(action.PathOrCommand)) return;

            try
            {
                ProcessStartInfo startInfo = new()
                {
                    FileName = action.PathOrCommand,
                    Arguments = action.Arguments,
                    UseShellExecute = true
                };

                Process.Start(startInfo);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"No se pudo iniciar el proceso: {e.Message}");
            }
        }
    }
}