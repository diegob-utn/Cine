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
            // GET
            var response = _httpClient.GetAsync(_ruta).Result;
            var json = response.Content.ReadAsStringAsync().Result;
            var funciones = JsonConvert.DeserializeObject<Modelos.ApiResult<List<Modelos.Funcion>>>(json);

            // INSERT
            var nuevaFuncion = new Modelos.Funcion { Id = 0, FechaHora = DateTime.Now, PeliculaId = 1, SalaId = 1 };
            var funcionJson = JsonConvert.SerializeObject(nuevaFuncion);
            var content = new StringContent(funcionJson, Encoding.UTF8, "application/json");
            response = _httpClient.PostAsync(_ruta, content).Result;
            json = response.Content.ReadAsStringAsync().Result;
            var funcionCreada = JsonConvert.DeserializeObject<Modelos.ApiResult<Modelos.Funcion>>(json);

            // UPDATE
            funcionCreada.Data.FechaHora = DateTime.Now.AddDays(1);
            funcionJson = JsonConvert.SerializeObject(funcionCreada.Data);
            content = new StringContent(funcionJson, Encoding.UTF8, "application/json");
            response = _httpClient.PutAsync($"{_ruta}/{funcionCreada.Data.Id}", content).Result;
            json = response.Content.ReadAsStringAsync().Result;
            var funcionUpdate = JsonConvert.DeserializeObject<Modelos.ApiResult<Modelos.Funcion>>(json);

            // DELETE
            response = _httpClient.DeleteAsync($"{_ruta}/{funcionCreada.Data.Id}").Result;
            json = response.Content.ReadAsStringAsync().Result;
            var funcionDelete = JsonConvert.DeserializeObject<Modelos.ApiResult<Modelos.Funcion>>(json);

            Console.WriteLine($"FuncionesApiTest: {json}");
        }
    }
}
