using Helper;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using TripService.FireBaseServices;
using TripService.Models;
using TripService.RabbitMQServices;

namespace TripService.Repositories
{
    public class TripRepository : BaseRepository
    {
        private readonly IConfiguration _configuration;
        private readonly FirebaseService _fireBaseServices;
        //private readonly RabbitmqProducer _messageProducer;
        public TripRepository(TripDbContext context) : base(context)
        {
            _fireBaseServices = new FirebaseService();
            //_messageProducer = new RabbitmqProducer(_configuration);
        }

        public async Task<Guid> AcceptTrip(string driverId, string requestId)
        {
            TripRequest tripRequest = await context.TripRequest.FindAsync(Guid.Parse(requestId));
            if(tripRequest == null)
            {
                return Guid.Empty;
            }
            if(tripRequest.RequestStatus == Catalouge.Request.MovedToTrip)
            {
                return Guid.Empty;
            }
            tripRequest.RequestStatus = Catalouge.Request.MovedToTrip;
            Trip trip = new()
            {
                TripId = Guid.NewGuid(),
                DriverId = Guid.Parse(driverId),
                PassengerId = tripRequest.PassengerId,
                StaffId = tripRequest.StaffId,
                VehicleId = Guid.NewGuid(),
                CreatedTime = DateTime.UtcNow,
                Destination = tripRequest.Destination,
                LatDesAddr = tripRequest.LatDesAddr,
                LongDesAddr = tripRequest.LongDesAddr,
                StartAddress = tripRequest.StartAddress,
                LatStartAddr = tripRequest.LatStartAddr,
                LongStartAddr = tripRequest.LongStartAddr,
                TripStatus = Catalouge.Trip.PickingUpCus,
                Distance = tripRequest.Distance,
                Price = tripRequest.Price,
                VehicleType = tripRequest.VehicleType,
                RequestId = tripRequest.RequestId,
            };
            await context.Trip.AddAsync(trip);
            _fireBaseServices.AddNewTrip(trip);
            _fireBaseServices.RemoveRequest(Guid.Parse(requestId));
            await context.SaveChangesAsync();
            return trip.TripId;
        }

        public async Task<int> PickedPassenger(Guid tripId)
        {
            Trip trip = await context.Trip.FindAsync(tripId);
            if (trip != null)
            {
                trip.TripStatus = Catalouge.Trip.OnTheWay;
                _fireBaseServices.UpdateOnGoingTrip(trip);
                return await context.SaveChangesAsync();
            }
            return 0;
        }

        public async Task<Trip> GetTripForPassenger(Guid passengerId, Guid requestId)
        {
            Trip trip = await context.Trip.FirstOrDefaultAsync(t => t.PassengerId == passengerId && t.RequestId == requestId);
            return trip;
        }

        public async Task<Trip> GetTrip(Guid tripId)
        {
            Trip trip = await context.Trip.FindAsync(tripId);
            //_messageProducer.SendMessage("info", new
            //{
            //    Status = true,
            //    Message = "GetTripInfo",
            //    Data = new
            //    {
            //        PassengerId = trip.PassengerId,
            //        DriverId = trip.DriverId,
            //        StaffId = trip.StaffId,
            //        VehicleId = trip.VehicleId,
            //    }
            //});
            ////Declare rabbitmq consumer
            //var factory = new ConnectionFactory { Uri = new Uri("amqps://gtyepqer:MFoGZBk-zqtRAf8fZoKPYIdBIcQTOp8T@fly.rmq.cloudamqp.com/gtyepqer") };
            //var connection = factory.CreateConnection();
            //using var channel = connection.CreateModel();
            //channel.ExchangeDeclare(exchange: "info", type: ExchangeType.Direct);
            //channel.QueueDeclare(queue: "info", exclusive: false);
            //channel.QueueBind(queue: "info", exchange: "info", routingKey: "info");
            //RabbitmqConsumer rabbitmqConsumer = new RabbitmqConsumer(channel);
            //channel.BasicConsume(queue: "info", autoAck: true, consumer: rabbitmqConsumer);
            return trip;
        }

        public async Task<int> CompleteTrip(Guid tripId)
        {
            Trip trip = await context.Trip.FindAsync(tripId);
            trip.TripStatus = Catalouge.Trip.Done;
            _fireBaseServices.RemoveTrip(tripId);
            return await context.SaveChangesAsync();
        }

        public async Task<List<Trip>> GetTrips()
        {
            return await context.Trip.ToListAsync();
        }
    }
}
