using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using CitaWebApp.DataAccess;
using CitaWebApp.Models;


namespace CitaWebApp.Controllers
{
    public class CitasController : ApiController
    {
        private CitaDbContext db = new CitaDbContext();

        // GET: api/Citas
        public IQueryable<Cita> GetCitas()
        {
            return db.Citas;
        }

        // GET: api/Citas/5
        [ResponseType(typeof(Cita))]
        public IHttpActionResult GetCita(int id)
        {
            Cita cita = db.Citas.Find(id);
            if (cita == null)
            {
                return NotFound();
            }

            return Ok(cita);
        }

        // PUT: api/Citas/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCita(int id, Cita cita)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cita.Id)
            {
                return BadRequest();
            }

            db.Entry(cita).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CitaExists(id))
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

        // POST: api/Citas
        [ResponseType(typeof(Cita))]
        public IHttpActionResult PostCita(Cita cita)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Citas.Add(cita);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = cita.Id }, cita);
        }

        // DELETE: api/Citas/5
        [ResponseType(typeof(Cita))]
        public IHttpActionResult DeleteCita(int id)
        {
            Cita cita = db.Citas.Find(id);
            if (cita == null)
            {
                return NotFound();
            }

            db.Citas.Remove(cita);
            db.SaveChanges();

            return Ok(cita);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CitaExists(int id)
        {
            return db.Citas.Count(e => e.Id == id) > 0;
        }

        // GET: api/Citas/filtered
        [HttpGet]
        [Route("api/Citas/filtered")]
        public IQueryable<Cita> GetCitasByFilters(int? id = null, string estado = null, int? pacienteId = null, int? medicoId = null)
        {
            IQueryable<Cita> citas = db.Citas;

            if (id.HasValue)
            {
                citas = citas.Where(c => c.Id == id);
            }

            if (!string.IsNullOrEmpty(estado))
            {
                citas = citas.Where(c => c.Estado == estado);
            }

            if (pacienteId.HasValue)
            {
                citas = citas.Where(c => c.PacienteId == pacienteId);
            }

            if (medicoId.HasValue)
            {
                citas = citas.Where(c => c.MedicoId == medicoId);
            }

            return citas;
        }

        // PUT: api/Citas/updateEstado/5
        [HttpPut]
        [Route("api/Citas/updateEstado/{id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateEstadoCita(int id, string estado)
        {
            if (string.IsNullOrEmpty(estado))
            {
                return BadRequest("El estado no puede ser vacío.");
            }

            Cita cita = db.Citas.Find(id);
            if (cita == null)
            {
                return NotFound();
            }

            cita.Estado = estado;

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                return InternalServerError();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        private readonly HttpClient _httpClient;
        private readonly RabbitMqService _rabbitMqService;

        public CitasController()
        {
            _httpClient = new HttpClient();
            _rabbitMqService = new RabbitMqService();
        }

        [HttpGet]
        [Route("api/citas/ObtenerMedicos")]
        public async Task<IHttpActionResult> ObtenerMedicos()
        {
            var url = "https://localhost:44345/api/Personas/PorTipo/Medico"; // Cambia esta URL a la de tu servicio de Personas

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var medicos = await response.Content.ReadAsStringAsync();
                    return Ok(medicos); // Retorna los datos de los médicos en formato JSON
                }

                return StatusCode((System.Net.HttpStatusCode)response.StatusCode); // Devuelve el código de estado adecuado
            }
            catch (Exception ex)
            {
                return InternalServerError(ex); // Manejo de excepciones
            }
        }




        [HttpPost]
        [Route("api/citas/finalizar/{citaId}")]
        public IHttpActionResult FinalizarCita(int citaId)
        {
            try
            {
                var cita = db.Citas.FirstOrDefault(c => c.Id == citaId);

                if (cita == null)
                {
                    return NotFound();
                }

                cita.Estado = "Finalizada";
                db.SaveChanges();

                // Generar un código único para la receta
                string codigoUnico = Guid.NewGuid().ToString();

                // Crear el objeto con la información necesaria
                var recetaInfo = new
                {
                    PacienteId = cita.PacienteId,
                    Codigo = codigoUnico
                };

                // Serializar el mensaje a JSON
                string message = Newtonsoft.Json.JsonConvert.SerializeObject(recetaInfo);

                // Enviar el mensaje a RabbitMQ
                _rabbitMqService.SendMessage(message);

                return Ok($"Cita con ID {citaId} finalizada y mensaje enviado correctamente.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}