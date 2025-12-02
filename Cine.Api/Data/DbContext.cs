using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cine.Modelos;

namespace Cine
{
    public class DbContext:Microsoft.EntityFrameworkCore.DbContext
    {
        public DbContext(DbContextOptions<DbContext> options)
            : base(options)
        {
        }

        public DbSet<Cine.Modelos.Boleto> Boletos { get; set; } = default!;
        public DbSet<Cine.Modelos.Funcion> Funciones { get; set; } = default!;
        public DbSet<Cine.Modelos.Pelicula> Peliculas { get; set; } = default!;
        public DbSet<Cine.Modelos.Sala> Salas { get; set; } = default!;
    }
}
