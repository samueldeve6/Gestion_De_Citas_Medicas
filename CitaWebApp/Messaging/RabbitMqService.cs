using RabbitMQ.Client;
using System.Text;
using System;

public class RabbitMqService
{
    private readonly string _hostname = "localhost";
    private readonly string _exchangeName = "citas_exchange"; // Nombre del exchange
    private readonly string _routingKey = "";  // Routing key vacío, no es necesario para fanout

    public void SendMessage(string message)
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
                // Declaración de exchange como 'fanout'
                channel.ExchangeDeclare(exchange: _exchangeName, type: "fanout", durable: true);

                var body = Encoding.UTF8.GetBytes(message);

                // Publicar el mensaje
                channel.BasicPublish(exchange: _exchangeName,
                                     routingKey: _routingKey,  // No se usa routing key en fanout
                                     basicProperties: null,
                                     body: body);

                Console.WriteLine($"Mensaje enviado: {message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al enviar el mensaje: {ex.Message}");
        }
    }
}
