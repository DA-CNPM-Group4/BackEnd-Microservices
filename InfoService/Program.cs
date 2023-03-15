using AuthenticationService.RabbitMQServices;
using Helper;
using InfoService.Models;
using InfoService.RabbitMQServices;
using InfoService.Repositories;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddTransient<IMessageProducer, RabbitmqProducer>();

//Repository declare
//ServiceRepository serviceRepository = new ServiceRepository();

////Declare rabbitmq consumer
var factory = new ConnectionFactory { Uri = new Uri("amqps://gtyepqer:MFoGZBk-zqtRAf8fZoKPYIdBIcQTOp8T@fly.rmq.cloudamqp.com/gtyepqer") };
var connection = factory.CreateConnection();
using var channel = connection.CreateModel();
channel.ExchangeDeclare(exchange: "info", type: ExchangeType.Direct);
channel.QueueDeclare(queue: "info", exclusive: false);
channel.QueueBind(queue: "info", exchange: "info", routingKey: "info");
RabbitmqConsumer rabbitmqConsumer = new RabbitmqConsumer(channel);
channel.BasicConsume(queue: "info", autoAck: true, consumer: rabbitmqConsumer);


//var consumer = new EventingBasicConsumer(channel);
//consumer.Received += async (model, eventArgs) =>
//{
//    var body = eventArgs.Body.ToArray();
//    var message = Encoding.UTF8.GetString(body);
//    JObject jsonMessage = JObject.Parse(message.ToString());
//    string status = (string)jsonMessage["Status"];
//    string messageReceveid = (string)jsonMessage["Message"];
//    JObject data = (JObject)jsonMessage["Data"];
//    if(messageReceveid == "AddDataInfo")
//    {
//        string Role = (string)data["Role"];
//        if(Role == Catalouge.Role.Staff)
//        {
//            Staff newUser = new Staff
//            {
//                AccountId = (Guid)data["AccountId"],
//                Email = (string)data["Email"],
//                Phone = (string)data["Phone"],
//                Name = (string)data["Name"],
//            };
//            await serviceRepository.Staff.AddStaffInfo(newUser);
//        }
//        else if(Role == Catalouge.Role.Passenger)
//        {
//            Passenger newUser = new Passenger
//            {
//                AccountId = (Guid)data["AccountId"],
//                Email = (string)data["Email"],
//                Phone = (string)data["Phone"],
//                Name = (string)data["Name"],
//            };
//            await serviceRepository.Passenger.AddPassengerInfo(newUser);
//        }
//        else if(Role == Catalouge.Role.Driver)
//        {
//            Driver newUser = new Driver
//            {
//                AccountId = (Guid)data["AccountId"],
//                Email = (string)data["Email"],
//                Phone = (string)data["Phone"],
//                Name = (string)data["Name"],
//            };
//            await serviceRepository.Driver.AddDriverInfo(newUser);
//        }
//    }
//};
//channel.BasicConsume(queue: "authenInfo", autoAck: true, consumer: consumer);
var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
