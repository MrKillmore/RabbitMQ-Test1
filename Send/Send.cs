using System;
using RabbitMQ.Client;
using System.Text;
using Shared;
using System.Text.Json;

namespace Send {
    class Send {
        

        static void Main(string[] args)
        {
            Console.WriteLine("Sender Test-Program preparing...");
            var factory = new ConnectionFactory() { HostName = "c3po.hw-schmidt.local", UserName = "connector", Password = "connector" };
            var rand = new Random((int)DateTime.Now.Ticks);

            using (var connection = factory.CreateConnection()) {
                using (var channel = connection.CreateModel()) {
                    channel.QueueDeclare(queue: "hello",
                                                     durable: false,
                                                     exclusive: false,
                                                     autoDelete: false,
                                                     arguments: null);

                    Console.WriteLine("Sender Test-Program prepared!");
                    Console.WriteLine(" Sending 1000 messages....");

                    do {
                        for (int i = 0; i < 1000; i++) {
                            var coord = new Coordinate();
                            coord.X1 = rand.NextDouble();
                            coord.X2 = rand.NextDouble();
                            coord.Y1 = rand.NextDouble();
                            coord.Y2 = rand.NextDouble();
                            var message = JsonSerializer.Serialize(coord);
                            var body = Encoding.UTF8.GetBytes(message);
                            SendMessage(channel, body);
                        }
                    } while (Console.ReadLine() == "");
                    //do {
                    //    var body = Encoding.UTF8.GetBytes($"{ DateTime.Now.TimeOfDay.TotalSeconds}.{ DateTime.Now.TimeOfDay.TotalMilliseconds}");
                    //    SendMessage(channel, body);
                    //} while (Console.ReadLine() == "");
                }
            }
        }

        static void SendMessage(IModel channel, byte[] message)
        {
            channel.BasicPublish(exchange: "",
                                 routingKey: "hello",
                                 basicProperties: null,
                                 body: message);
            Console.WriteLine($"{DateTime.Now.TimeOfDay.TotalSeconds}.{DateTime.Now.TimeOfDay.TotalMilliseconds} : Sent {Encoding.UTF8.GetString(message)}");
        }
    }
}
