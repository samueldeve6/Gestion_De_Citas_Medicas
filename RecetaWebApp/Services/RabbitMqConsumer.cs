using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

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
                // Declaración de la cola y el exchange
                channel.ExchangeDeclare(exchange: _exchangeName, type: "direct", durable: true);
                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName, exchange: _exchangeName, routingKey: _routingKey);

                // Consumiendo los mensajes
                Console.WriteLine("Esperando mensajes...");
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"Mensaje recibido: {message}");
                };

                // Empezar a consumir
                channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

                // Mantener el consumidor activo
                Console.ReadLine();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al consumir el mensaje: {ex.Message}");
        }
    }
}
