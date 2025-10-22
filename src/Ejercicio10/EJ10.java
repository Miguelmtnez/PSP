package Ejercicio10;

import java.awt.*;
import java.awt.image.BufferedImage;
import javax.imageio.ImageIO;
import java.io.File;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.Scanner;
import java.util.concurrent.*;

public class EJ10 {

    // --- Clase interna que realiza las capturas ---
    static class Capturadora {
        /**
         * Toma una captura de pantalla completa y la guarda en el directorio indicado.
         * @param directorio Ruta del directorio donde se guardará la imagen.
         */
        public static void capturarPantalla(String directorio) throws Exception {
            Robot robot = new Robot();
            Rectangle pantalla = new Rectangle(Toolkit.getDefaultToolkit().getScreenSize());
            BufferedImage imagen = robot.createScreenCapture(pantalla);

            String timestamp = LocalDateTime.now().format(DateTimeFormatter.ofPattern("yyyyMMdd_HHmmss"));
            File archivo = new File(directorio + File.separator + "captura_" + timestamp + ".png");

            ImageIO.write(imagen, "png", archivo);
        }
    }

    // --- Método principal ---
    public static void main(String[] args) {
        try (Scanner sc = new Scanner(System.in)) {
            System.out.print("Introduce la frecuencia en segundos para las capturas: ");
            int frecuencia = sc.nextInt();
            sc.nextLine(); // limpiar buffer

            System.out.print("Introduce el directorio donde se guardarán las capturas: ");
            String directorio = sc.nextLine();

            // Crear el planificador de tareas (solo 1 hilo)
            ScheduledExecutorService scheduler = Executors.newScheduledThreadPool(1);

            // Definir la tarea periódica
            Runnable tareaCaptura = () -> {
                try {
                    Capturadora.capturarPantalla(directorio);
                    System.out.println("Captura realizada en: " + directorio + " (" + java.time.LocalTime.now() + ")");
                } catch (Exception e) {
                    System.err.println("Error al realizar la captura: " + e.getMessage());
                }
            };

            // Programar la tarea con la frecuencia indicada
            scheduler.scheduleAtFixedRate(tareaCaptura, 0, frecuencia, TimeUnit.SECONDS);

            // Mantener el programa activo hasta que el usuario lo detenga
            System.out.println("Capturas automáticas iniciadas. Pulsa ENTER para detener...");
            sc.nextLine();

            // Detener el programador
            scheduler.shutdown();
            System.out.println("Deteniendo capturas...");
            
            try {
                if (!scheduler.awaitTermination(5, TimeUnit.SECONDS)) {
                    scheduler.shutdownNow();
                    if (!scheduler.awaitTermination(5, TimeUnit.SECONDS)) {
                        System.err.println("No se pudo detener el scheduler correctamente");
                    }
                }
            } catch (InterruptedException e) {
                scheduler.shutdownNow();
                Thread.currentThread().interrupt();
            }
            
            System.out.println("Capturas detenidas. Programa finalizado.");
        }
    }
}
