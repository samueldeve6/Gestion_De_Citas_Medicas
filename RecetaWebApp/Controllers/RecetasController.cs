using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using RecetaWebApp.DataAccess;
using RecetaWebApp.Models;

namespace RecetaWebApp.Controllers
{
    public class RecetasController : ApiController
    {
        private RecetaDbContext db = new RecetaDbContext();

        // GET: api/Recetas
        public IQueryable<Receta> GetRecetas()
        {
            return db.Recetas;
        }

        // GET: api/Recetas/5
        [ResponseType(typeof(Receta))]
        public IHttpActionResult GetReceta(int id)
        {
            Receta receta = db.Recetas.Find(id);
            if (receta == null)
            {
                return NotFound();
            }

            return Ok(receta);
        }

        // PUT: api/Recetas/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutReceta(int id, Receta receta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != receta.Id)
            {
                return BadRequest();
            }

            db.Entry(receta).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecetaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Recetas
        [ResponseType(typeof(Receta))]
        public IHttpActionResult PostReceta(Receta receta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Generar automáticamente un Código Único para la receta
            receta.Codigo = Guid.NewGuid().ToString();

            db.Recetas.Add(receta);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = receta.Id }, receta);
        }

        // DELETE: api/Recetas/5
        [ResponseType(typeof(Receta))]
        public IHttpActionResult DeleteReceta(int id)
        {
            Receta receta = db.Recetas.Find(id);
            if (receta == null)
            {
                return NotFound();
            }

            db.Recetas.Remove(receta);
            db.SaveChanges();

            return Ok(receta);
        }

        // GET: api/Recetas/PorCodigo/{codigo}
        [HttpGet]
        [Route("PorCodigo/{codigo}")]
        public IHttpActionResult GetRecetaPorCodigo(string codigo)
        {
            var receta = db.Recetas.FirstOrDefault(r => r.Codigo == codigo);
            if (receta == null)
            {
                return NotFound();
            }

            return Ok(receta);
        }

        // GET: api/Recetas/PorPaciente/{pacienteId}
        [HttpGet]
        [Route("PorPaciente/{pacienteId}")]
        public IHttpActionResult GetRecetasPorPaciente(int pacienteId)
        {
            var recetas = db.Recetas.Where(r => r.PacienteId == pacienteId).ToList();
            if (recetas == null || !recetas.Any())
            {
                return NotFound();
            }

            return Ok(recetas);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RecetaExists(int id)
        {
            return db.Recetas.Count(e => e.Id == id) > 0;
        }
    }
}