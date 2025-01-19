using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RecetaWebApp.Models
{
    public class Receta
    {
        public int Id { get; set; }
        public string Codigo { get; set; } // Código único
        public int PacienteId { get; set; }
        public string Estado { get; set; } // "Activa", "Vencida", "Entregada"
    }
}