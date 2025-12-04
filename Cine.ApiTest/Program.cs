using System;
using System.Net.Http;

namespace Cine.ApiTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:5251/");

            new PeliculasApiTest(httpClient).RunAllTests();
            new FuncionesApiTest(httpClient).RunAllTests();
            new BoletosApiTest(httpClient).RunAllTests();

            Console.WriteLine("Pruebas finalizadas.");
            Console.ReadLine();
        }
    }
}
