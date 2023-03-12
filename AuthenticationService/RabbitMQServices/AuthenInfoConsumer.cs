using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

namespace AuthenticationService.RabbitMQServices
{
    public class AuthenInfoConsumer
    {
        IConfiguration configuration;

        public AuthenInfoConsumer(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public object GetMessage()
        {
            var result = new object();
            //consumer for listen message
            var factory = new ConnectionFactory { Uri = new Uri("amqps://gtyepqer:MFoGZBk-zqtRAf8fZoKPYIdBIcQTOp8T@fly.rmq.cloudamqp.com/gtyepqer") };
            var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "authenInfo", exclusive: false);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                result = message;
            };
            return result;
        }
    }
}
