using RecetaWebApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RecetaWebApp.DataAccess
{
    public class RecetaDbContext: DbContext
    {
        public RecetaDbContext()
        : base("name=RecetasDB")
        {
        }
        public DbSet<Receta> Recetas { get; set; }
    }
}