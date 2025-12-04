using System;
using System.Net.Http;

namespace Cine.ApiTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://cine-g99w.onrender.com/api/");

            // Orden correcto: Salas, Peliculas, Funciones, Boletos
            new SalasApiTest(httpClient).RunAllTests();
            new PeliculasApiTest(httpClient).RunAllTests();
            new FuncionesApiTest(httpClient).RunAllTests();
            new BoletosApiTest(httpClient).RunAllTests();

            Console.WriteLine("Pruebas finalizadas.");
            Console.ReadLine();
        }
    }
}
