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
            // GET
            var response = _httpClient.GetAsync(_ruta).Result;
            var json = response.Content.ReadAsStringAsync().Result;
            var salas = JsonConvert.DeserializeObject<Modelos.ApiResult<List<Modelos.Sala>>>(json);

            // INSERT
            var nuevaSala = new Modelos.Sala { Id = 0, Nombre = "Sala Test", Capacidad = 100 };
            var salaJson = JsonConvert.SerializeObject(nuevaSala);
            var content = new StringContent(salaJson, Encoding.UTF8, "application/json");
            response = _httpClient.PostAsync(_ruta, content).Result;
            json = response.Content.ReadAsStringAsync().Result;
            var salaCreada = JsonConvert.DeserializeObject<Modelos.ApiResult<Modelos.Sala>>(json);

            if(salaCreada == null || !salaCreada.Success || salaCreada.Data == null)
            {
                Console.WriteLine($"Error al crear sala: {salaCreada?.Message ?? json}");
                return;
            }

            // UPDATE
            salaCreada.Data.Nombre = "Sala Test Actualizada";
            salaJson = JsonConvert.SerializeObject(salaCreada.Data);
            content = new StringContent(salaJson, Encoding.UTF8, "application/json");
            response = _httpClient.PutAsync($"{_ruta}/{salaCreada.Data.Id}", content).Result;
            json = response.Content.ReadAsStringAsync().Result;
            var salaUpdate = JsonConvert.DeserializeObject<Modelos.ApiResult<Modelos.Sala>>(json);

            // DELETE
            response = _httpClient.DeleteAsync($"{_ruta}/{salaCreada.Data.Id}").Result;
            json = response.Content.ReadAsStringAsync().Result;
            var salaDelete = JsonConvert.DeserializeObject<Modelos.ApiResult<Modelos.Sala>>(json);

            Console.WriteLine($"SalasApiTest: {json}");
        }
    }
}
