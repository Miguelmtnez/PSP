using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ejercicio14
{
    public partial class App : Application
    {
        [STAThread]
        public static void Main()
        {
            App app = new App();
            app.InitializeComponent();
            app.Run();
        }

        public void InitializeComponent()
        {
            var window = new MainWindow();
            this.MainWindow = window;
            window.Show();
        }
    }

    public class MainWindow : Window
    {
        private Button leerArchivoButton;
        private ProgressBar progressBar;
        private TextBlock statusLabel;
        private TextBlock fileInfoLabel;
        private string archivoSeleccionado = "";
        private CancellationTokenSource? cancellationTokenSource;

        public MainWindow()
        {
            InitializeComponent();
            CrearArchivoDeEjemplo();
        }

        private void InitializeComponent()
        {
            // Configuración de la ventana
            this.Title = "Ejercicio 14 - Lectura de Archivos con Progreso";
            this.Width = 600;
            this.Height = 400;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Crear el contenedor principal
            var stackPanel = new StackPanel
            {
                Margin = new Thickness(20),
                VerticalAlignment = VerticalAlignment.Center
            };

            // Título
            var titleLabel = new Label
            {
                Content = "Lector de Archivos con Barra de Progreso",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };

            // Información del archivo
            fileInfoLabel = new TextBlock
            {
                Text = "Preparando archivo de ejemplo...",
                FontSize = 12,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10),
                TextWrapping = TextWrapping.Wrap,
                Foreground = Brushes.DarkBlue
            };

            // Label de estado
            statusLabel = new TextBlock
            {
                Text = "Listo para leer archivo",
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20),
                TextWrapping = TextWrapping.Wrap
            };

            // Barra de progreso
            progressBar = new ProgressBar
            {
                Height = 25,
                Margin = new Thickness(0, 0, 0, 20),
                Minimum = 0,
                Maximum = 100,
                Value = 0
            };

            // Botón para leer archivo
            leerArchivoButton = new Button
            {
                Content = "Leer Archivo",
                FontSize = 14,
                Padding = new Thickness(20, 10, 20, 10),
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = Brushes.LightGreen
            };

            // Asignar el evento del botón
            leerArchivoButton.Click += LeerArchivoButton_Click;

            // Agregar controles al contenedor
            stackPanel.Children.Add(titleLabel);
            stackPanel.Children.Add(fileInfoLabel);
            stackPanel.Children.Add(statusLabel);
            stackPanel.Children.Add(progressBar);
            stackPanel.Children.Add(leerArchivoButton);

            // Establecer el contenido de la ventana
            this.Content = stackPanel;
        }

        private void CrearArchivoDeEjemplo()
        {
            Task.Run(() =>
            {
                try
                {
                    string rutaArchivo = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "archivo_grande_ejemplo.txt");
                    
                    if (!File.Exists(rutaArchivo))
                    {
                        // Crear un archivo de ejemplo de aproximadamente 50MB
                        using (var writer = new StreamWriter(rutaArchivo))
                        {
                            var linea = "Esta es una línea de ejemplo para crear un archivo de gran tamaño. " +
                                       "Contiene suficiente texto para hacer que el archivo sea lo suficientemente grande " +
                                       "para demostrar el progreso de lectura. Línea número: ";
                            
                            for (int i = 0; i < 500000; i++) // 500,000 líneas
                            {
                                writer.WriteLine($"{linea}{i}");
                            }
                        }
                    }

                    var fileInfo = new FileInfo(rutaArchivo);
                    archivoSeleccionado = rutaArchivo;

                    Dispatcher.Invoke(() =>
                    {
                        fileInfoLabel.Text = $"Archivo: {Path.GetFileName(rutaArchivo)}\nTamaño: {fileInfo.Length:N0} bytes ({fileInfo.Length / (1024 * 1024):F1} MB)";
                        leerArchivoButton.IsEnabled = true;
                    });
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        fileInfoLabel.Text = $"Error al crear archivo de ejemplo: {ex.Message}";
                        fileInfoLabel.Foreground = Brushes.Red;
                    });
                }
            });
        }

        // VERSIÓN PROBLEMÁTICA (comentada):
        // Este método causaría bloqueo porque lee el archivo síncronamente
        /*
        private void LeerArchivoButton_Click_Problematica(object sender, RoutedEventArgs e)
        {
            // PROBLEMA: Este código bloquea la UI porque lee el archivo síncronamente
            leerArchivoButton.IsEnabled = false;
            statusLabel.Text = "Leyendo archivo...";
            progressBar.Value = 0;
            
            try
            {
                LeerArchivo(archivoSeleccionado); // ESTO BLOQUEA LA UI
                statusLabel.Text = "Lectura completada";
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Error: {ex.Message}";
            }
            finally
            {
                leerArchivoButton.IsEnabled = true;
            }
        }
        
        private void LeerArchivo(string rutaArchivo)
        {
            // Método problemático que bloquea la UI
            byte[] buffer = new byte[8192];
            long tamañoFichero = new FileInfo(rutaArchivo).Length;
            long leidosTotales = 0;
            
            using (var fileStream = new FileStream(rutaArchivo, FileMode.Open, FileAccess.Read))
            {
                int bytesLeidos;
                while ((bytesLeidos = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    leidosTotales += bytesLeidos;
                    
                    // Actualizar progreso - ESTO TAMBIÉN BLOQUEA
                    int porcentaje = (int)((leidosTotales * 100) / tamañoFichero);
                    progressBar.Value = porcentaje;
                    
                    // Simular procesamiento
                    Thread.Sleep(1); // ESTO BLOQUEA AÚN MÁS
                }
            }
        }
        */

        // VERSIÓN CORREGIDA usando async/await:
        private async void LeerArchivoButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(archivoSeleccionado) || !File.Exists(archivoSeleccionado))
            {
                statusLabel.Text = "No hay archivo disponible para leer";
                statusLabel.Foreground = Brushes.Red;
                return;
            }

            if (cancellationTokenSource != null)
            {
                // Si hay una lectura en progreso, cancelarla
                cancellationTokenSource.Cancel();
                return;
            }

            cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await LeerArchivoAsync(archivoSeleccionado, cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                statusLabel.Text = "Lectura cancelada por el usuario";
                statusLabel.Foreground = Brushes.Orange;
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Error en la lectura: {ex.Message}";
                statusLabel.Foreground = Brushes.Red;
            }
            finally
            {
                cancellationTokenSource?.Dispose();
                cancellationTokenSource = null;
                leerArchivoButton.Content = "Leer Archivo";
                leerArchivoButton.Background = Brushes.LightGreen;
            }
        }

        private async Task LeerArchivoAsync(string rutaArchivo, CancellationToken cancellationToken)
        {
            // Configurar UI para lectura
            leerArchivoButton.Content = "Cancelar Lectura";
            leerArchivoButton.Background = Brushes.LightCoral;
            statusLabel.Text = "Iniciando lectura del archivo...";
            statusLabel.Foreground = Brushes.Black;
            progressBar.Value = 0;

            var fileInfo = new FileInfo(rutaArchivo);
            long tamañoFichero = fileInfo.Length;
            long leidosTotales = 0;
            byte[] buffer = new byte[8192]; // Buffer de 8KB

            using (var fileStream = new FileStream(rutaArchivo, FileMode.Open, FileAccess.Read))
            {
                int bytesLeidos;
                var startTime = DateTime.Now;

                while ((bytesLeidos = await fileStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                {
                    leidosTotales += bytesLeidos;

                    // Callback para actualizar la barra de progreso
                    ActualizarProgreso(leidosTotales, tamañoFichero, startTime);

                    // Simular un pequeño procesamiento (opcional)
                    await Task.Delay(1, cancellationToken);
                }
            }

            // Lectura completada exitosamente
            Dispatcher.Invoke(() =>
            {
                progressBar.Value = 100;
                statusLabel.Text = $"✅ Lectura completada: {leidosTotales:N0} bytes leídos";
                statusLabel.Foreground = Brushes.Green;
            });
        }

        // Callback encargado de actualizar la barra de progreso
        private void ActualizarProgreso(long leidosTotales, long tamañoFichero, DateTime startTime)
        {
            // Calcular porcentaje
            int porcentaje = (int)((leidosTotales * 100) / tamañoFichero);
            
            // Calcular velocidad
            var elapsed = DateTime.Now - startTime;
            double velocidadMBps = elapsed.TotalSeconds > 0 ? (leidosTotales / (1024.0 * 1024.0)) / elapsed.TotalSeconds : 0;

            // Actualizar UI desde el hilo principal usando Dispatcher
            Dispatcher.Invoke(() =>
            {
                progressBar.Value = porcentaje;
                statusLabel.Text = $"Leyendo... {porcentaje}% ({leidosTotales:N0} / {tamañoFichero:N0} bytes) - {velocidadMBps:F1} MB/s";
            });
        }

        protected override void OnClosed(EventArgs e)
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            base.OnClosed(e);
        }
    }
}
