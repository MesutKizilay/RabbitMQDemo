using RabbitMQ.Client;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        ConnectionFactory connectionFactory = new ConnectionFactory()
        {
            Uri = new Uri("amqp://guest:guest@localhost:5672"),
            ClientProvidedName = "Rabbit Sender App"
        };

        IConnection connection = connectionFactory.CreateConnection();

        IModel channel = connection.CreateModel();

        string exchangeName = "DemoExchange";
        string routingKey = "demo-routing-key";
        string queueName = "DemoQueue";

        channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
        channel.QueueDeclare(queueName, false, false, false, null);
        channel.QueueBind(queueName, exchangeName, routingKey, null);

        for (int i = 0; i < 100; i++)
        {
            Console.WriteLine($"Sending Message {i}");

            byte[] messageBodyBytes = Encoding.UTF8.GetBytes($"Message #{i}");
            channel.BasicPublish(exchangeName, routingKey, null, messageBodyBytes);
            Thread.Sleep(1000);
        }

        channel.Close();
        connection.Close();
    }
}