using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Cine.ApiTest
{
    public class BoletosApiTest
    {
        private readonly HttpClient _httpClient;
        private readonly string _ruta = "api/Boletos";

        public BoletosApiTest(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void RunAllTests()
        {
            Console.WriteLine("--- Iniciando Test de Boletos (Flujo con Persistencia) ---");

            // 1. GET
            var response = _httpClient.GetAsync(_ruta).Result;

            // 2. INSERT (Dato Original)
            var boletoOriginal = new Modelos.Boleto { Id = 0, Cliente = "Cliente Test " + DateTime.Now.Ticks, Asiento = 1, FuncionId = 1 };
            var jsonOriginal = JsonConvert.SerializeObject(boletoOriginal);
            var contentOriginal = new StringContent(jsonOriginal, Encoding.UTF8, "application/json");
            response = _httpClient.PostAsync(_ruta, contentOriginal).Result;
            var jsonResp = response.Content.ReadAsStringAsync().Result;
            var boletoCreado1 = JsonConvert.DeserializeObject<Modelos.ApiResult<Modelos.Boleto>>(jsonResp);

            if(boletoCreado1 == null || !boletoCreado1.Success || boletoCreado1.Data == null)
            {
                Console.WriteLine($" Error creando Boleto 1: {jsonResp}");
                return;
            }
            Console.WriteLine($" Boleto 1 Creado: (ID: {boletoCreado1.Data.Id})");

            // 3. "UPDATE" (Simulado como Nuevo Insert para mantener historial)
            var boletoUpdate = new Modelos.Boleto
            {
                Id = 0,
                Cliente = "Cliente Test Update " + DateTime.Now.Ticks,
                Asiento = 2,
                FuncionId = 1
            };
            var jsonUpdate = JsonConvert.SerializeObject(boletoUpdate);
            var contentUpdate = new StringContent(jsonUpdate, Encoding.UTF8, "application/json");
            response = _httpClient.PostAsync(_ruta, contentUpdate).Result;
            jsonResp = response.Content.ReadAsStringAsync().Result;
            var boletoCreado2 = JsonConvert.DeserializeObject<Modelos.ApiResult<Modelos.Boleto>>(jsonResp);

            Console.WriteLine($" Boleto 2 (Versión Update) Creado: (ID: {boletoCreado2.Data.Id})");

            // 4. DELETE (Eliminamos el primero para limpiar, pero dejamos el segundo)
            Console.WriteLine($"? Eliminando Boleto 1 (ID: {boletoCreado1.Data.Id})...");
            response = _httpClient.DeleteAsync($"{_ruta}/{boletoCreado1.Data.Id}").Result;

            if(response.IsSuccessStatusCode)
            {
                Console.WriteLine($" Boleto 1 Eliminado correctamente.");
                Console.WriteLine($" El sistema mantiene el Boleto ID {boletoCreado2.Data.Id} para uso futuro.");
            }
            else
            {
                Console.WriteLine($" Error al eliminar Boleto 1: {response.StatusCode}");
            }

            Console.WriteLine("--- Fin Test Boletos ---");
        }
    }
}
