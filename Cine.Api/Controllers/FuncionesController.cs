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
    public class FuncionesController : ControllerBase
    {
        private readonly DbContext _context;

        public FuncionesController(DbContext context)
        {
            _context = context;
        }

        // GET: api/Funciones
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Funcion>>>> GetFuncion()
        {
            try
            {
                var data = await _context.Funciones.ToListAsync();
                return ApiResult<List<Funcion>>.Ok(data);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Funcion>>.Fail(ex.Message);
            }
        }

        // GET: api/Funciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Funcion>>> GetFuncion(int id)
        {
            try
            {
                var funcion = await _context.Funciones.FindAsync(id);
                if (funcion == null)
                {
                    return ApiResult<Funcion>.Fail("Datos no encontrados");
                }
                return ApiResult<Funcion>.Ok(funcion);
            }
            catch (Exception ex)
            {
                return ApiResult<Funcion>.Fail(ex.Message);
            }
        }

        // PUT: api/Funciones/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Funcion>>> PutFuncion(int id, Funcion funcion)
        {
            if (id != funcion.Id)
            {
                return ApiResult<Funcion>.Fail("No coinciden los identificadores");
            }

            _context.Entry(funcion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!FuncionExists(id))
                {
                    return ApiResult<Funcion>.Fail("Datos no encontrados");
                }
                else
                {
                    return ApiResult<Funcion>.Fail(ex.Message);
                }
            }

            return ApiResult<Funcion>.Ok(null);
        }

        // POST: api/Funciones
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ApiResult<Funcion>>> PostFuncion(Funcion funcion)
        {
            try
            {
                _context.Funciones.Add(funcion);
                await _context.SaveChangesAsync();
                return ApiResult<Funcion>.Ok(funcion);
            }
            catch (Exception ex)
            {
                return ApiResult<Funcion>.Fail(ex.Message);
            }
        }

        // DELETE: api/Funciones/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Funcion>>> DeleteFuncion(int id)
        {
            try
            {
                var funcion = await _context.Funciones.FindAsync(id);
                if (funcion == null)
                {
                    return ApiResult<Funcion>.Fail("Datos no encontrados");
                }
                _context.Funciones.Remove(funcion);
                await _context.SaveChangesAsync();
                return ApiResult<Funcion>.Ok(null);
            }
            catch (Exception ex)
            {
                return ApiResult<Funcion>.Fail(ex.Message);
            }
        }

        private bool FuncionExists(int id)
        {
            return _context.Funciones.Any(e => e.Id == id);
        }
    }
}
