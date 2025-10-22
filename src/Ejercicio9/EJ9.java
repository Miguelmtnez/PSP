package Ejercicio9;

import java.util.*;
import java.util.concurrent.*;

public class EJ9 {

    // Clase que representa una partida de lanzamiento de dados
    static class Partida implements Runnable {
        private int numDados;
        private Map<Integer, Integer> resultadoObjetivo;

        public Partida(int numDados, Map<Integer, Integer> resultadoObjetivo) {
            this.numDados = numDados;
            this.resultadoObjetivo = resultadoObjetivo;
        }

        @Override
        public void run() {
            Random rand = new Random();
            int intentos = 0;

            while (true) {
                intentos++;
                // Lanzar los dados
                Map<Integer, Integer> tirada = new HashMap<>();
                for (int i = 0; i < numDados; i++) {
                    int dado = rand.nextInt(6) + 1; // [1,6]
                    tirada.put(dado, tirada.getOrDefault(dado, 0) + 1);
                }

                // Comprobar si coincide con el objetivo
                if (tirada.equals(resultadoObjetivo)) {
                    System.out.println(Thread.currentThread().getName() +
                            " → ¡Partida terminada! Resultado obtenido tras " + intentos + " lanzamientos: " + tirada);
                    break;
                }

                try {
                    Thread.sleep(100); // Espera 100 ms entre lanzamientos
                } catch (InterruptedException e) {
                    Thread.currentThread().interrupt();
                    break;
                }
            }
        }
    }

    public static void main(String[] args) {
        ExecutorService pool = Executors.newFixedThreadPool(2); // Thread pool fijo de tamaño 2

        try (Scanner sc = new Scanner(System.in)) {
            while (true) {
                System.out.print("\nIntroduce el número de dados (0 para salir): ");
                int numDados = sc.nextInt();
                if (numDados == 0) break;

                // Leer los resultados objetivo
                Map<Integer, Integer> objetivo = new HashMap<>();
                System.out.println("Introduce los resultados deseados (ejemplo: para un 4 y dos 6, escribe '4 6 6'):");
                sc.nextLine(); // limpiar buffer
                String linea = sc.nextLine();
                String[] partes = linea.split("\\s+");
                for (String s : partes) {
                    int valor = Integer.parseInt(s);
                    objetivo.put(valor, objetivo.getOrDefault(valor, 0) + 1);
                }

                // Crear y lanzar la partida
                Partida p = new Partida(numDados, objetivo);
                pool.execute(p);
            }
        }

        pool.shutdown();
        System.out.println("Esperando a que terminen las partidas...");
        try {
            if (!pool.awaitTermination(60, TimeUnit.SECONDS)) {
                pool.shutdownNow();
                if (!pool.awaitTermination(60, TimeUnit.SECONDS)) {
                    System.err.println("Pool no terminó correctamente");
                }
            }
        } catch (InterruptedException e) {
            pool.shutdownNow();
            Thread.currentThread().interrupt();
        }
    }
}

