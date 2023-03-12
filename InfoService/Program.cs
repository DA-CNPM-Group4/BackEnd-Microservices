using AuthenticationService.RabbitMQServices;
using InfoService.Models;
using InfoService.RabbitMQServices;
using InfoService.Repositories;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddTransient<IMessageProducer, RabbitmqProducer>();

//Repository declare
ServiceRepository serviceRepository = new ServiceRepository();

//Declare rabbitmq consumer
var factory = new ConnectionFactory { Uri = new Uri("amqps://gtyepqer:MFoGZBk-zqtRAf8fZoKPYIdBIcQTOp8T@fly.rmq.cloudamqp.com/gtyepqer") };
var connection = factory.CreateConnection();
using var channel = connection.CreateModel();
channel.ExchangeDeclare(exchange: "info", type: ExchangeType.Direct);
channel.QueueDeclare(queue: "authenInfo", exclusive: false);
channel.QueueBind(queue: "authenInfo", exchange: "info", routingKey: "authenInfo");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    JObject jsonMessage = JObject.Parse(message.ToString());
    Console.WriteLine(jsonMessage);
    string status = (string)jsonMessage["Status"];
    string message1 = (string)jsonMessage["Message"];
    JObject data = (JObject)jsonMessage["Data"];
    Driver newDriver = new Driver
    {
        AccountId = new Guid(),
        Email = (string)data["Email"],
        Phone = (string)data["Phone"],
        Name = (string)data["Name"],
    };
    serviceRepository.Driver.CreateDriver(newDriver);
    //serviceRepository.Driver.CreateDriver();
    //result = message;
};
channel.BasicConsume(queue: "authenInfo", autoAck: true, consumer: consumer);
var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
