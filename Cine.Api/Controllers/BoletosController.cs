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
        public async Task<ActionResult<IEnumerable<Boleto>>> GetBoleto()
        {
            return await _context.Boletos.ToListAsync();
        }

        // GET: api/Boletos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Boleto>> GetBoleto(int id)
        {
            var boleto = await _context.Boletos.FindAsync(id);

            if(boleto == null)
            {
                return NotFound();
            }

            return boleto;
        }

        // PUT: api/Boletos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBoleto(int id, Boleto boleto)
        {
            if(id != boleto.Id)
            {
                return BadRequest();
            }

            _context.Entry(boleto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if(!BoletoExists(id))
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

        // POST: api/Boletos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Boleto>> PostBoleto(Boleto boleto)
        {
            _context.Boletos.Add(boleto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBoleto", new { id = boleto.Id }, boleto);
        }

        // DELETE: api/Boletos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBoleto(int id)
        {
            var boleto = await _context.Boletos.FindAsync(id);
            if(boleto == null)
            {
                return NotFound();
            }

            _context.Boletos.Remove(boleto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BoletoExists(int id)
        {
            return _context.Boletos.Any(e => e.Id == id);
        }
    }
}
