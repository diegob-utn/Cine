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
    public class SalasController : ControllerBase
    {
        private readonly DbContext _context;

        public SalasController(DbContext context)
        {
            _context = context;
        }

        // GET: api/Salas
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Sala>>>> GetSala()
        {
            try
            {
                var data = await _context.Salas.ToListAsync();
                return ApiResult<List<Sala>>.Ok(data);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Sala>>.Fail(ex.Message);
            }
        }

        // GET: api/Salas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Sala>>> GetSala(int id)
        {
            try
            {
                var sala = await _context.Salas.FindAsync(id);
                if (sala == null)
                {
                    return ApiResult<Sala>.Fail("Datos no encontrados");
                }
                return ApiResult<Sala>.Ok(sala);
            }
            catch (Exception ex)
            {
                return ApiResult<Sala>.Fail(ex.Message);
            }
        }

        // PUT: api/Salas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Sala>>> PutSala(int id, Sala sala)
        {
            if (id != sala.Id)
            {
                return ApiResult<Sala>.Fail("No coinciden los identificadores");
            }

            _context.Entry(sala).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!SalaExists(id))
                {
                    return ApiResult<Sala>.Fail("Datos no encontrados");
                }
                else
                {
                    return ApiResult<Sala>.Fail(ex.Message);
                }
            }

            return ApiResult<Sala>.Ok(null);
        }

        // POST: api/Salas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ApiResult<Sala>>> PostSala(Sala sala)
        {
            try
            {
                _context.Salas.Add(sala);
                await _context.SaveChangesAsync();
                return ApiResult<Sala>.Ok(sala);
            }
            catch (Exception ex)
            {
                return ApiResult<Sala>.Fail(ex.Message);
            }
        }

        // DELETE: api/Salas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Sala>>> DeleteSala(int id)
        {
            try
            {
                var sala = await _context.Salas.FindAsync(id);
                if (sala == null)
                {
                    return ApiResult<Sala>.Fail("Datos no encontrados");
                }
                _context.Salas.Remove(sala);
                await _context.SaveChangesAsync();
                return ApiResult<Sala>.Ok(null);
            }
            catch (Exception ex)
            {
                return ApiResult<Sala>.Fail(ex.Message);
            }
        }

        private bool SalaExists(int id)
        {
            return _context.Salas.Any(e => e.Id == id);
        }
    }
}
