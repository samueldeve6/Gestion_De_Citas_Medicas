using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CitaWebApp.Models
{
    public class Cita
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Lugar { get; set; }
        public int PacienteId { get; set; }
        public int MedicoId { get; set; }
        public string Estado { get; set; } // "Pendiente", "En proceso", "Finalizada"
    }
}