using CitaWebApp.Controllers;
using CitaWebApp.DataAccess;
using CitaWebApp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Results;
using System.Web.Http;

[TestClass]
public class CitasControllerTests
{
    private Mock<CitaDbContext> mockContext;
    private CitasController controller;
    private Mock<HttpClient> mockHttpClient;
    private Mock<RabbitMqService> mockRabbitMqService;

    [TestInitialize]
    public void Setup()
    {
        mockContext = new Mock<CitaDbContext>();
        mockHttpClient = new Mock<HttpClient>();
        mockRabbitMqService = new Mock<RabbitMqService>();

        controller = new CitasController(mockContext.Object, mockHttpClient.Object, mockRabbitMqService.Object)
        {
            Configuration = new HttpConfiguration()
        };
    }

    // Test para GetCitas
    [TestMethod]
    public void GetCitas_ReturnsAllCitas()
    {
        var citas = new List<Cita>
        {
            new Cita { Id = 1, Estado = "Pendiente", PacienteId = 1, MedicoId = 1 },
            new Cita { Id = 2, Estado = "Finalizada", PacienteId = 2, MedicoId = 2 }
        }.AsQueryable();

        var mockCitasDbSet = new Mock<DbSet<Cita>>();
        mockCitasDbSet.As<IQueryable<Cita>>().Setup(m => m.Provider).Returns(citas.Provider);
        mockCitasDbSet.As<IQueryable<Cita>>().Setup(m => m.Expression).Returns(citas.Expression);
        mockCitasDbSet.As<IQueryable<Cita>>().Setup(m => m.ElementType).Returns(citas.ElementType);
        mockCitasDbSet.As<IQueryable<Cita>>().Setup(m => m.GetEnumerator()).Returns(citas.GetEnumerator());

        mockContext.Setup(m => m.Citas).Returns(mockCitasDbSet.Object);

        var result = controller.GetCitas();
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
    }

    // Test para GetCita (por ID)
    [TestMethod]
    public void GetCita_ReturnsCitaById()
    {
        var cita = new Cita { Id = 1, Estado = "Pendiente", PacienteId = 1, MedicoId = 1 };

        mockContext.Setup(m => m.Citas.Find(1)).Returns(cita);

        var result = controller.GetCita(1);
        Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<Cita>));
    }

    [TestMethod]
    public void GetCita_ReturnsNotFoundWhenCitaDoesNotExist()
    {
        mockContext.Setup(m => m.Citas.Find(1)).Returns((Cita)null);

        var result = controller.GetCita(1);
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    // Test para PutCita (actualizar cita)
    [TestMethod]
    public void PutCita_ReturnsBadRequestWhenModelStateIsInvalid()
    {
        // Arrange
        controller.ModelState.AddModelError("key", "error");  // Agregar error al ModelState

        // Act
        var result = controller.PutCita(1, new Cita());

        // Assert
        Assert.IsInstanceOfType(result, typeof(InvalidModelStateResult));  // Asegurarse que devuelva InvalidModelStateResult
    }

    //[TestMethod]
    //public void PutCita_ReturnsNotFoundWhenCitaDoesNotExist()
    //{
    //    var cita = new Cita { Id = 999 };  // ID que no existe

    //    mockContext.Setup(m => m.Citas.Find(999)).Returns((Cita)null);

    //    var result = controller.PutCita(cita.Id, cita);

    //    // Assert
    //    Assert.IsInstanceOfType(result, typeof(NotFoundResult));  // Debe devolver NotFoundResult
    //}

    // Test para PostCita (crear cita)
    [TestMethod]
    public void PostCita_ReturnsCreated()
    {
        var cita = new Cita { Id = 1, Estado = "Pendiente", PacienteId = 1, MedicoId = 1 };

        mockContext.Setup(m => m.Citas.Add(It.IsAny<Cita>())).Returns(cita);
        mockContext.Setup(m => m.SaveChanges()).Returns(1);

        var result = controller.PostCita(cita);
        Assert.IsInstanceOfType(result, typeof(CreatedAtRouteNegotiatedContentResult<Cita>));
    }

    [TestMethod]
    public void PostCita_ReturnsBadRequestWhenModelStateIsInvalid()
    {
        // Arrange
        controller.ModelState.AddModelError("Error", "Modelo inválido");
        var cita = new Cita(); // Objeto con datos inválidos

        // Act
        var result = controller.PostCita(cita);

        // Assert
        Assert.IsInstanceOfType(result, typeof(InvalidModelStateResult));
    }

    // Test para DeleteCita (eliminar cita)
    [TestMethod]
    public void DeleteCita_ReturnsOkWhenCitaIsFound()
    {
        var cita = new Cita { Id = 1, Estado = "Pendiente", PacienteId = 1, MedicoId = 1 };
        mockContext.Setup(m => m.Citas.Find(1)).Returns(cita);

        var result = controller.DeleteCita(1);
        Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<Cita>));
    }

    [TestMethod]
    public void DeleteCita_ReturnsNotFoundWhenCitaDoesNotExist()
    {
        mockContext.Setup(m => m.Citas.Find(1)).Returns((Cita)null);

        var result = controller.DeleteCita(1);
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    // Test para UpdateEstadoCita
    [TestMethod]
    public void UpdateEstadoCita_ReturnsBadRequestWhenEstadoIsNullOrEmpty()
    {
        var result = controller.UpdateEstadoCita(1, "");
        Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));
    }

    [TestMethod]
    public void UpdateEstadoCita_ReturnsNotFoundWhenCitaDoesNotExist()
    {
        mockContext.Setup(m => m.Citas.Find(1)).Returns((Cita)null);

        var result = controller.UpdateEstadoCita(1, "Finalizada");
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    [TestMethod]
    public void UpdateEstadoCita_ReturnsNoContentWhenEstadoIsUpdated()
    {
        var cita = new Cita { Id = 1, Estado = "Pendiente", PacienteId = 1, MedicoId = 1 };
        mockContext.Setup(m => m.Citas.Find(1)).Returns(cita);

        var result = controller.UpdateEstadoCita(1, "Finalizada");
        Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
    }
}
