using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Cine.Modelos
{
    public class Boleto
    {
        [Key] public int Id { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public int Asiento { get; set; }

        // Clave foránea
        public int FuncionId { get; set; }
        public Funcion? Funcion { get; set; }


    }
}
