using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Cine.ApiTest
{
    public class SalasApiTest
    {
        private readonly HttpClient _httpClient;
        private readonly string _ruta = "api/Salas";

        public SalasApiTest(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void RunAllTests()
        {
            Console.WriteLine("--- Iniciando Test de Salas (Flujo con Persistencia) ---");

            // 1. GET
            var response = _httpClient.GetAsync(_ruta).Result;

            // 2. INSERT (Dato Original)
            var salaOriginal = new Modelos.Sala { Id = 0, Nombre = "Sala Test ", Capacidad = 100 };
            var jsonOriginal = JsonConvert.SerializeObject(salaOriginal);
            var contentOriginal = new StringContent(jsonOriginal, Encoding.UTF8, "application/json");
            response = _httpClient.PostAsync(_ruta, contentOriginal).Result;
            var jsonResp = response.Content.ReadAsStringAsync().Result;
            var salaCreada1 = JsonConvert.DeserializeObject<Modelos.ApiResult<Modelos.Sala>>(jsonResp);

            if(salaCreada1 == null || !salaCreada1.Success || salaCreada1.Data == null)
            {
                Console.WriteLine($"Error creando Sala 1: {jsonResp}");
                return;
            }
            Console.WriteLine($"Sala 1 Creada: {salaCreada1.Data.Nombre} (ID: {salaCreada1.Data.Id})");

            // 3. "UPDATE" (Simulado como Nuevo Insert para mantener historial)
            var salaUpdate = new Modelos.Sala
            {
                Id = 0,
                Nombre = "Sala Test Update",
                Capacidad = 120
            };
            var jsonUpdate = JsonConvert.SerializeObject(salaUpdate);
            var contentUpdate = new StringContent(jsonUpdate, Encoding.UTF8, "application/json");
            response = _httpClient.PostAsync(_ruta, contentUpdate).Result;
            jsonResp = response.Content.ReadAsStringAsync().Result;
            var salaCreada2 = JsonConvert.DeserializeObject<Modelos.ApiResult<Modelos.Sala>>(jsonResp);

            Console.WriteLine($"Sala 2 (Versión Update) Creada: {salaCreada2.Data.Nombre} (ID: {salaCreada2.Data.Id})");

            // 4. DELETE (Eliminamos la primera para limpiar, pero dejamos la segunda)
            Console.WriteLine($"Eliminando Sala 1 (ID: {salaCreada1.Data.Id})...");
            response = _httpClient.DeleteAsync($"{_ruta}/{salaCreada1.Data.Id}").Result;

            if(response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Sala 1 Eliminada correctamente.");
                Console.WriteLine($"El sistema mantiene la Sala ID {salaCreada2.Data.Id} para uso futuro.");
            }
            else
            {
                Console.WriteLine($"Error al eliminar Sala 1: {response.StatusCode}");
            }

            Console.WriteLine("--- Fin Test Salas ---");
        }
    }
}
