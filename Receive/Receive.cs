using System;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;

namespace Receive {
    class Receive {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "c3po.hw-schmidt.local", UserName = "connector", Password = "connector" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel()) {
                channel.QueueDeclare(queue: "hello",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    var coord = JsonSerializer.Deserialize<Coordinate>(message);
                    Console.WriteLine($"Received {message} - {coord.ToString()}");
                };
                channel.BasicConsume(queue: "hello",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
