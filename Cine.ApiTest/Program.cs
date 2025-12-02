namespace Cine.ApiTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var httpClient = new HttpClient();
            string rutaPeliculas = "api/Peliculas";

            httpClient.BaseAddress = new Uri("https://localhost:5251/");
            var response = httpClient.GetAsync(rutaPeliculas).Result;
            var json = response.Content.ReadAsStringAsync().Result;
            var peliculas = Newtonsoft.Json.JsonConvert.DeserializeObject<Modelos.ApiResult<List<Modelos.Pelicula>>>(json);


            // insertar
            var nuevaPelicula = new Modelos.Pelicula
            {
                Id = 0,
                Titulo = "Lobo gris"
            };

            var peliculaJson = Newtonsoft.Json.JsonConvert.SerializeObject(nuevaPelicula);
            var content = new StringContent(peliculaJson, System.Text.Encoding.UTF8, "application/json");
            response = httpClient.PostAsync(rutaPeliculas, content).Result;
            json = response.Content.ReadAsStringAsync().Result;
            var peliculaCreada = Newtonsoft.Json.JsonConvert.DeserializeObject<Modelos.ApiResult<Modelos.Pelicula>>(json);



            //update
            peliculaCreada.Data.Titulo = "Lobo gris actualizado";

            peliculaJson = Newtonsoft.Json.JsonConvert.SerializeObject(peliculaCreada.Data);
            content = new StringContent(peliculaJson, System.Text.Encoding.UTF8, "application/json");
            response = httpClient.PutAsync($"{rutaPeliculas}/{peliculaCreada.Data.Id}", content).Result;
            json = response.Content.ReadAsStringAsync().Result;
            var peliculaUpdate = Newtonsoft.Json.JsonConvert.DeserializeObject<Modelos.ApiResult<Modelos.Pelicula>>(json);

            //eliminar

            response = httpClient.DeleteAsync($"{rutaPeliculas}/{peliculaCreada.Data.Id}").Result;
            json = response.Content.ReadAsStringAsync().Result;
            var peliculaDelete = Newtonsoft.Json.JsonConvert.DeserializeObject<Modelos.ApiResult<Modelos.Pelicula>>(json);


            Console.WriteLine(json);
            Console.ReadLine();


        }
    }
}
