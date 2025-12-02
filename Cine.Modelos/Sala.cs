using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Cine.Modelos
{
    public class Sala
    {
        [Key] public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Capacidad { get; set; }

        // Relación: una sala puede tener varias funciones
        public List<Funcion>? Funciones { get; set; } = new List<Funcion>();

    }
}
