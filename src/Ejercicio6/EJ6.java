package Ejercicio6;

import java.io.FileWriter;
import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.ArrayList;
import java.util.List;

public class EJ6 {

	public static void main(String[] args) {
		String ruta = "src/Ejercicio6/log.txt"; // mismo directorio que EJ6.java
		Log log = new Log(ruta);

		int numHilos = 3;
		int mensajesPorHilo = 5;
		List<Thread> hilos = new ArrayList<>();

		for (int i = 1; i <= numHilos; i++) {
			final int idx = i;
			Thread t = new Thread(() -> {
				for (int j = 1; j <= mensajesPorHilo; j++) {
					log.escribir("Mensaje " + j + " del hilo " + idx);
					try {
						Thread.sleep(200);
					} catch (InterruptedException e) {
						Thread.currentThread().interrupt();
						return;
					}
				}
			});
			hilos.add(t);
			t.start();
		}

		for (Thread t : hilos) {
			try {
				t.join();
			} catch (InterruptedException e) {
				Thread.currentThread().interrupt();
				break;
			}
		}

		System.out.println("Escritura concurrente finalizada. Revisa 'src/Ejercicio6/log.txt'.");
	}
}

class Log {

	private String nombreFichero;

	public Log(String nombreFichero) {
		this.nombreFichero = nombreFichero;
	}

	public synchronized void escribir(String mensaje) {
		// Con synchronized, solo un hilo puede ejecutar este método a la vez
		try (FileWriter fw = new FileWriter(nombreFichero, true)) {
			long id = Thread.currentThread().getId();
			String fecha = new SimpleDateFormat("HH:mm:ss yyyy/MM/dd").format(new Date());

			fw.write("ID: " + id + " - " + fecha + "\n");
			fw.write(mensaje + "\n");
			fw.flush();

		} catch (IOException e) {
			e.printStackTrace();
		}
	}
}
