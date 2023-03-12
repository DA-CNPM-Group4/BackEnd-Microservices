using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

namespace InfoService.RabbitMQServices
{
    public class AuthenInfoConsumer: DefaultBasicConsumer
    {
        IConfiguration configuration;
        private readonly IModel _channel;
        public AuthenInfoConsumer(IModel channel, IConfiguration configuration)
        {
            this.configuration = configuration;
            this._channel = channel;
        }

        //public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        //{
        //    Console.WriteLine($"Consuming Message");
        //    Console.WriteLine(string.Concat("Message received from the exchange ", exchange));
        //    Console.WriteLine(string.Concat("Consumer tag: ", consumerTag));
        //    Console.WriteLine(string.Concat("Delivery tag: ", deliveryTag));
        //    Console.WriteLine(string.Concat("Routing tag: ", routingKey));
        //    Console.WriteLine(string.Concat("Message: ", Encoding.UTF8.GetString(body)));
        //    _channel.BasicAck(deliveryTag, false);
        //}

        public void GetMessage()
        {
            var result = new object();
            //consumer for listen message
            var factory = new ConnectionFactory { Uri = new Uri(configuration.GetValue<string>("RabbitMQCloud")) };
            var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            
            channel.QueueDeclare(queue: "authenInfo", exclusive: false);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(message);
                result = message;
            };
            channel.BasicConsume(queue: "authenInfo", autoAck: true, consumer: consumer);
        }
    }
}
