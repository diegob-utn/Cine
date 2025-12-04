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
            // GET
            var response = _httpClient.GetAsync(_ruta).Result;
            var json = response.Content.ReadAsStringAsync().Result;
            var peliculas = JsonConvert.DeserializeObject<Modelos.ApiResult<List<Modelos.Pelicula>>>(json);

            // INSERT
            var nuevaPelicula = new Modelos.Pelicula { Id = 0, Titulo = "Lobo gris" };
            var peliculaJson = JsonConvert.SerializeObject(nuevaPelicula);
            var content = new StringContent(peliculaJson, Encoding.UTF8, "application/json");
            response = _httpClient.PostAsync(_ruta, content).Result;
            json = response.Content.ReadAsStringAsync().Result;
            var peliculaCreada = JsonConvert.DeserializeObject<Modelos.ApiResult<Modelos.Pelicula>>(json);

            // UPDATE
            peliculaCreada.Data.Titulo = "Lobo gris actualizado";
            peliculaJson = JsonConvert.SerializeObject(peliculaCreada.Data);
            content = new StringContent(peliculaJson, Encoding.UTF8, "application/json");
            response = _httpClient.PutAsync($"{_ruta}/{peliculaCreada.Data.Id}", content).Result;
            json = response.Content.ReadAsStringAsync().Result;
            var peliculaUpdate = JsonConvert.DeserializeObject<Modelos.ApiResult<Modelos.Pelicula>>(json);

            // DELETE
            response = _httpClient.DeleteAsync($"{_ruta}/{peliculaCreada.Data.Id}").Result;
            json = response.Content.ReadAsStringAsync().Result;
            var peliculaDelete = JsonConvert.DeserializeObject<Modelos.ApiResult<Modelos.Pelicula>>(json);

            Console.WriteLine($"PeliculasApiTest: {json}");
        }
    }
}
