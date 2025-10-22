package Ejercicio7;

import java.util.concurrent.Semaphore;

public class EJ7 {

    // Semáforo para asegurar que la carrocería se ensambla antes del motor y batería
    private static Semaphore carroceriaListo = new Semaphore(0);

    public static void main(String[] args) {

        Thread fabricaCarroceria = new Thread(() -> {
            try {
                System.out.println("Fabricando carrocería...");
                Thread.sleep(1000); // Simula tiempo de fabricación
                System.out.println("Carrocería ensamblada.");
                carroceriaListo.release(2); // 1 para motor y 1 para batería
            } catch (InterruptedException e) {
                Thread.currentThread().interrupt();
                System.err.println("Fabricación de carrocería interrumpida");
            }
        });

        Thread fabricaMotor = new Thread(() -> {
            try {
                carroceriaListo.acquire(); // Espera a que la carrocería esté lista
                System.out.println("Fabricando motor...");
                Thread.sleep(500);
                System.out.println("Motor ensamblado.");
            } catch (InterruptedException e) {
                Thread.currentThread().interrupt();
                System.err.println("Fabricación de motor interrumpida");
            }
        });

        Thread fabricaBateria = new Thread(() -> {
            try {
                carroceriaListo.acquire(); // Espera a que la carrocería esté lista
                System.out.println("Fabricando batería...");
                Thread.sleep(500);
                System.out.println("Batería ensamblada.");
            } catch (InterruptedException e) {
                Thread.currentThread().interrupt();
                System.err.println("Fabricación de batería interrumpida");
            }
        });

        // Iniciamos todos los hilos
        fabricaCarroceria.start();
        fabricaMotor.start();
        fabricaBateria.start();

        try {
            fabricaCarroceria.join();
            fabricaMotor.join();
            fabricaBateria.join();
        } catch (InterruptedException e) {
            Thread.currentThread().interrupt();
            System.err.println("Espera de hilos interrumpida");
        }

        System.out.println("Vehículo ensamblado correctamente.");
    }
}


