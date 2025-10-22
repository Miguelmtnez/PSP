package Ejercicio8;

import java.util.ArrayList;
import java.util.List;
import java.util.Random;

public class EJ8 {
    
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
    
    public static void main(String[] args) {
        String variedad = "Colombia";
        int intensidad = 7;
        
        // Hilo productor
        Thread productor = new Thread(() -> {
            Random rand = new Random();
            while (true) {
                try {
                    // Fabricar cápsula
                    Capsula capsula = new Capsula(variedad, intensidad);
                    
                    synchronized (lock) {
                        contenedor.add(capsula);
                        System.out.println("Hilo Productor: Se ha fabricado una cápsula. Total en contenedor: " + contenedor.size());
                        
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
        
        // Hilo consumidor
        Thread consumidor = new Thread(() -> {
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
                    }
                    
                } catch (InterruptedException e) {
                    Thread.currentThread().interrupt();
                    break;
                }
            }
        });
        
        // Iniciar los hilos
        productor.start();
        consumidor.start();
        
        // Esperar a que terminen (en este caso, el programa correrá indefinidamente)
        try {
            productor.join();
            consumidor.join();
        } catch (InterruptedException e) {
            Thread.currentThread().interrupt();
        }
    }
}
