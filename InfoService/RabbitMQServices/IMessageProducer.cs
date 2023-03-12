namespace AuthenticationService.RabbitMQServices
{
    public interface IMessageProducer
    {
        void SendMessage<T>(T message);
    }
}
