using PersonaWebApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PersonaWebApp.DataAccess
{
    public class PersonaDbContext : DbContext
    {
        public virtual DbSet<Persona> Personas { get; set; }

        public PersonaDbContext() : base("name=PersonasDB")
        {
        }
    }
}