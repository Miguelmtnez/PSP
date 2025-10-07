package Ejercicio4;

public class EJ4 {
    private static class Contador implements Runnable {
        private volatile boolean detenido = false;
        private volatile int valorActual = 0;

        public void detener() {
            detenido = true;
        }

        public int getValorActual() {
            return valorActual;
        }

        @Override
        public void run() {
            while (!detenido) {
                valorActual++;
                if (valorActual <= 5) {
                    System.out.println(valorActual);
                }
                try {
                    Thread.sleep(1000);
                } catch (InterruptedException e) {
                    Thread.currentThread().interrupt();
                    return;
                }
            }
        }
    }

    public static void main(String[] args) {
        java.util.Scanner scanner = new java.util.Scanner(System.in);
        java.util.Random random = new java.util.Random();

        while (true) {
            int objetivo = 10 + random.nextInt(11); // 10..20
            System.out.println("Pulsa enter cuando creas que el contador ha llegado a " + objetivo);

            Contador contador = new Contador();
            Thread hilo = new Thread(contador);
            hilo.start();

            // Esperar a que el usuario pulse Enter
            scanner.nextLine();

            // Detener el contador y esperar a que finalice
            contador.detener();
            try {
                hilo.join();
            } catch (InterruptedException e) {
                Thread.currentThread().interrupt();
                break;
            }

            int valorDetenido = contador.getValorActual();
            if (valorDetenido == objetivo) {
                System.out.println("¡Lo has conseguido!");
                break;
            } else {
                System.out.println("Vuelve a intentarlo, has detenido el contador en " + valorDetenido + "…");
            }
        }

        scanner.close();
    }
}
