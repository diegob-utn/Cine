using System;
using System.Net.Http;

namespace Cine.ApiTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://cine-g99w.onrender.com");

            // 1. Crear las bases (Independientes)
            new SalasApiTest(httpClient).RunAllTests();
            new PeliculasApiTest(httpClient).RunAllTests();

            // 2. Crear la relación intermedia (Depende de 1)
            new FuncionesApiTest(httpClient).RunAllTests();

            // 3. Crear el producto final (Depende de 2)
            new BoletosApiTest(httpClient).RunAllTests();

            Console.WriteLine("Pruebas finalizadas.");
            Console.ReadLine();
        }
    }
}
