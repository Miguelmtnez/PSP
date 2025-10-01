package Ejercicio1;
import java.util.Scanner;
import java.util.Random;

class MostrarNumeros implements Runnable {
    private int n1, n2;
    private Random random = new Random();

    public MostrarNumeros(int n1, int n2) {
        this.n1 = n1;
        this.n2 = n2;
    }

    @Override
    public void run() {
        for (int i = n1; i <= n2; i++) {
            System.out.println(i);
            try {
                Thread.sleep(random.nextInt(1000) + 1); // entre 1 y 1000 ms
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }
    }
}

public class EJ1 {
    public static void main(String[] args) {
        Scanner sc = new Scanner(System.in);

        System.out.print("Introduce n1: ");
        int n1 = sc.nextInt();
        System.out.print("Introduce n2: ");
        int n2 = sc.nextInt();

        sc.close();
        
        Thread hilo = new Thread(new MostrarNumeros(n1, n2));
        hilo.start();

        System.out.println("El hilo se ha lanzado");
    }
}
