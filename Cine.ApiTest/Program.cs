using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Cine.Modelos; // Asegúrate de tener referenciado Cine.Modelos
using Newtonsoft.Json;

namespace Cine.ApiTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // --- CONFIGURACIÓN ---
            var httpClient = new HttpClient();
            // Asegúrate de que esta URL sea la correcta
            httpClient.BaseAddress = new Uri("https://cine-g99w.onrender.com");
            
            // IDs que sobrevivirán para usarse en los pasos siguientes
            int peliculaFinalId = 0;
            int salaFinalId = 0;
            int funcionFinalId = 0;

            Console.WriteLine("=================================================");
            Console.WriteLine(" INICIANDO TEST DE INTEGRIDAD (CON DETALLE DE DATOS)");
            Console.WriteLine("=================================================");

            // =================================================================================
            // 1. PELICULAS
            // =================================================================================
            Console.WriteLine("\n--- 1. GESTIÓN DE PELICULAS ---");
            string rutaPeliculas = "api/Peliculas";

            // A) GET
            var getResp = httpClient.GetAsync(rutaPeliculas).Result;
            var listaPelis = JsonConvert.DeserializeObject<ApiResult<List<Pelicula>>>(getResp.Content.ReadAsStringAsync().Result);
            
            Console.WriteLine($"[GET] Total actual en BD: {listaPelis?.Data?.Count ?? 0}");
            if (listaPelis?.Data != null && listaPelis.Data.Count > 0)
            {
                foreach (var p in listaPelis.Data)
                {
                    Console.WriteLine($"   > ID: {p.Id} | Título: {p.Titulo}");
                }
            }

            // B) INSERT (Original)
            var peliV1 = new Pelicula
            {
                Id = 0,
                Titulo = "Pelicula V1 (A Borrar)",
                Duracion = 120,
                Genero = "Test",
                Clasificacion = "A",
                Idioma = "ES",
                Descripcion = "Registro original"
            };
            var peliV1Creada = PostData(httpClient, rutaPeliculas, peliV1);

            if (peliV1Creada != null)
            {
                Console.WriteLine($"[INSERT] Original creada (ID: {peliV1Creada.Id}) - {peliV1Creada.Titulo}");

                // C) UPDATE (Creando NUEVO registro para historial)
                var peliV2 = new Pelicula
                {
                    Id = 0, // ID 0 fuerza la creación de uno nuevo
                    Titulo = "Pelicula V2 (Actualizada)",
                    Duracion = 120,
                    Genero = "Test",
                    Clasificacion = "A",
                    Idioma = "ES",
                    Descripcion = $"Actualización del ID {peliV1Creada.Id}"
                };
                var peliV2Creada = PostData(httpClient, rutaPeliculas, peliV2);
                
                if(peliV2Creada != null)
                {
                    Console.WriteLine($"[UPDATE-INSERT] Versión actualizada creada (ID: {peliV2Creada.Id}) - {peliV2Creada.Titulo}");
                    peliculaFinalId = peliV2Creada.Id; // Guardamos este ID para el futuro

                    // D) DELETE (El original)
                    DeleteData(httpClient, rutaPeliculas, peliV1Creada.Id);
                    Console.WriteLine($"[DELETE] Registro anterior (ID: {peliV1Creada.Id}) eliminado por integridad.");
                    Console.WriteLine($"[ESTADO] Se mantiene Pelicula ID: {peliculaFinalId}");
                }
            }

            // =================================================================================
            // 2. SALAS
            // =================================================================================
            Console.WriteLine("\n--- 2. GESTIÓN DE SALAS ---");
            string rutaSalas = "api/Salas";

            // A) GET
            getResp = httpClient.GetAsync(rutaSalas).Result;
            var listaSalas = JsonConvert.DeserializeObject<ApiResult<List<Sala>>>(getResp.Content.ReadAsStringAsync().Result);
            
            Console.WriteLine($"[GET] Total actual en BD: {listaSalas?.Data?.Count ?? 0}");
            if (listaSalas?.Data != null && listaSalas.Data.Count > 0)
            {
                foreach (var s in listaSalas.Data)
                {
                    Console.WriteLine($"   > ID: {s.Id} | Sala: {s.Nombre} | Capacidad: {s.Capacidad}");
                }
            }

            // B) INSERT (Original)
            var salaV1 = new Sala { Id = 0, Nombre = "Sala V1", Capacidad = 50 };
            var salaV1Creada = PostData(httpClient, rutaSalas, salaV1);

            if (salaV1Creada != null)
            {
                Console.WriteLine($"[INSERT] Original creada (ID: {salaV1Creada.Id}) - {salaV1Creada.Nombre}");

                // C) UPDATE (Nuevo Registro)
                var salaV2 = new Sala { Id = 0, Nombre = "Sala V2 (Upgrade)", Capacidad = 100 };
                var salaV2Creada = PostData(httpClient, rutaSalas, salaV2);

                if (salaV2Creada != null)
                {
                    Console.WriteLine($"[UPDATE-INSERT] Versión actualizada creada (ID: {salaV2Creada.Id}) - {salaV2Creada.Nombre}");
                    salaFinalId = salaV2Creada.Id;

                    // D) DELETE
                    DeleteData(httpClient, rutaSalas, salaV1Creada.Id);
                    Console.WriteLine($"[DELETE] Registro anterior (ID: {salaV1Creada.Id}) eliminado.");
                    Console.WriteLine($"[ESTADO] Se mantiene Sala ID: {salaFinalId}");
                }
            }

            // =================================================================================
            // 3. FUNCIONES
            // =================================================================================
            Console.WriteLine("\n--- 3. GESTIÓN DE FUNCIONES ---");
            string rutaFunciones = "api/Funciones";

            if (peliculaFinalId > 0 && salaFinalId > 0)
            {
                // A) GET
                getResp = httpClient.GetAsync(rutaFunciones).Result;
                var listaFunc = JsonConvert.DeserializeObject<ApiResult<List<Funcion>>>(getResp.Content.ReadAsStringAsync().Result);
                
                Console.WriteLine($"[GET] Total actual en BD: {listaFunc?.Data?.Count ?? 0}");
                if (listaFunc?.Data != null && listaFunc.Data.Count > 0)
                {
                    foreach (var f in listaFunc.Data)
                    {
                        Console.WriteLine($"   > ID: {f.Id} | Fecha: {f.FechaHora} | PeliID: {f.PeliculaId} | SalaID: {f.SalaId}");
                    }
                }

                // B) INSERT (Original) - USAMOS UTC
                var funcionV1 = new Funcion 
                { 
                    Id = 0, 
                    FechaHora = DateTime.UtcNow.AddDays(1), 
                    PeliculaId = peliculaFinalId, 
                    SalaId = salaFinalId 
                };
                var funcionV1Creada = PostData(httpClient, rutaFunciones, funcionV1);

                if (funcionV1Creada != null)
                {
                    Console.WriteLine($"[INSERT] Original creada (ID: {funcionV1Creada.Id}) - Fecha: {funcionV1Creada.FechaHora}");

                    // C) UPDATE (Nuevo Registro)
                    var funcionV2 = new Funcion
                    {
                        Id = 0,
                        FechaHora = DateTime.UtcNow.AddDays(2),
                        PeliculaId = peliculaFinalId,
                        SalaId = salaFinalId
                    };
                    var funcionV2Creada = PostData(httpClient, rutaFunciones, funcionV2);

                    if (funcionV2Creada != null)
                    {
                        Console.WriteLine($"[UPDATE-INSERT] Versión actualizada creada (ID: {funcionV2Creada.Id}) - Fecha: {funcionV2Creada.FechaHora}");
                        funcionFinalId = funcionV2Creada.Id;

                        // D) DELETE
                        DeleteData(httpClient, rutaFunciones, funcionV1Creada.Id);
                        Console.WriteLine($"[DELETE] Registro anterior (ID: {funcionV1Creada.Id}) eliminado.");
                        Console.WriteLine($"[ESTADO] Se mantiene Funcion ID: {funcionFinalId}");
                    }
                }
            }
            else
            {
                Console.WriteLine("[SKIP] Saltando Funciones (Faltan IDs previos).");
            }

            // =================================================================================
            // 4. BOLETOS
            // =================================================================================
            Console.WriteLine("\n--- 4. GESTIÓN DE BOLETOS ---");
            string rutaBoletos = "api/Boletos";

            if (funcionFinalId > 0)
            {
                // A) GET
                getResp = httpClient.GetAsync(rutaBoletos).Result;
                var listaBol = JsonConvert.DeserializeObject<ApiResult<List<Boleto>>>(getResp.Content.ReadAsStringAsync().Result);
                
                Console.WriteLine($"[GET] Total actual en BD: {listaBol?.Data?.Count ?? 0}");
                if (listaBol?.Data != null && listaBol.Data.Count > 0)
                {
                    foreach (var b in listaBol.Data)
                    {
                        Console.WriteLine($"   > ID: {b.Id} | Cliente: {b.Cliente} | Asiento: {b.Asiento}");
                    }
                }

                // B) INSERT (Original)
                var boletoV1 = new Boleto
                {
                    Id = 0,
                    Cliente = "Juan Perez (V1)",
                    Asiento = 5,
                    FuncionId = funcionFinalId
                };
                var boletoV1Creado = PostData(httpClient, rutaBoletos, boletoV1);

                if (boletoV1Creado != null)
                {
                    Console.WriteLine($"[INSERT] Original creado (ID: {boletoV1Creado.Id}) - Cliente: {boletoV1Creado.Cliente}");

                    // C) UPDATE (Nuevo Registro)
                    var boletoV2 = new Boleto
                    {
                        Id = 0,
                        Cliente = "Juan Perez (V2 - Corregido)",
                        Asiento = 5,
                        FuncionId = funcionFinalId
                    };
                    var boletoV2Creado = PostData(httpClient, rutaBoletos, boletoV2);

                    if (boletoV2Creado != null)
                    {
                        Console.WriteLine($"[UPDATE-INSERT] Versión actualizada creada (ID: {boletoV2Creado.Id}) - Cliente: {boletoV2Creado.Cliente}");
                        
                        // D) DELETE
                        DeleteData(httpClient, rutaBoletos, boletoV1Creado.Id);
                        Console.WriteLine($"[DELETE] Registro anterior (ID: {boletoV1Creado.Id}) eliminado.");
                        Console.WriteLine($"[ESTADO] Se mantiene Boleto ID: {boletoV2Creado.Id}");
                    }
                }
            }
            else
            {
                Console.WriteLine("[SKIP] Saltando Boletos (Falta ID Funcion).");
            }

            Console.WriteLine("\n--- TEST FINALIZADO CORRECTAMENTE ---");
            Console.ReadLine();
        }


        // --- MÉTODOS AUXILIARES ---

        static T? PostData<T>(HttpClient client, string url, T data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = client.PostAsync(url, content).Result;
                var respString = response.Content.ReadAsStringAsync().Result;
                
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[ERROR HTTP] {response.StatusCode} - {respString}");
                    return default;
                }

                var result = JsonConvert.DeserializeObject<ApiResult<T>>(respString);
                if (result == null || !result.Success)
                {
                    Console.WriteLine($"[ERROR API] {result?.Message ?? "Respuesta nula"}");
                    return default;
                }
                return result.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EXCEPCION] {ex.Message}");
                return default;
            }
        }

        static void DeleteData(HttpClient client, string url, int id)
        {
            try
            {
                var response = client.DeleteAsync($"{url}/{id}").Result;
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[ERROR DELETE] {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EXCEPCION DELETE] {ex.Message}");
            }
        }
    }
}
