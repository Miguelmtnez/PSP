using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ejercicio13
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
        private Button downloadButton;
        private TextBlock statusLabel;
        private ProgressBar progressBar;
        private readonly HttpClient httpClient;
        private CancellationTokenSource? cancellationTokenSource;

        public MainWindow()
        {
            httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromMinutes(5); // Timeout de 5 minutos
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Configuración de la ventana
            this.Title = "Ejercicio 13 - Descarga de Archivos";
            this.Width = 500;
            this.Height = 300;
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
                Content = "Descargador de Archivos",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };

            // Label de estado
            statusLabel = new TextBlock
            {
                Text = "Listo para descargar",
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10),
                TextWrapping = TextWrapping.Wrap
            };

            // Barra de progreso
            progressBar = new ProgressBar
            {
                Height = 20,
                Margin = new Thickness(0, 0, 0, 20),
                Visibility = Visibility.Hidden
            };

            // Botón de descarga
            downloadButton = new Button
            {
                Content = "Descargar Archivo",
                FontSize = 14,
                Padding = new Thickness(20, 10, 20, 10),
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = Brushes.LightBlue
            };

            // Asignar el evento del botón
            downloadButton.Click += DownloadButton_Click;

            // Agregar controles al contenedor
            stackPanel.Children.Add(titleLabel);
            stackPanel.Children.Add(statusLabel);
            stackPanel.Children.Add(progressBar);
            stackPanel.Children.Add(downloadButton);

            // Establecer el contenido de la ventana
            this.Content = stackPanel;
        }

        // VERSIÓN PROBLEMÁTICA (comentada):
        // Este método causaría bloqueo porque usa operaciones síncronas en el hilo de la UI
        /*
        private void DownloadButton_Click_Problematica(object sender, RoutedEventArgs e)
        {
            // PROBLEMA: Este código bloquea la UI porque usa operaciones síncronas
            downloadButton.IsEnabled = false;
            statusLabel.Text = "Descargando...";
            
            try
            {
                using (var client = new WebClient())
                {
                    // ESTO BLOQUEA LA UI - operación síncrona
                    client.DownloadFile("https://httpbin.org/delay/5", "archivo_descargado.txt");
                }
                
                statusLabel.Text = "Descarga completada";
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Error: {ex.Message}";
            }
            finally
            {
                downloadButton.IsEnabled = true;
            }
        }
        */

        // VERSIÓN CORREGIDA usando async/await:
        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (cancellationTokenSource != null)
            {
                // Si hay una descarga en progreso, cancelarla
                cancellationTokenSource.Cancel();
                return;
            }

            cancellationTokenSource = new CancellationTokenSource();
            
            try
            {
                await DescargarArchivoAsync(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                statusLabel.Text = "Descarga cancelada por el usuario";
                statusLabel.Foreground = Brushes.Orange;
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Error en la descarga: {ex.Message}";
                statusLabel.Foreground = Brushes.Red;
            }
            finally
            {
                cancellationTokenSource?.Dispose();
                cancellationTokenSource = null;
                downloadButton.Content = "Descargar Archivo";
                downloadButton.Background = Brushes.LightBlue;
                progressBar.Visibility = Visibility.Hidden;
            }
        }

        private async Task DescargarArchivoAsync(CancellationToken cancellationToken)
        {
            // Configurar UI para descarga
            downloadButton.Content = "Cancelar Descarga";
            downloadButton.Background = Brushes.LightCoral;
            progressBar.Visibility = Visibility.Visible;
            progressBar.IsIndeterminate = true;
            statusLabel.Text = "Iniciando descarga...";
            statusLabel.Foreground = Brushes.Black;

            // URL de ejemplo que simula una descarga lenta
            string url = "https://httpbin.org/delay/3"; // Simula 3 segundos de retraso
            string fileName = "archivo_descargado.txt";
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);

            try
            {
                statusLabel.Text = $"Descargando desde: {url}";

                // Realizar la descarga asíncrona
                using (var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();

                    var totalBytes = response.Content.Headers.ContentLength ?? 0;
                    progressBar.IsIndeterminate = totalBytes == 0;
                    progressBar.Maximum = totalBytes;
                    progressBar.Value = 0;

                    using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken))
                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        var buffer = new byte[8192];
                        long totalBytesRead = 0;
                        int bytesRead;

                        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                            totalBytesRead += bytesRead;

                            // Actualizar progreso en el hilo de la UI
                            await Dispatcher.InvokeAsync(() =>
                            {
                                if (totalBytes > 0)
                                {
                                    progressBar.Value = totalBytesRead;
                                    double percentage = (double)totalBytesRead / totalBytes * 100;
                                    statusLabel.Text = $"Descargando... {percentage:F1}% ({totalBytesRead:N0} / {totalBytes:N0} bytes)";
                                }
                                else
                                {
                                    statusLabel.Text = $"Descargando... {totalBytesRead:N0} bytes";
                                }
                            });
                        }
                    }
                }

                // Descarga completada exitosamente
                await Dispatcher.InvokeAsync(() =>
                {
                    statusLabel.Text = $"✅ Descarga completada: {filePath}";
                    statusLabel.Foreground = Brushes.Green;
                    progressBar.Value = progressBar.Maximum;
                });

                // Simular un poco más de procesamiento
                await Task.Delay(1000, cancellationToken);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error de red: {ex.Message}");
            }
            catch (TaskCanceledException ex) when (ex.CancellationToken.IsCancellationRequested)
            {
                // Limpiar archivo parcial si existe
                if (File.Exists(filePath))
                {
                    try { File.Delete(filePath); } catch { }
                }
                throw new OperationCanceledException("Descarga cancelada");
            }
            catch (Exception ex)
            {
                // Limpiar archivo parcial si existe
                if (File.Exists(filePath))
                {
                    try { File.Delete(filePath); } catch { }
                }
                throw new Exception($"Error inesperado: {ex.Message}");
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            httpClient?.Dispose();
            base.OnClosed(e);
        }
    }
}
