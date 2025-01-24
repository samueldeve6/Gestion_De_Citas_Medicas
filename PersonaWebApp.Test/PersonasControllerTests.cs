using Moq;
using NUnit.Framework;
using PersonaWebApp.Controllers;
using PersonaWebApp.DataAccess;
using PersonaWebApp.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace PersonaWebApp.Tests
{
    [TestFixture]
    public class PersonasControllerTests
    {
        private PersonasController _controller;
        private Mock<PersonaDbContext> _mockContext;

        [SetUp]
        public void SetUp()
        {
            // Configuración del mock para la lista de personas
            var personas = new List<Persona>
        {
            new Persona { Id = 1, Nombre = "Juan", Apellido = "Pérez", TipoPersona = "Médico" },
            new Persona { Id = 2, Nombre = "Ana", Apellido = "Gómez", TipoPersona = "Paciente" }
        }.AsQueryable();

            // Configuración del DbSet simulado
            var mockSet = new Mock<DbSet<Persona>>();
            mockSet.As<IQueryable<Persona>>().Setup(m => m.Provider).Returns(personas.Provider);
            mockSet.As<IQueryable<Persona>>().Setup(m => m.Expression).Returns(personas.Expression);
            mockSet.As<IQueryable<Persona>>().Setup(m => m.ElementType).Returns(personas.ElementType);
            mockSet.As<IQueryable<Persona>>().Setup(m => m.GetEnumerator()).Returns(personas.GetEnumerator());
            mockSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns((object[] ids) => personas.FirstOrDefault(p => p.Id == (int)ids[0]));

            // Configuración del contexto simulado
            _mockContext = new Mock<PersonaDbContext>();
            _mockContext.Setup(c => c.Personas).Returns(mockSet.Object);

            // Instancia del controlador con el contexto simulado
            _controller = new PersonasController(_mockContext.Object);
        }

        [Test]
        public void GetPersona_IdExists_ReturnsOkResult()
        {
            // Act
            IHttpActionResult actionResult = _controller.GetPersona(1);
            var contentResult = actionResult as OkNegotiatedContentResult<Persona>;

            // Assert
            Assert.IsNotNull(contentResult, "El resultado no es un OkNegotiatedContentResult.");
            Assert.IsNotNull(contentResult.Content, "El contenido es nulo.");
            Assert.AreEqual(1, contentResult.Content.Id, "El ID no coincide.");
            Assert.AreEqual("Juan", contentResult.Content.Nombre, "El nombre no coincide.");
        }

        [Test]
        public void GetPersona_IdDoesNotExist_ReturnsNotFoundResult()
        {
            // Act
            IHttpActionResult actionResult = _controller.GetPersona(99);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        
        



        [Test]
        public void PutPersona_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var personaToUpdate = new Persona { Id = 2, Nombre = "Juan Actualizado", Apellido = "Pérez", TipoPersona = "Médico" };

            // Act
            IHttpActionResult actionResult = _controller.PutPersona(1, personaToUpdate);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(actionResult, "El resultado no es un BadRequestResult.");
        }

        [Test]
        public void PostPersona_ValidData_ReturnsCreatedResult()
        {
            // Arrange
            var newPersona = new Persona { Id = 3, Nombre = "Carlos", Apellido = "Ramírez", TipoPersona = "Paciente" };

            // Act
            IHttpActionResult actionResult = _controller.PostPersona(newPersona);
            var createdResult = actionResult as CreatedAtRouteNegotiatedContentResult<Persona>;

            // Assert
            Assert.IsNotNull(createdResult, "El resultado no es un CreatedAtRouteNegotiatedContentResult.");
            Assert.AreEqual("DefaultApi", createdResult.RouteName, "El nombre de la ruta no coincide.");
            Assert.AreEqual(3, createdResult.Content.Id, "El ID no coincide.");
            Assert.AreEqual("Carlos", createdResult.Content.Nombre, "El nombre no coincide.");
        }

        [Test]
        public void PostPersona_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Nombre", "El campo Nombre es obligatorio.");
            var newPersona = new Persona { Id = 4, Apellido = "Ramírez", TipoPersona = "Paciente" };

            // Act
            IHttpActionResult actionResult = _controller.PostPersona(newPersona);

            // Assert
            Assert.IsInstanceOf<InvalidModelStateResult>(actionResult, "El resultado no es un InvalidModelStateResult.");
        }

        [Test]
        public void DeletePersona_IdExists_ReturnsOkResult()
        {
            // Act
            IHttpActionResult actionResult = _controller.DeletePersona(1);
            var contentResult = actionResult as OkNegotiatedContentResult<Persona>;

            // Assert
            Assert.IsNotNull(contentResult, "El resultado no es un OkNegotiatedContentResult.");
            Assert.IsNotNull(contentResult.Content, "El contenido es nulo.");
            Assert.AreEqual(1, contentResult.Content.Id, "El ID no coincide.");
        }

        [Test]
        public void DeletePersona_IdDoesNotExist_ReturnsNotFound()
        {
            // Act
            IHttpActionResult actionResult = _controller.DeletePersona(99);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult, "El resultado no es un NotFoundResult.");
        }
    }
}
