package Ejercicio3;
import java.util.Scanner;
import java.util.Random;

class HiloPrimos implements Runnable {
    private String nombreHilo;
    private Random random = new Random();

    public HiloPrimos(String nombreHilo) {
        this.nombreHilo = nombreHilo;
    }

    private boolean esPrimo(int num) {
        if (num <= 1) return false;
        for (int i = 2; i <= Math.sqrt(num); i++) {
            if (num % i == 0) return false;
        }
        return true;
    }

    @Override
    public void run() {
        int limite = random.nextInt(100) + 1;
        System.out.println(nombreHilo + ": Mostrando primos hasta el " + limite);

        for (int i = 2; i <= limite; i++) {
            if (esPrimo(i)) {
                System.out.println(nombreHilo + ": " + i);
                try {
                    Thread.sleep(random.nextInt(501) + 500); // 500–1000 ms
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
            }
        }
    }
}

public class EJ3 {
    public static void main(String[] args) {
        Scanner sc = new Scanner(System.in);
        System.out.print("Introduce el número de hilos a crear: ");
        int n = sc.nextInt();

        Thread[] hilos = new Thread[n];

        // Crear y lanzar hilos
        for (int i = 0; i < n; i++) {
            String nombre = "Hilo " + (i + 1);
            hilos[i] = new Thread(new HiloPrimos(nombre), nombre);
            hilos[i].start();
        }

        sc.close();

        // Monitorear hilos cada segundo
        boolean todosTerminados;
        do {
            todosTerminados = true;
            for (Thread hilo : hilos) {
                System.out.println(hilo.getId() + " " + hilo.getName() + " " + hilo.getState());
                if (hilo.getState() != Thread.State.TERMINATED) {
                    todosTerminados = false;
                }
            }
            try {
                Thread.sleep(1000); // esperar 1 segundo
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        } while (!todosTerminados);
    }
}
