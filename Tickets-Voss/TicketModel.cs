using System;
using System.ComponentModel.DataAnnotations;

namespace Tickets_Voss
{
    public class TicketModel
    {
        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [Required]
        public string Departamento { get; set; }

        public string Telefono { get; set; }

        [Required]
        [EmailAddress]
        public string Correo { get; set; }

        [Required]
        public string Categoria { get; set; }

        [Required]
        public string Descripcion { get; set; }

        public string ArchivoNombre { get; set; }
    }
}
