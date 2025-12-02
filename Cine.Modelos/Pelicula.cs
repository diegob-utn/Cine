using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Cine.Modelos
{
    public class Pelicula
    {
        [Key] public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public int Duracion { get; set; } // en minutos
        public string Genero { get; set; } = string.Empty;
        public string Clasificacion { get; set; } = string.Empty; // Ej: PG-13
        public string Idioma { get; set; } = string.Empty;
        public string? Descripcion { get; set; }

        // Relación: una película puede tener varias funciones
        public List<Funcion>? Funciones { get; set; } = new List<Funcion>();

    }
}
