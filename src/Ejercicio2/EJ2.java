package Ejercicio2;
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
        if (num % 2 == 0) return num == 2;
        for (int i = 3; i * i <= num; i += 2) {
            if (num % i == 0) return false;
        }
        return true;
    }

    @Override
    public void run() {
        int limite = random.nextInt(100) + 1; // número aleatorio entre 1 y 100
        System.out.println(nombreHilo + ": Mostrando primos hasta el " + limite);

        for (int i = 2; i <= limite; i++) {
            if (esPrimo(i)) {
                System.out.println(nombreHilo + ": " + i);
                try {
                    Thread.sleep(random.nextInt(501) + 500); // entre 500 y 1000 ms
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
            }
        }
    }
}

public class EJ2 {
    public static void main(String[] args) {
        Scanner sc = new Scanner(System.in);
        System.out.print("Introduce el número de hilos a crear: ");
        int n = sc.nextInt();

        for (int i = 1; i <= n; i++) {
            String nombre = "Hilo " + i;
            Thread hilo = new Thread(new HiloPrimos(nombre));
            hilo.start();
        }
        sc.close();
    }
}

