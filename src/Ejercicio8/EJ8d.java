package Ejercicio8;

import java.util.ArrayList;
import java.util.List;
import java.util.Random;

public class EJ8d {
    
    // Clase que representa una cápsula de café
    static class Capsula {
        private String variedad;
        private int intensidad;
        
        public Capsula(String variedad, int intensidad) {
            this.variedad = variedad;
            this.intensidad = intensidad;
        }
        
        public String getVariedad() {
            return variedad;
        }
        
        public int getIntensidad() {
            return intensidad;
        }
        
        @Override
        public String toString() {
            return "Cápsula " + variedad + " (intensidad " + intensidad + ")";
        }
    }
    
    // Contenedor compartido para las cápsulas
    private static List<Capsula> contenedor = new ArrayList<>();
    private static final Object lock = new Object();
    private static final int CAPACIDAD_MAXIMA = 100;
    
    public static void main(String[] args) {
        String variedad = "Colombia";
        int intensidad = 7;
        
        // Crear 4 hilos productores
        Thread[] productores = new Thread[4];
        for (int i = 0; i < 4; i++) {
            final int idProductor = i + 1;
            productores[i] = new Thread(() -> {
                Random rand = new Random();
                while (true) {
                    try {
                        synchronized (lock) {
                            // Esperar si el contenedor está lleno
                            while (contenedor.size() >= CAPACIDAD_MAXIMA) {
                                System.out.println("Hilo Productor " + idProductor + ": Contenedor lleno (" + contenedor.size() + " cápsulas). Esperando...");
                                lock.wait();
                            }
                            
                            // Fabricar cápsula
                            Capsula capsula = new Capsula(variedad, intensidad);
                            contenedor.add(capsula);
                            System.out.println("Hilo Productor " + idProductor + ": Se ha fabricado una cápsula. Total en contenedor: " + contenedor.size());
                            
                            // Si hay 6 o más cápsulas, notificar al consumidor
                            if (contenedor.size() >= 6) {
                                lock.notify();
                            }
                        }
                        
                        // Espera aleatoria entre 500 y 1000 ms
                        Thread.sleep(500 + rand.nextInt(501));
                        
                    } catch (InterruptedException e) {
                        Thread.currentThread().interrupt();
                        break;
                    }
                }
            });
        }
        
        // Hilo consumidor
        Thread consumidor = new Thread(() -> {
            Random rand = new Random();
            while (true) {
                try {
                    synchronized (lock) {
                        // Esperar hasta que haya al menos 6 cápsulas
                        while (contenedor.size() < 6) {
                            lock.wait();
                        }
                        
                        // Empaquetar 6 cápsulas
                        System.out.println("Hilo Consumidor: Creando caja con 6 cápsulas");
                        contenedor.clear(); // Eliminar las 6 cápsulas del contenedor
                        System.out.println("Hilo Consumidor: Caja creada");
                        
                        // Notificar a los productores que hay espacio disponible
                        lock.notifyAll();
                    }
                    
                    // Espera aleatoria entre 1000 y 3000 ms después de empaquetar
                    Thread.sleep(1000 + rand.nextInt(2001));
                    
                } catch (InterruptedException e) {
                    Thread.currentThread().interrupt();
                    break;
                }
            }
        });
        
        // Iniciar los hilos
        for (Thread productor : productores) {
            productor.start();
        }
        consumidor.start();
        
        // Esperar a que terminen (en este caso, el programa correrá indefinidamente)
        try {
            for (Thread productor : productores) {
                productor.join();
            }
            consumidor.join();
        } catch (InterruptedException e) {
            Thread.currentThread().interrupt();
        }
    }
}
