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
    public class BoletosController:ControllerBase
    {
        private readonly DbContext _context;

        public BoletosController(DbContext context)
        {
            _context = context;
        }

        // GET: api/Boletos
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Boleto>>>> GetBoletos()
        {
            try
            {
                var data = await _context.Boletos.ToListAsync();
                return ApiResult<List<Boleto>>.Ok(data);
            }
            catch(Exception ex)
            {
                return ApiResult<List<Boleto>>.Fail(ex.Message);
            }
        }

        // GET: api/Boletos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Boleto>>> GetBoleto(int id)
        {
            try
            {
                var boleto = await _context
                    .Boletos
                    .Include(b => b.Funcion)
                    .ThenInclude(f => f.Sala)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if(boleto == null)
                {
                    return ApiResult<Boleto>.Fail("Datos no encontrados");
                }

                return ApiResult<Boleto>.Ok(boleto);
            }
            catch(Exception ex)
            {
                return ApiResult<Boleto>.Fail(ex.Message);
            }
        }


        // GET: api/Boletos/5
        [HttpGet("Disponibilidad/{funcioId}")]
        public async Task<ActionResult<ApiResult<List<int>>>> GetBoletosDisponibles(int funcioId)
        {
            try
            {
                var asientosOcupados = await _context
                    .Boletos
                    .Where(b => b.FuncionId == funcioId && !b.Cancelado)
                    .Select(b => b.Asiento)
                    .ToListAsync();

                return ApiResult<List<int>>.Ok(asientosOcupados);
            }
            catch(Exception e)
            {
                return ApiResult<List<int>>.Fail(e.Message);
            }
        }



        // PUT: api/Boletos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Boleto>>> PutBoleto(int id, Boleto boleto)
        {
            if(id != boleto.Id)
            {
                return ApiResult<Boleto>.Fail("No coinciden los identificadores");
            }

            _context.Entry(boleto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException ex)
            {
                if(!BoletoExists(id))
                {
                    return ApiResult<Boleto>.Fail("Datos no encontrados");
                }
                else
                {
                    return ApiResult<Boleto>.Fail(ex.Message);
                }
            }

            return ApiResult<Boleto>.Ok(null);
        }

        // POST: api/Boletos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ApiResult<Boleto>>> PostBoleto(Boleto boleto)
        {
            try
            {
                _context.Boletos.Add(boleto);
                await _context.SaveChangesAsync();

                return ApiResult<Boleto>.Ok(boleto);
            }
            catch(Exception ex)
            {
                return ApiResult<Boleto>.Fail(ex.Message);
            }
        }

        // DELETE: api/Boletos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Boleto>>> DeleteBoleto(int id)
        {
            try
            {
                var boleto = await _context.Boletos.FindAsync(id);
                if(boleto == null)
                {
                    return ApiResult<Boleto>.Fail("Datos no encontrados");
                }

                _context.Boletos.Remove(boleto);
                await _context.SaveChangesAsync();

                return ApiResult<Boleto>.Ok(null);
            }
            catch(Exception ex)
            {
                return ApiResult<Boleto>.Fail(ex.Message);
            }
        }

        private bool BoletoExists(int id)
        {
            return _context.Boletos.Any(e => e.Id == id);
        }
    }
}
