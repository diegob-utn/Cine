using System;
using System.Net.Http;

namespace Cine.ApiTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var httpClient = new HttpClient();
            //httpClient.BaseAddress = new Uri("https://cine-g99w.onrender.com");
            httpClient.BaseAddress = new Uri("https://localhost:7269/");

            // 1. Crear las bases (Independientes)
            new SalasApiTest(httpClient).RunAllTests();
            Console.WriteLine("\n\n");
            new PeliculasApiTest(httpClient).RunAllTests();
            Console.WriteLine("\n\n");


            // 2. Crear la relación intermedia (Depende de 1)
            new FuncionesApiTest(httpClient).RunAllTests();
            Console.WriteLine("\n\n");

            // 3. Crear el producto final (Depende de 2)
            new BoletosApiTest(httpClient).RunAllTests();
            Console.WriteLine("\n\n");

            Console.WriteLine("Pruebas finalizadas.");
            Console.ReadLine();
        }
    }
}
