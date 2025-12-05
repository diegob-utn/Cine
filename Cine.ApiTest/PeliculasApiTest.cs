using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Cine.ApiTest
{
    public class PeliculasApiTest
    {
        private readonly HttpClient _httpClient;
        private readonly string _ruta = "api/Peliculas";

        public PeliculasApiTest(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void RunAllTests()
        {
            Console.WriteLine("--- Iniciando Test de Peliculas (Flujo con Persistencia) ---");

            // 1. GET: Ver qué hay
            var response = _httpClient.GetAsync(_ruta).Result;

            // 2. INSERT (Dato Original)
            var peliOriginal = new Modelos.Pelicula { Id = 0, Titulo = "Pelicula Original ", Duracion = 120, Genero = "Accion", Clasificacion = "PG", Idioma = "ES", Descripcion = "Sera borrada" };
            var jsonOriginal = JsonConvert.SerializeObject(peliOriginal);
            var contentOriginal = new StringContent(jsonOriginal, Encoding.UTF8, "application/json");

            response = _httpClient.PostAsync(_ruta, contentOriginal).Result;
            var jsonResp = response.Content.ReadAsStringAsync().Result;
            var peliCreada1 = JsonConvert.DeserializeObject<Modelos.ApiResult<Modelos.Pelicula>>(jsonResp);

            if(peliCreada1 == null || !peliCreada1.Success || peliCreada1.Data == null)
            {
                Console.WriteLine($"Error creando Pelicula 1: {jsonResp}");
                return;
            }
            Console.WriteLine($"Pelicula 1 Creada: {peliCreada1.Data.Titulo} (ID: {peliCreada1.Data.Id})");

            // 3. "UPDATE" (Simulado como Nuevo Insert para mantener historial)
            // Creamos un segundo registro que será el que sobreviva
            var peliUpdate = new Modelos.Pelicula
            {
                Id = 0,
                Titulo = "Pelicula Updated ",
                Duracion = 130,
                Genero = "Accion",
                Clasificacion = "PG-13",
                Idioma = "EN",
                Descripcion = "Esta pelicula sobrevivirá para los siguientes tests"
            };

            var jsonUpdate = JsonConvert.SerializeObject(peliUpdate);
            var contentUpdate = new StringContent(jsonUpdate, Encoding.UTF8, "application/json");

            response = _httpClient.PostAsync(_ruta, contentUpdate).Result;
            jsonResp = response.Content.ReadAsStringAsync().Result;
            var peliCreada2 = JsonConvert.DeserializeObject<Modelos.ApiResult<Modelos.Pelicula>>(jsonResp);

            Console.WriteLine($"Pelicula 2 (Versión Update) Creada: {peliCreada2.Data.Titulo} (ID: {peliCreada2.Data.Id})");

            // 4. DELETE (Eliminamos la primera para limpiar, pero dejamos la segunda)
            Console.WriteLine($"Eliminando Pelicula 1 (ID: {peliCreada1.Data.Id})...");
            response = _httpClient.DeleteAsync($"{_ruta}/{peliCreada1.Data.Id}").Result;

            if(response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Pelicula 1 Eliminada correctamente.");
                Console.WriteLine($"El sistema mantiene la Pelicula ID {peliCreada2.Data.Id} para uso futuro.");
            }
            else
            {
                Console.WriteLine($"Error al eliminar Pelicula 1: {response.StatusCode}");
            }

            Console.WriteLine("--- Fin Test Peliculas ---");
        }
    }
}
