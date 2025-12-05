using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

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

            // 1. GET
            var response = _httpClient.GetAsync(_ruta).Result;

            // 2. INSERT (Dato Original)
            var funcionOriginal = new Modelos.Funcion { Id = 0, FechaHora = DateTime.Now, PeliculaId = 1, SalaId = 1 };
            var jsonOriginal = JsonConvert.SerializeObject(funcionOriginal);
            var contentOriginal = new StringContent(jsonOriginal, Encoding.UTF8, "application/json");
            response = _httpClient.PostAsync(_ruta, contentOriginal).Result;
            var jsonResp = response.Content.ReadAsStringAsync().Result;
            var funcionCreada1 = JsonConvert.DeserializeObject<Modelos.ApiResult<Modelos.Funcion>>(jsonResp);

            if(funcionCreada1 == null || !funcionCreada1.Success || funcionCreada1.Data == null)
            {
                Console.WriteLine($" Error creando Funcion 1: {jsonResp}");
                return;
            }
            Console.WriteLine($" Funcion 1 Creada: (ID: {funcionCreada1.Data.Id})");

            // 3. "UPDATE" (Simulado como Nuevo Insert para mantener historial)
            var funcionUpdate = new Modelos.Funcion
            {
                Id = 0,
                FechaHora = DateTime.Now.AddDays(1),
                PeliculaId = 1,
                SalaId = 1
            };
            var jsonUpdate = JsonConvert.SerializeObject(funcionUpdate);
            var contentUpdate = new StringContent(jsonUpdate, Encoding.UTF8, "application/json");
            response = _httpClient.PostAsync(_ruta, contentUpdate).Result;
            jsonResp = response.Content.ReadAsStringAsync().Result;
            var funcionCreada2 = JsonConvert.DeserializeObject<Modelos.ApiResult<Modelos.Funcion>>(jsonResp);

            Console.WriteLine($" Funcion 2 (Versión Update) Creada: (ID: {funcionCreada2.Data.Id})");

            // 4. DELETE (Eliminamos la primera para limpiar, pero dejamos la segunda)
            Console.WriteLine($"? Eliminando Funcion 1 (ID: {funcionCreada1.Data.Id})...");
            response = _httpClient.DeleteAsync($"{_ruta}/{funcionCreada1.Data.Id}").Result;

            if(response.IsSuccessStatusCode)
            {
                Console.WriteLine($" Funcion 1 Eliminada correctamente.");
                Console.WriteLine($" El sistema mantiene la Funcion ID {funcionCreada2.Data.Id} para uso futuro.");
            }
            else
            {
                Console.WriteLine($" Error al eliminar Funcion 1: {response.StatusCode}");
            }

            Console.WriteLine("--- Fin Test Funciones ---");
        }
    }
}
