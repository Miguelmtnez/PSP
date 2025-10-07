package Ejercicio5;

public class EJ5 {
	public static void main(String[] args) {
		try (java.util.Scanner scanner = new java.util.Scanner(System.in)) {
			System.out.print("Indica cada cuántos segundos quieres que se guarde el saludo: ");
			int segundos;
			try {
				segundos = Integer.parseInt(scanner.nextLine().trim());
				if (segundos <= 0) {
					System.out.println("La frecuencia debe ser mayor que 0.");
					return;
				}
			} catch (NumberFormatException e) {
				System.out.println("Entrada no válida.");
				return;
			}

		final int periodoMs = segundos * 1000;
		final java.nio.file.Path ruta = java.nio.file.Paths.get("src", "Ejercicio5", "saludos.txt");

		Thread hilo = new Thread(() -> {
			try (java.io.BufferedWriter writer = java.nio.file.Files.newBufferedWriter(
					ruta,
					java.nio.charset.StandardCharsets.UTF_8,
					java.nio.file.StandardOpenOption.CREATE,
					java.nio.file.StandardOpenOption.APPEND)) {
				while (true) {
					writer.write("¡Hola mundo!");
					writer.newLine();
					writer.flush();
					try {
						Thread.sleep(periodoMs);
					} catch (InterruptedException ie) {
						// Restablecer el estado de interrupción y salir del bucle
						Thread.currentThread().interrupt();
						break;
					}
				}
			} catch (java.io.IOException ioe) {
				System.out.println("Error al escribir en el fichero: " + ioe.getMessage());
			} finally {
				System.out.println("Hilo interrumpido");
			}
		});

			hilo.start();
			System.out.print("Pulsa enter para interrumpir el hilo: ");
			scanner.nextLine();
			System.out.println("Interrumpiendo hilo");
			hilo.interrupt();
			try {
				hilo.join();
			} catch (InterruptedException e) {
				Thread.currentThread().interrupt();
			}
			System.out.println("¡Adiós!");
		}
	}
}
