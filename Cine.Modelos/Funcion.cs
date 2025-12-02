using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cine.Modelos
{
    public class Funcion
    {
        [Key] public int Id { get; set; }
        public DateTime FechaHora { get; set; }

        // Claves foráneas
        public int PeliculaId { get; set; }
        public Pelicula? Pelicula { get; set; }

        public int SalaId { get; set; }
        public Sala? Sala { get; set; }

        // Relación: una función puede tener varios boletos
        public List<Boleto>? Boletos { get; set; } = new List<Boleto>();

    }
}
