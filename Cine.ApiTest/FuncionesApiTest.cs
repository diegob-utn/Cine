using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Cine.ApiTest
{
    public class FuncionesApiTest
    {
        private readonly HttpClient _httpClient;
        private readonly string _ruta = "api/Funciones";

        public FuncionesApiTest(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void RunAllTests()
        {
            Console.WriteLine("--- Iniciando Test de Funciones (Flujo con Persistencia) ---");

            // BUSCAR DEPENDENCIAS
            int idSala = GetFirstId("api/Salas");
            int idPelicula = GetFirstId("api/Peliculas");

            if(idSala == 0 || idPelicula == 0)
            {
                Console.WriteLine("SKIPPED: No hay Salas o Películas disponibles. Ejecuta los tests de Salas y Peliculas primero.");
                return;
            }

            // 1. INSERT (Temporal)
            // IMPORTANTE: Usamos DateTime.UtcNow para evitar errores de zona horaria en PostgreSQL
            var funcTemp = new Modelos.Funcion
            {
                Id = 0,
                FechaHora = DateTime.UtcNow.AddHours(2),
                SalaId = idSala,
                PeliculaId = idPelicula
            };

            var funcTempCreada = CreateFuncion(funcTemp);
            if(funcTempCreada == null) return;
            Console.WriteLine($"Función Temporal Creada: ID {funcTempCreada.Id}");

            // 2. INSERT (Persistente)
            var funcPerm = new Modelos.Funcion
            {
                Id = 0,
                FechaHora = DateTime.UtcNow.AddDays(1),
                SalaId = idSala,
                PeliculaId = idPelicula
            };

            var funcPermCreada = CreateFuncion(funcPerm);
            if(funcPermCreada == null) return;
            Console.WriteLine($"Función Persistente Creada: ID {funcPermCreada.Id}");

            // 3. UPDATE
            funcPermCreada.FechaHora = funcPermCreada.FechaHora.AddHours(2);
            var jsonUpdate = JsonConvert.SerializeObject(funcPermCreada);
            var contentUpdate = new StringContent(jsonUpdate, Encoding.UTF8, "application/json");

            var response = _httpClient.PutAsync($"{_ruta}/{funcPermCreada.Id}", contentUpdate).Result;
            if(response.IsSuccessStatusCode)
                Console.WriteLine($"Función Persistente Actualizada.");
            else
                Console.WriteLine($"Error Update Funcion: {response.StatusCode}");

            // 4. DELETE
            Console.WriteLine($"Eliminando Función Temporal (ID: {funcTempCreada.Id})...");
            _httpClient.DeleteAsync($"{_ruta}/{funcTempCreada.Id}").Wait();
            Console.WriteLine($"Función Temporal eliminada. La Función ID {funcPermCreada.Id} queda disponible.");

            Console.WriteLine("--- Fin Test Funciones ---");
        }

        private Modelos.Funcion? CreateFuncion(Modelos.Funcion funcion)
        {
            var json = JsonConvert.SerializeObject(funcion);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = _httpClient.PostAsync(_ruta, content).Result;
            var respJson = response.Content.ReadAsStringAsync().Result;

            if(!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error creando Función: {respJson}");
                return null;
            }

            var result = JsonConvert.DeserializeObject<Modelos.ApiResult<Modelos.Funcion>>(respJson);
            return result?.Success == true ? result.Data : null;
        }

        private int GetFirstId(string endpoint)
        {
            try
            {
                var json = _httpClient.GetStringAsync(endpoint).Result;
                // Deserializamos a una estructura genérica para extraer el ID
                var definition = new { data = new List<dynamic>() };
                dynamic result = JsonConvert.DeserializeObject(json);

                if(result?.data != null && result.data.Count > 0)
                    return (int)result.data[0].id;
            }
            catch { }
            return 0;
        }
    }
}
