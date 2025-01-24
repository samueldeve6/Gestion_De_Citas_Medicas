using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using RecetaWebApp.DataAccess;
using RecetaWebApp.Models;

public class RabbitMqConsumer
{
    private readonly string _hostname = "localhost";
    private readonly string _exchangeName = "citas_exchange"; // Nombre del exchange
    private readonly string _routingKey = "cita_finalizada"; // Routing key

    public void StartConsuming()
    {
        try
        {
            var factory = new ConnectionFactory()
            {
                HostName = _hostname,
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // Declarar la cola si no existe (esto evita problemas de configuración)
                var queueName = "creacion_receta";
                channel.QueueDeclare(queue: queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                // Vincular la cola al exchange con la routing key
                channel.ExchangeDeclare(exchange: _exchangeName, type: "fanout", durable: true);
                channel.QueueBind(queue: queueName, exchange: _exchangeName, routingKey: _routingKey);

                Console.WriteLine("Esperando mensajes...");
                var consumer = new EventingBasicConsumer(channel);

                // Evento que se activa al recibir un mensaje
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"Mensaje recibido: {message}");

                    try
                    {
                        // Deserializar el mensaje recibido
                        var recetaInfo = JsonConvert.DeserializeObject<RecetaInfo>(message);
                        Console.WriteLine($"Deserializado: PacienteId = {recetaInfo.PacienteId}, Codigo = {recetaInfo.Codigo}");

                        // Crear la receta en la base de datos
                        CrearRecetaEnBaseDeDatos(recetaInfo);

                        // Confirmar mensaje procesado
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al procesar el mensaje: {ex.Message}");
                        Console.WriteLine($"StackTrace: {ex.StackTrace}");

                        // Rechazar el mensaje (no reenviarlo a otra cola)
                        channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                    }
                };

                // Consumir mensajes de la cola
                channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
                Console.ReadLine();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al consumir el mensaje: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
        }
    }

    private void CrearRecetaEnBaseDeDatos(RecetaInfo recetaInfo)
    {
        try
        {
            using (var context = new RecetaDbContext())
            {
                var receta = new Receta
                {
                    PacienteId = recetaInfo.PacienteId,
                    Codigo = recetaInfo.Codigo,
                    Estado = "Activa"
                };

                context.Recetas.Add(receta);
                context.SaveChanges(); // Guardar en la base de datos

                Console.WriteLine("Receta creada correctamente.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al crear la receta en la base de datos: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
        }
    }
}

public class RecetaInfo
{
    public int PacienteId { get; set; }
    public string Codigo { get; set; }
}
