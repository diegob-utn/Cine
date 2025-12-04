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
            // GET
            var response = _httpClient.GetAsync(_ruta).Result;
            var json = response.Content.ReadAsStringAsync().Result;
            var boletos = JsonConvert.DeserializeObject<Modelos.ApiResult<List<Modelos.Boleto>>>(json);

            // INSERT
            var nuevoBoleto = new Modelos.Boleto { Id = 0, Cliente = "Test", Asiento = 1, FuncionId = 1 };
            var boletoJson = JsonConvert.SerializeObject(nuevoBoleto);
            var content = new StringContent(boletoJson, Encoding.UTF8, "application/json");
            response = _httpClient.PostAsync(_ruta, content).Result;
            json = response.Content.ReadAsStringAsync().Result;
            var boletoCreado = JsonConvert.DeserializeObject<Modelos.ApiResult<Modelos.Boleto>>(json);

            // UPDATE
            boletoCreado.Data.Asiento = 2;
            boletoJson = JsonConvert.SerializeObject(boletoCreado.Data);
            content = new StringContent(boletoJson, Encoding.UTF8, "application/json");
            response = _httpClient.PutAsync($"{_ruta}/{boletoCreado.Data.Id}", content).Result;
            json = response.Content.ReadAsStringAsync().Result;
            var boletoUpdate = JsonConvert.DeserializeObject<Modelos.ApiResult<Modelos.Boleto>>(json);

            // DELETE
            response = _httpClient.DeleteAsync($"{_ruta}/{boletoCreado.Data.Id}").Result;
            json = response.Content.ReadAsStringAsync().Result;
            var boletoDelete = JsonConvert.DeserializeObject<Modelos.ApiResult<Modelos.Boleto>>(json);

            Console.WriteLine($"BoletosApiTest: {json}");
        }
    }
}
