using CitaWebApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CitaWebApp.DataAccess
{
    public class CitaDbContext : DbContext
    {
        public CitaDbContext()
            : base("name=CitasDB")  
        {
        }



        public virtual DbSet<Cita> Citas { get; set; }
    }
}
