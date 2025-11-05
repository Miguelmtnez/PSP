using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ejercicio12
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
        private Label colorLabel;
        private Button startButton;
        private bool isRunning = false;
        private CancellationTokenSource? cancellationTokenSource;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Configuración de la ventana
            this.Title = "Ejercicio 12 - Cambio de Colores";
            this.Width = 400;
            this.Height = 300;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Crear el contenedor principal
            var stackPanel = new StackPanel
            {
                Margin = new Thickness(20),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // Crear el label
            colorLabel = new Label
            {
                Content = "Este label cambiará de color",
                Background = Brushes.LightGray,
                Foreground = Brushes.Black,
                FontSize = 16,
                Padding = new Thickness(20),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };

            // Crear el botón
            startButton = new Button
            {
                Content = "Iniciar Cambio de Colores",
                FontSize = 14,
                Padding = new Thickness(20, 10, 20, 10),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // Asignar el evento del botón
            startButton.Click += StartButton_Click;

            // Agregar controles al contenedor
            stackPanel.Children.Add(colorLabel);
            stackPanel.Children.Add(startButton);

            // Establecer el contenido de la ventana
            this.Content = stackPanel;
        }

        // VERSIÓN PROBLEMÁTICA (comentada):
        // Este método causaría bloqueo porque usa Thread.Sleep() en el hilo de la UI
        /*
        private void StartButton_Click_Problematica(object sender, RoutedEventArgs e)
        {
            // PROBLEMA: Este código bloquea la UI porque usa Thread.Sleep en el hilo principal
            startButton.IsEnabled = false;
            
            for (int i = 0; i < 10; i++) // Cambiar colores 10 veces
            {
                if (i % 2 == 0)
                {
                    colorLabel.Background = Brushes.Gray;
                }
                else
                {
                    colorLabel.Background = Brushes.Red;
                }
                
                Thread.Sleep(2000); // ESTO BLOQUEA LA UI
            }
            
            startButton.IsEnabled = true;
        }
        */

        // VERSIÓN CORREGIDA usando async/await:
        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning)
            {
                // Si ya está ejecutándose, detener
                cancellationTokenSource?.Cancel();
                return;
            }

            isRunning = true;
            startButton.Content = "Detener Cambio de Colores";
            cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await CambiarColoresAsync(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                // La operación fue cancelada por el usuario
                colorLabel.Background = Brushes.LightGray;
            }
            finally
            {
                isRunning = false;
                startButton.Content = "Iniciar Cambio de Colores";
                cancellationTokenSource?.Dispose();
                cancellationTokenSource = null;
            }
        }

        private async Task CambiarColoresAsync(CancellationToken cancellationToken)
        {
            bool esGris = true;

            // Cambiar colores durante 20 segundos (10 cambios de 2 segundos cada uno)
            for (int i = 0; i < 10; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Cambiar el color en el hilo de la UI
                await Dispatcher.InvokeAsync(() =>
                {
                    if (esGris)
                    {
                        colorLabel.Background = Brushes.Gray;
                        colorLabel.Content = $"Color: Gris (Cambio {i + 1}/10)";
                    }
                    else
                    {
                        colorLabel.Background = Brushes.Red;
                        colorLabel.Content = $"Color: Rojo (Cambio {i + 1}/10)";
                    }
                });

                esGris = !esGris; // Alternar color

                // Esperar 2 segundos de forma asíncrona (no bloquea la UI)
                await Task.Delay(2000, cancellationToken);
            }

            // Restaurar el estado inicial
            await Dispatcher.InvokeAsync(() =>
            {
                colorLabel.Background = Brushes.LightGray;
                colorLabel.Content = "Cambio de colores completado";
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
