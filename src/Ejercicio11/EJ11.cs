using System;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Ejercicio11
{
    public class EJ11
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("=== SIMULACIÓN DE COCINADO DE PAELLA ===");
            Console.WriteLine();
            
            // Ejecución secuencial
            Console.WriteLine("--- EJECUCIÓN SECUENCIAL ---");
            var stopwatchSecuencial = Stopwatch.StartNew();
            CocinarPaellaSecuencial();
            stopwatchSecuencial.Stop();
            Console.WriteLine($"Tiempo total secuencial: {stopwatchSecuencial.ElapsedMilliseconds} ms");
            
            Console.WriteLine();
            
            // Ejecución asíncrona
            Console.WriteLine("--- EJECUCIÓN ASÍNCRONA ---");
            var stopwatchAsincrono = Stopwatch.StartNew();
            await CocinarPaellaAsincrona();
            stopwatchAsincrono.Stop();
            Console.WriteLine($"Tiempo total asíncrono: {stopwatchAsincrono.ElapsedMilliseconds} ms");
        }
        
        // Implementación secuencial
        private static void CocinarPaellaSecuencial()
        {
            Console.WriteLine("Iniciando cocinado secuencial...");
            
            // Paso 1: Calentar la paella
            CalentarPaella();
            
            // Paso 2: Sofreír los ingredientes
            SofreirIngredientes();
            
            // Paso 3: Preparar el caldo
            PrepararCaldo();
            
            // Paso 4: Echar el caldo a la paella
            EcharCaldoALaPaella();
            
            // Paso 5: Cocinar la paella
            CocinarPaella();
            
            Console.WriteLine("¡Paella lista! (Secuencial)");
        }
        
        // Implementación asíncrona
        private static async Task CocinarPaellaAsincrona()
        {
            Console.WriteLine("Iniciando cocinado asíncrono...");
            
            // Ejecutar tareas en paralelo:
            // - Calentar la paella
            // - Sofreír los ingredientes  
            // - Preparar el caldo
            Task tareaCalentar = CalentarPaellaAsync();
            Task tareaSofreir = SofreirIngredientesAsync();
            Task tareaCaldo = PrepararCaldoAsync();
            
            // Esperar a que terminen todas las tareas paralelas
            await Task.WhenAll(tareaCalentar, tareaSofreir, tareaCaldo);
            
            Console.WriteLine("Tareas paralelas completadas. Continuando con los pasos finales...");
            
            // Ahora ejecutar secuencialmente los pasos finales
            await EcharCaldoALaPaellaAsync();
            await CocinarPaellaAsync();
            
            Console.WriteLine("¡Paella lista! (Asíncrono)");
        }
        
        // Métodos síncronos
        private static void CalentarPaella()
        {
            Console.WriteLine("🔥 Calentando la paella...");
            System.Threading.Thread.Sleep(3000); // Simula 3 segundos
            Console.WriteLine("✅ Paella calentada");
        }
        
        private static void SofreirIngredientes()
        {
            Console.WriteLine("🥘 Sofriendo los ingredientes...");
            System.Threading.Thread.Sleep(4000); // Simula 4 segundos
            Console.WriteLine("✅ Ingredientes sofritos");
        }
        
        private static void PrepararCaldo()
        {
            Console.WriteLine("🍲 Preparando el caldo...");
            System.Threading.Thread.Sleep(5000); // Simula 5 segundos
            Console.WriteLine("✅ Caldo preparado");
        }
        
        private static void EcharCaldoALaPaella()
        {
            Console.WriteLine("🥄 Echando el caldo a la paella...");
            System.Threading.Thread.Sleep(1000); // Simula 1 segundo
            Console.WriteLine("✅ Caldo echado a la paella");
        }
        
        private static void CocinarPaella()
        {
            Console.WriteLine("👨‍🍳 Cocinando la paella...");
            System.Threading.Thread.Sleep(6000); // Simula 6 segundos
            Console.WriteLine("✅ Paella cocinada");
        }
        
        // Métodos asíncronos
        private static async Task CalentarPaellaAsync()
        {
            Console.WriteLine("🔥 Calentando la paella... (async)");
            await Task.Delay(3000); // Simula 3 segundos de forma asíncrona
            Console.WriteLine("✅ Paella calentada (async)");
        }
        
        private static async Task SofreirIngredientesAsync()
        {
            Console.WriteLine("🥘 Sofriendo los ingredientes... (async)");
            await Task.Delay(4000); // Simula 4 segundos de forma asíncrona
            Console.WriteLine("✅ Ingredientes sofritos (async)");
        }
        
        private static async Task PrepararCaldoAsync()
        {
            Console.WriteLine("🍲 Preparando el caldo... (async)");
            await Task.Delay(5000); // Simula 5 segundos de forma asíncrona
            Console.WriteLine("✅ Caldo preparado (async)");
        }
        
        private static async Task EcharCaldoALaPaellaAsync()
        {
            Console.WriteLine("🥄 Echando el caldo a la paella... (async)");
            await Task.Delay(1000); // Simula 1 segundo de forma asíncrona
            Console.WriteLine("✅ Caldo echado a la paella (async)");
        }
        
        private static async Task CocinarPaellaAsync()
        {
            Console.WriteLine("👨‍🍳 Cocinando la paella... (async)");
            await Task.Delay(6000); // Simula 6 segundos de forma asíncrona
            Console.WriteLine("✅ Paella cocinada (async)");
        }
    }
}