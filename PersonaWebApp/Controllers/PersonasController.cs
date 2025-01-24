using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using PersonaWebApp.DataAccess;
using PersonaWebApp.Models;

namespace PersonaWebApp.Controllers
{
    public class PersonasController : ApiController
    {

        private readonly PersonaDbContext db;

        // Constructor con inyección de dependencias y valores por defecto
        public PersonasController(PersonaDbContext context = null)
        {
            db = context ?? new PersonaDbContext();  // Si no se inyecta, crea una nueva instancia de PersonaDbContext
        }

        // GET: api/Personas
        public IQueryable<Persona> GetPersonas()
        {
            return db.Personas;
        }

        // GET: api/Personas/5
        [ResponseType(typeof(Persona))]
        public IHttpActionResult GetPersona(int id)
        {
            Persona persona = db.Personas.Find(id);
            if (persona == null)
            {
                return NotFound();
            }

            return Ok(persona);
        }

        // PUT: api/Personas/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPersona(int id, Persona persona)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != persona.Id)
            {
                return BadRequest();
            }

            db.Entry(persona).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonaExists(id))
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

        // POST: api/Personas
        [ResponseType(typeof(Persona))]
        public IHttpActionResult PostPersona(Persona persona)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Personas.Add(persona);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = persona.Id }, persona);
        }

        // DELETE: api/Personas/5
        [ResponseType(typeof(Persona))]
        public IHttpActionResult DeletePersona(int id)
        {
            Persona persona = db.Personas.Find(id);
            if (persona == null)
            {
                return NotFound();
            }

            db.Personas.Remove(persona);
            db.SaveChanges();

            return Ok(persona);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PersonaExists(int id)
        {
            return db.Personas.Count(e => e.Id == id) > 0;
        }

        [HttpGet]
        [Route("api/Personas/PorTipo/{tipo}")]
        public HttpResponseMessage GetPersonasPorTipo(string tipo)
        {
            try
            {
                var personas = db.Personas.Where(p => p.TipoPersona == tipo).ToList();

                if (personas == null || !personas.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No se encontraron personas del tipo especificado.");
                }

                return Request.CreateResponse(HttpStatusCode.OK, personas);
            }
            catch (EntityCommandExecutionException ex)
            {
                // Puedes registrar o devolver la excepción interna para obtener más detalles
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.InnerException?.Message ?? ex.Message);
            }
        }


    }
}