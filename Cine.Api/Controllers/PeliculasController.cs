using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cine;
using Cine.Modelos;

namespace Cine.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeliculasController : ControllerBase
    {
        private readonly DbContext _context;

        public PeliculasController(DbContext context)
        {
            _context = context;
        }

        // GET: api/Peliculas
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Pelicula>>>> GetPelicula()
        {
            try
            {
                var data = await _context.Peliculas.ToListAsync();
                return ApiResult<List<Pelicula>>.Ok(data);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Pelicula>>.Fail(ex.Message);
            }
        }

        // GET: api/Peliculas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Pelicula>>> GetPelicula(int id)
        {
            try
            {
                var pelicula = await _context.Peliculas.FindAsync(id);
                if (pelicula == null)
                {
                    return ApiResult<Pelicula>.Fail("Datos no encontrados");
                }
                return ApiResult<Pelicula>.Ok(pelicula);
            }
            catch (Exception ex)
            {
                return ApiResult<Pelicula>.Fail(ex.Message);
            }
        }

        // PUT: api/Peliculas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Pelicula>>> PutPelicula(int id, Pelicula pelicula)
        {
            if (id != pelicula.Id)
            {
                return ApiResult<Pelicula>.Fail("No coinciden los identificadores");
            }

            _context.Entry(pelicula).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!PeliculaExists(id))
                {
                    return ApiResult<Pelicula>.Fail("Datos no encontrados");
                }
                else
                {
                    return ApiResult<Pelicula>.Fail(ex.Message);
                }
            }

            return ApiResult<Pelicula>.Ok(null);
        }

        // POST: api/Peliculas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ApiResult<Pelicula>>> PostPelicula(Pelicula pelicula)
        {
            try
            {
                _context.Peliculas.Add(pelicula);
                await _context.SaveChangesAsync();
                return ApiResult<Pelicula>.Ok(pelicula);
            }
            catch (Exception ex)
            {
                return ApiResult<Pelicula>.Fail(ex.Message);
            }
        }

        // DELETE: api/Peliculas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Pelicula>>> DeletePelicula(int id)
        {
            try
            {
                var pelicula = await _context.Peliculas.FindAsync(id);
                if (pelicula == null)
                {
                    return ApiResult<Pelicula>.Fail("Datos no encontrados");
                }
                _context.Peliculas.Remove(pelicula);
                await _context.SaveChangesAsync();
                return ApiResult<Pelicula>.Ok(null);
            }
            catch (Exception ex)
            {
                return ApiResult<Pelicula>.Fail(ex.Message);
            }
        }

        private bool PeliculaExists(int id)
        {
            return _context.Peliculas.Any(e => e.Id == id);
        }
    }
}
