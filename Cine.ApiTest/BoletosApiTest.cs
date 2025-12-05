using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;

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

            // BUSCAR DEPENDENCIAS (Necesitamos una función para vender el boleto)
            int idFuncion = GetFirstId("api/Funciones");

            if(idFuncion == 0)
            {
                Console.WriteLine(" SKIPPED: No hay Funciones disponibles. Ejecuta el test de Funciones primero.");
                return;
            }

            // 1. INSERT (Temporal) - Usamos Asiento 1
            var bolTemp = new Modelos.Boleto
            {
                Id = 0,
                Cliente = "Cliente Temp",
                Asiento = 1,
                FuncionId = idFuncion
            };

            var bolTempCreado = CreateBoleto(bolTemp);
            if(bolTempCreado == null) return;
            Console.WriteLine($"Boleto Temporal Creado: ID {bolTempCreado.Id}");

            // 2. INSERT (Persistente) - Usamos Asiento 2 (Para evitar conflicto con el 1)
            var bolPerm = new Modelos.Boleto
            {
                Id = 0,
                Cliente = "Cliente Persistente",
                Asiento = 2,
                FuncionId = idFuncion
            };

            var bolPermCreado = CreateBoleto(bolPerm);
            if(bolPermCreado == null) return;
            Console.WriteLine($" Boleto Persistente Creado: ID {bolPermCreado.Id}");

            // 3. UPDATE
            bolPermCreado.Cliente = "Cliente Updated";
            var jsonUpdate = JsonConvert.SerializeObject(bolPermCreado);
            var contentUpdate = new StringContent(jsonUpdate, Encoding.UTF8, "application/json");

            var response = _httpClient.PutAsync($"{_ruta}/{bolPermCreado.Id}", contentUpdate).Result;
            if(response.IsSuccessStatusCode)
                Console.WriteLine($"Boleto Persistente Actualizado.");
            else
                Console.WriteLine($"Error Update Boleto: {response.StatusCode}");

            // 4. DELETE
            Console.WriteLine($"Eliminando Boleto Temporal (ID: {bolTempCreado.Id})...");
            _httpClient.DeleteAsync($"{_ruta}/{bolTempCreado.Id}").Wait();
            Console.WriteLine($"Boleto Temporal eliminado. El Boleto ID {bolPermCreado.Id} queda disponible.");

            Console.WriteLine("--- Fin Test Boletos ---");
        }

        private Modelos.Boleto? CreateBoleto(Modelos.Boleto boleto)
        {
            var json = JsonConvert.SerializeObject(boleto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = _httpClient.PostAsync(_ruta, content).Result;
            var respJson = response.Content.ReadAsStringAsync().Result;

            if(!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error POST Boleto: {respJson}");
                return null;
            }

            var result = JsonConvert.DeserializeObject<Modelos.ApiResult<Modelos.Boleto>>(respJson);

            // Validación de éxito del API
            if(result == null || !result.Success)
            {
                Console.WriteLine($"API Error Boleto: {result?.Message ?? respJson}");
                return null;
            }

            return result.Data;
        }

        private int GetFirstId(string endpoint)
        {
            try
            {
                var json = _httpClient.GetStringAsync(endpoint).Result;
                // Deserialización dinámica para encontrar el ID sin importar el modelo exacto
                dynamic result = JsonConvert.DeserializeObject(json);

                if(result?.data != null && result.data.Count > 0)
                    return (int)result.data[0].id;
            }
            catch { }
            return 0;
        }
    }
}
