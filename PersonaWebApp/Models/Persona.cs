using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PersonaWebApp.Models
{
   
    public class Persona
    {


        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string TipoPersona { get; set; } // "Médico" o "Paciente"
    }
}