using Helper;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using TripService.FireBaseServices;
using TripService.Models;
using TripService.RabbitMQServices;
using static Helper.Catalouge;

namespace TripService.Repositories
{
    public class TripRepository : BaseRepository
    {
        private readonly IConfiguration _configuration;
        private readonly FirebaseService _fireBaseServices;
        private readonly RabbitmqProducer _rabbitmqProducer;
        //private readonly RabbitmqProducer _messageProducer;
        public TripRepository(TripDbContext context) : base(context)
        {
            _fireBaseServices = new FirebaseService();
            _rabbitmqProducer = new RabbitmqProducer();
            //_messageProducer = new RabbitmqProducer(_configuration);
        }

        public async Task<Guid> AcceptTrip(string driverId, string requestId)
        {
            TripRequest tripRequest = await context.TripRequest.FindAsync(Guid.Parse(requestId));
            if(tripRequest == null)
            {
                return Guid.Empty;
            }
            if(tripRequest.RequestStatus == Catalouge.Request.Canceled)
            {
                return Guid.Empty;
            }
            if(tripRequest.RequestStatus == Catalouge.Request.MovedToTrip)
            {
                return Guid.Empty;
            }
            tripRequest.RequestStatus = Catalouge.Request.MovedToTrip;
            Models.Trip trip = new()
            {
                TripId = Guid.NewGuid(),
                DriverId = Guid.Parse(driverId),
                PassengerId = tripRequest.PassengerId,
                StaffId = tripRequest.StaffId,
                VehicleId = Guid.NewGuid(),
                PassengerPhone = tripRequest.PassengerPhone,
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
            Models.Trip trip = await context.Trip.FindAsync(tripId);
            if (trip != null)
            {
                trip.TripStatus = Catalouge.Trip.OnTheWay;
                _fireBaseServices.UpdateOnGoingTrip(trip);
                return await context.SaveChangesAsync();
            }
            return 0;
        }

        public async Task<Models.Trip> GetTripForPassenger(Guid passengerId, Guid requestId)
        {
            Models.Trip trip = await context.Trip.FirstOrDefaultAsync(t => t.PassengerId == passengerId && t.RequestId == requestId);
            return trip;
        }

        public async Task<List<Models.Trip>> GetListTripsByDriver(Guid driverId)
        {
            List<Models.Trip> trips = await context.Trip.Where(t => t.DriverId == driverId).ToListAsync(); 
            return trips;
        }

        public async Task<List<Models.Trip>> GetListTripsByPassenger(Guid passengerId)
        {
            List<Models.Trip> trips = await context.Trip.Where(t => t.PassengerId == passengerId).ToListAsync();
            return trips;
        }

        public async Task<int> CancelTrip(Guid tripId)
        {
            Models.Trip trip = await context.Trip.FindAsync(tripId);
            trip.TripStatus = Catalouge.Trip.CanceledByDriver;
            _fireBaseServices.RemoveTrip(tripId);
            return await context.SaveChangesAsync();
        }

        public async Task<Models.Trip> GetTrip(Guid tripId)
        {
            Models.Trip trip = await context.Trip.FindAsync(tripId);
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

        public async Task<int> GetIncome(Guid driverId, string from, string to)
        {
            int totalPrice = 0;
            DateTime fromTime = DateTime.Parse(from);
            DateTime toTime = DateTime.Parse(to);
            if(fromTime == toTime)
            {
                DateTime startDateTime = DateTime.Today; //Today at 00:00:00
                DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59
                List<Models.Trip> driverTodayTrips = context.Trip.Where(t => t.DriverId == driverId && t.CompleteTime >= startDateTime && t.CompleteTime <= endDateTime && t.TripStatus == Catalouge.Trip.Done).ToList();
                foreach (var trip in driverTodayTrips)
                {
                    if (trip.Price != null)
                    {
                        totalPrice += trip.Price.Value;
                    }
                }
                return totalPrice;
            }
            List<Models.Trip> driverTrips = context.Trip.Where(t => t.DriverId == driverId && t.CompleteTime >= fromTime && t.CompleteTime <= toTime &&  t.TripStatus == Catalouge.Trip.Done).ToList();
            foreach(var trip in driverTrips)
            {
                if (trip.Price != null)
                {
                    totalPrice += trip.Price.Value;
                }
            }

            return totalPrice;
        }

        public async Task<int> CompleteTrip(Guid tripId)
        {
            Models.Trip trip = await context.Trip.FindAsync(tripId);
            if(trip == null || trip.TripStatus == Catalouge.Trip.Done)
            {
                return 0;
            }
            trip.TripStatus = Catalouge.Trip.Done;
            trip.CompleteTime = DateTime.Now;
            _fireBaseServices.RemoveTrip(tripId);
            _rabbitmqProducer.SendMessage("chat", new
            {
                Status = true,
                Message = "SaveChat",
                Data = new
                {
                    TripId = tripId.ToString(),
                },
            });
            return await context.SaveChangesAsync();
        }

        public async Task<List<Models.Trip>> GetTrips()
        {
            return await context.Trip.ToListAsync();
        }

        public async Task<int> ClearTable()
        {
            context.RemoveRange(context.Trip);
            return await context.SaveChangesAsync();
        }
    }
}
