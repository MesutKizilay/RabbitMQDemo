using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMQReceiver1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory connectionFactory = new ConnectionFactory()
            {
                Uri = new Uri("amqp://guest:guest@localhost:5672"),
                ClientProvidedName = "Rabbit Receiver1 App"
            };

            IConnection connection = connectionFactory.CreateConnection();

            IModel channel = connection.CreateModel();

            string exchangeName = "DemoExchange";
            string routingKey = "demo-routing-key";
            string queueName = "DemoQueue";

            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            channel.QueueDeclare(queueName, false, false, false, null);
            channel.QueueBind(queueName, exchangeName, routingKey, null);
            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                Thread.Sleep(5000);
                var body = args.Body.ToArray();

                string message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"Message Received: {message}");

                channel.BasicAck(args.DeliveryTag, false);
            };

            string consumerTag = channel.BasicConsume(queueName, false, consumer);

            Console.ReadLine();

            channel.BasicCancel(consumerTag);

            channel.Close();
            connection.Close();
        }
    }
}