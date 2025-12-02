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
        public async Task<ActionResult<IEnumerable<Funcion>>> GetFuncion()
        {
            return await _context.Funciones.ToListAsync();
        }

        // GET: api/Funciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Funcion>> GetFuncion(int id)
        {
            var funcion = await _context.Funciones.FindAsync(id);

            if (funcion == null)
            {
                return NotFound();
            }

            return funcion;
        }

        // PUT: api/Funciones/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFuncion(int id, Funcion funcion)
        {
            if (id != funcion.Id)
            {
                return BadRequest();
            }

            _context.Entry(funcion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FuncionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Funciones
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Funcion>> PostFuncion(Funcion funcion)
        {
            _context.Funciones.Add(funcion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFuncion", new { id = funcion.Id }, funcion);
        }

        // DELETE: api/Funciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFuncion(int id)
        {
            var funcion = await _context.Funciones.FindAsync(id);
            if (funcion == null)
            {
                return NotFound();
            }

            _context.Funciones.Remove(funcion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FuncionExists(int id)
        {
            return _context.Funciones.Any(e => e.Id == id);
        }
    }
}
