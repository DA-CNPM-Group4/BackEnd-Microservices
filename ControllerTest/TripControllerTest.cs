using AuthenticationService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Security.Principal;
using TripService.Controllers;
using TripService.DataAccess.Interface;
using TripService.DTOs;
using TripService.Models;
using Xunit;



namespace ControllerTest
{
    public class TripControllerTest
    {
        private readonly Mock<ITripDataAccess> tripDataAccess;
        private readonly Mock<ITripRequestDataAccess> requestDataAccess;
        private readonly Mock<ITripFeedbackDataAccess> feedbackDataAccess;
        private readonly TripController tripController;
        private readonly TripRequestController tripRequestController;
        private readonly TripFeedbackController tripFeedbackController;
        private Guid userId;

        public TripControllerTest()
        {
            userId = new Guid();
            tripDataAccess = new Mock<ITripDataAccess>();
            requestDataAccess = new Mock<ITripRequestDataAccess>();
            feedbackDataAccess = new Mock<ITripFeedbackDataAccess>();
            //setup authorize
            tripController = new TripController(tripDataAccess.Object, requestDataAccess.Object, feedbackDataAccess.Object);
            tripRequestController = new TripRequestController(requestDataAccess.Object);
            tripFeedbackController = new TripFeedbackController(feedbackDataAccess.Object);
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "username"),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim("name", "John Doe"),
            };
            var identity = new ClaimsIdentity(claims, "test");
            var contextUser = new ClaimsPrincipal(identity); //add claims as needed
            var httpContext = new DefaultHttpContext()
            {
                User = contextUser,
            };
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };
            tripController.ControllerContext = controllerContext;
            tripRequestController.ControllerContext = controllerContext;
            tripFeedbackController.ControllerContext = controllerContext;
        }



        [Fact]
        public async void AcceptTripSuccess()
        {
            //arrange
            Guid driverId = Guid.NewGuid();
            Guid requestId = Guid.NewGuid();
            Guid tripId = Guid.NewGuid();
            TripRequest tripRequest = new TripRequest()
            {
                RequestId = requestId,
            };
            tripDataAccess.Setup(m => m.AcceptTrip(userId.ToString(), driverId.ToString(), requestId.ToString())).ReturnsAsync(tripId);

            //act
            AcceptTripDTO acceptTripDTO = new AcceptTripDTO()
            {
                DriverId = driverId.ToString(),
                RequestId = requestId.ToString(),
            };
            var result = await tripController.AcceptRequest(acceptTripDTO);
            //assert
            Assert.True(result.status);
            Assert.Equal(tripId, result.data);
        }

        [Fact]
        public async void AcceptTripFailed()
        {
            //arrange
            Guid driverId = Guid.NewGuid();
            Guid requestId = Guid.NewGuid();
            Guid tripId = Guid.NewGuid();
            TripRequest tripRequest = new TripRequest()
            {
                RequestId = requestId,
            };
            tripDataAccess.Setup(m => m.AcceptTrip(userId.ToString(), driverId.ToString(), requestId.ToString())).ReturnsAsync(Guid.Empty);

            //act
            AcceptTripDTO acceptTripDTO = new AcceptTripDTO()
            {
                DriverId = driverId.ToString(),
                RequestId = requestId.ToString(),
            };
            var result = await tripController.AcceptRequest(acceptTripDTO);
            //assert
            Assert.False(result.status);
            Assert.NotEqual(tripId, result.data);
            Assert.Equal("Failed to accept this request", result.message);
        }

        [Fact]
        public async void FinishTripSuccess()
        {
            //arrange
            Guid driverId = Guid.NewGuid();
            Guid requestId = Guid.NewGuid();
            Guid tripId = Guid.NewGuid();
            TripRequest tripRequest = new TripRequest()
            {
                RequestId = requestId,
            };
            tripDataAccess.Setup(m => m.CompleteTrip(userId.ToString(), tripId)).ReturnsAsync(1);

            //act
            JObject keyValuePairs = new JObject();
            keyValuePairs.Add("tripId", tripId.ToString());
            var result = await tripController.FinishTrip(keyValuePairs);
            //assert
            Assert.True(result.status);
            Assert.Equal("Trip is finished", result.message);
        }

        [Fact]
        public async void FinishTripFailed()
        {
            //arrange
            Guid driverId = Guid.NewGuid();
            Guid requestId = Guid.NewGuid();
            Guid tripId = Guid.NewGuid();
            TripRequest tripRequest = new TripRequest()
            {
                RequestId = requestId,
            };
            tripDataAccess.Setup(m => m.CompleteTrip(userId.ToString(), tripId)).ReturnsAsync(0);

            //act
            JObject keyValuePairs = new JObject();
            keyValuePairs.Add("tripId", tripId.ToString());
            var result = await tripController.FinishTrip(keyValuePairs);
            //assert
            Assert.False(result.status);
            Assert.Equal("Failed to finish trip", result.message);
        }

        [Fact]
        public async void CancelTripSuccess()
        {
            //arrange
            Guid driverId = Guid.NewGuid();
            Guid requestId = Guid.NewGuid();
            Guid tripId = Guid.NewGuid();
            TripRequest tripRequest = new TripRequest()
            {
                RequestId = requestId,
            };
            tripDataAccess.Setup(m => m.CancelTrip(userId.ToString(), tripId)).ReturnsAsync(1);

            //act
            JObject keyValuePairs = new JObject();
            keyValuePairs.Add("tripId", tripId.ToString());
            var result = await tripController.CancelTrip(keyValuePairs);
            //assert
            Assert.True(result.status);
            Assert.Equal("Cancel trip successfully", result.message);
        }

        [Fact]
        public async void CancelTripFailed()
        {
            //arrange
            Guid driverId = Guid.NewGuid();
            Guid requestId = Guid.NewGuid();
            Guid tripId = Guid.NewGuid();
            TripRequest tripRequest = new TripRequest()
            {
                RequestId = requestId,
            };
            tripDataAccess.Setup(m => m.CancelTrip(userId.ToString(), tripId)).ReturnsAsync(0);

            //act
            JObject keyValuePairs = new JObject();
            keyValuePairs.Add("tripId", tripId.ToString());
            var result = await tripController.CancelTrip(keyValuePairs);
            //assert
            Assert.False(result.status);
            Assert.Equal("Failed to cancel this trip", result.message);
        }

        [Fact]
        public async void GetPassengerTripsSuccess()
        {
            //arrange
            List<Trip> trips = new List<Trip>()
            {
                new Trip {TripId = Guid.NewGuid()},
                new Trip {TripId = Guid.NewGuid()},
                new Trip {TripId = Guid.NewGuid()}
            };
            Guid passengerId = Guid.NewGuid();
            Guid requestId = Guid.NewGuid();
            Guid tripId = Guid.NewGuid();
            TripRequest tripRequest = new TripRequest()
            {
                RequestId = requestId,
            };
            tripDataAccess.Setup(m => m.GetListTripsByPassenger(userId.ToString(), passengerId)).ReturnsAsync(trips);

            //act
            JObject keyValuePairs = new JObject();
            keyValuePairs.Add("passengerId", passengerId.ToString());
            var result = await tripController.GetPassengerTrips(keyValuePairs);
            //assert
            Assert.True(result.status);
            Assert.Equal(result.data, trips);
            Assert.Equal("Get trips by passenger successfully", result.message);
        }

        [Fact]
        public async void GetPassengerTripsFailWithWrongPassenger()
        {
            //arrange
            List<Trip> trips = new List<Trip>()
            {
                new Trip {TripId = Guid.NewGuid()},
                new Trip {TripId = Guid.NewGuid()},
                new Trip {TripId = Guid.NewGuid()}
            };
            Guid passengerId = Guid.NewGuid();
            Guid requestId = Guid.NewGuid();
            Guid tripId = Guid.NewGuid();
            TripRequest tripRequest = new TripRequest()
            {
                RequestId = requestId,
            };
            tripDataAccess.Setup(m => m.GetListTripsByPassenger(userId.ToString(), passengerId)).ReturnsAsync(new List<Trip>());

            //act
            JObject keyValuePairs = new JObject();
            keyValuePairs.Add("passengerId", passengerId.ToString());
            var result = await tripController.GetPassengerTrips(keyValuePairs);
            //assert
            Assert.False(result.status);
            Assert.Equal(result.data, trips);
            Assert.Equal("Failed to get trips by passenger", result.message);
        }

        [Fact]
        public async void GetDriverTripsSuccess()
        {
            //arrange
            List<Trip> trips = new List<Trip>()
            {
                new Trip {TripId = Guid.NewGuid()},
                new Trip {TripId = Guid.NewGuid()},
                new Trip {TripId = Guid.NewGuid()}
            };
            Guid driverId = Guid.NewGuid();
            Guid requestId = Guid.NewGuid();
            Guid tripId = Guid.NewGuid();
            TripRequest tripRequest = new TripRequest()
            {
                RequestId = requestId,
            };
            tripDataAccess.Setup(m => m.GetListTripsByPassenger(userId.ToString(), driverId)).ReturnsAsync(trips);

            //act
            JObject keyValuePairs = new JObject();
            keyValuePairs.Add("driverId", driverId.ToString());
            var result = await tripController.GetDriverTrips(keyValuePairs);
            //assert
            Assert.True(result.status);
            Assert.Equal(result.data, trips);
            Assert.Equal("Get trips by driver successfully", result.message);
        }

        [Fact]
        public async void GetDriverTripsFailWithWrongDriver()
        {
            //arrange
            List<Trip> trips = new List<Trip>()
            {
                new Trip {TripId = Guid.NewGuid()},
                new Trip {TripId = Guid.NewGuid()},
                new Trip {TripId = Guid.NewGuid()}
            };
            Guid driverId = Guid.NewGuid();
            Guid requestId = Guid.NewGuid();
            Guid tripId = Guid.NewGuid();
            TripRequest tripRequest = new TripRequest()
            {
                RequestId = requestId,
            };
            tripDataAccess.Setup(m => m.GetListTripsByDriver(userId.ToString(), driverId)).ReturnsAsync(new List<Trip>());

            //act
            JObject keyValuePairs = new JObject();
            keyValuePairs.Add("driverId", driverId.ToString());
            var result = await tripController.GetDriverTrips(keyValuePairs);
            //assert
            Assert.False(result.status);
            Assert.Equal(result.data, trips);
            Assert.Equal("Failed to get trips by driver", result.message);
        }

        [Fact]
        public async void SendRequestSuccess()
        {
            Guid pasengerId = Guid.NewGuid();
            Guid requestId = Guid.NewGuid();
            TripRequest tripRequest = new TripRequest()
            {
                PassengerId = pasengerId,
            };
            //arrange
            requestDataAccess.Setup(m => m.CreateRequest(userId.ToString(), tripRequest)).ReturnsAsync(1);
            //act

            var result = await tripRequestController.SendRequest(tripRequest);
            //assert
            Assert.True(result.status);
            Assert.Equal("Send request successfully", result.message);
        }

        [Fact]
        public async void SendRequestFailed()
        {
            Guid pasengerId = Guid.NewGuid();
            Guid requestId = Guid.NewGuid();
            TripRequest tripRequest = new TripRequest()
            {
                PassengerId = pasengerId,
            };
            //arrange
            requestDataAccess.Setup(m => m.CreateRequest(userId.ToString(), tripRequest)).ReturnsAsync(0);
            //act

            var result = await tripRequestController.SendRequest(tripRequest);
            //assert
            Assert.False(result.status);
            Assert.Null(result.data);
            Assert.Equal("Failed to send request", result.message);
        }

        [Fact]
        public async void CancelRequestSuccess()
        {
            
            Guid requestId = Guid.NewGuid();
            
            //arrange
            requestDataAccess.Setup(m => m.CancelRequest(userId.ToString(), requestId)).ReturnsAsync(1);
            //act
            JObject keyValuePairs = new JObject();
            keyValuePairs.Add("requestId", requestId.ToString());
            var result = await tripRequestController.CancelRequest(keyValuePairs);
            //assert
            Assert.True(result.status);
            Assert.Null(result.data);
            Assert.Equal("Cancel request successfully", result.message);
        }

        [Fact]
        public async void CancelRequestFail()
        {

            Guid requestId = Guid.NewGuid();

            //arrange
            requestDataAccess.Setup(m => m.CancelRequest(userId.ToString(), requestId)).ReturnsAsync(0);
            //act
            JObject keyValuePairs = new JObject();
            keyValuePairs.Add("requestId", requestId.ToString());
            var result = await tripRequestController.CancelRequest(keyValuePairs);
            //assert
            Assert.False(result.status);
            Assert.Null(result.data);
            Assert.Equal("Failed to cancel request", result.message);
        }

        [Fact]
        public async void CalcPriceSuccess()
        {
            object prices = new
            {
                Motorbike = 1.2 * 1.0 * 12000,
                Car4S = 1.2 * 1.0 * 15000,
                Car7S = 1.2 * 1.0 * 18000,
            };
            //arrange
            requestDataAccess.Setup(m => m.CalcPrice(1.2)).Returns(prices);
            //act
            
            var result = await tripRequestController.CalculatePrice(1.2);
            //assert
            Assert.True(result.status);
            Assert.Equal(result.data.ToString(), prices.ToString());
            Assert.Equal("Price base on distance and vehicle type", result.message);
        }

        [Fact]
        public async void GetTripSuccess()
        {
            //arrange
            Guid tripId = Guid.NewGuid();
            Trip trip = new Trip()
            {
                TripId = tripId,
            };
            
          
            tripDataAccess.Setup(m => m.GetTrip(userId.ToString(), tripId)).ReturnsAsync(trip);

            //act
            JObject keyValuePairs = new JObject();
            keyValuePairs.Add("tripId", tripId.ToString());
            var result = await tripController.GetCurrentTrip(keyValuePairs);
            //assert
            Assert.True(result.status);
            Assert.Equal(result.data, trip);
            Assert.Equal("Get trip successfully", result.message);
        }

        [Fact]
        public async void GetTripFailed()
        {
            //arrange
            Guid tripId = Guid.NewGuid();
            Trip trip = new Trip()
            {
                TripId = tripId,
            };


            tripDataAccess.Setup(m => m.GetTrip(userId.ToString(), tripId)).ReturnsAsync(new Trip());

            //act
            JObject keyValuePairs = new JObject();
            keyValuePairs.Add("tripId", tripId.ToString());
            var result = await tripController.GetCurrentTrip(keyValuePairs);
            //assert
            Assert.False(result.status);
            Assert.Equal("Failed to get trip", result.message);
        }

        [Fact]
        public async void GetTripForPassengerSuccess()
        {
            //arrange
            Guid passengerId = Guid.NewGuid();
            Guid requestId = Guid.NewGuid() ;
            Guid tripId = Guid.NewGuid();
            Trip trip = new Trip()
            {
                TripId = tripId,
            };

            tripDataAccess.Setup(m => m.GetTripForPassenger(userId.ToString(), passengerId, requestId)).ReturnsAsync(trip);

            //act
            JObject keyValuePairs = new JObject();
            keyValuePairs.Add("passengerId", passengerId.ToString());
            keyValuePairs.Add("requestId", requestId.ToString());
            var result = await tripController.GetCurrentTripForPassenger(keyValuePairs);
            //assert
            Assert.True(result.status);
            Assert.Equal(result.data, trip);
            Assert.Equal("Get trip successfully", result.message);
        }

        [Fact]
        public async void GetTripForPassengerFailed()
        {
            //arrange
            Guid passengerId = Guid.NewGuid();
            Guid requestId = Guid.NewGuid();
            Guid tripId = Guid.NewGuid();
            Trip trip = new Trip()
            {
                TripId = tripId,
            };

            tripDataAccess.Setup(m => m.GetTripForPassenger(userId.ToString(), passengerId, requestId)).ReturnsAsync(new Trip());

            //act
            JObject keyValuePairs = new JObject();
            keyValuePairs.Add("passengerId", passengerId.ToString());
            keyValuePairs.Add("requestId", requestId.ToString());
            var result = await tripController.GetCurrentTripForPassenger(keyValuePairs);
            //assert
            Assert.True(result.status);
            Assert.Equal("Get trip successfully", result.message);
        }

        [Fact]
        public async void PickedPassengerSuccess()
        {
            //arrange
            Guid passengerId = Guid.NewGuid();
            Guid requestId = Guid.NewGuid();
            Guid tripId = Guid.NewGuid();
            Trip trip = new Trip()
            {
                TripId = tripId,
            };

            tripDataAccess.Setup(m => m.PickedPassenger(userId.ToString(), tripId)).ReturnsAsync(1);

            //act
            JObject keyValuePairs = new JObject();
            keyValuePairs.Add("tripId", tripId.ToString());
            var result = await tripController.PickedPassenger(keyValuePairs);
            //assert
            Assert.True(result.status);
            Assert.Equal("Update status picked passenger successfully", result.message);
        }

        [Fact]
        public async void PickedPassengerFailed()
        {
            //arrange
            Guid passengerId = Guid.NewGuid();
            Guid requestId = Guid.NewGuid();
            Guid tripId = Guid.NewGuid();
            Trip trip = new Trip()
            {
                TripId = tripId,
            };

            tripDataAccess.Setup(m => m.PickedPassenger(userId.ToString(), tripId)).ReturnsAsync(0);

            //act
            JObject keyValuePairs = new JObject();
            keyValuePairs.Add("tripId", tripId.ToString());
            var result = await tripController.PickedPassenger(keyValuePairs);
            //assert
            Assert.False (result.status);
            Assert.Equal("Failed to update status picked passenger", result.message);
        }

        [Fact]
        public async void GetTripsSuccess()
        {
            //arrange
            List<Trip> trips = new List<Trip>()
            {
                new Trip(){TripId = Guid.NewGuid()},
                new Trip(){TripId = Guid.NewGuid()},
                new Trip(){TripId = Guid.NewGuid()},
            };

            tripDataAccess.Setup(m => m.GetTrips()).ReturnsAsync(trips);

            //act
            
            var result = await tripController.GetTrips();
            //assert
            Assert.True(result.status);
            Assert.Equal(trips.Count, result.data.Count);
            Assert.Equal("Get trips successfully", result.message);
        }

        [Fact]
        public async void GetDriverIncomeSuccess()
        {
            Guid driverId = Guid.NewGuid();
            string from = "20/04/2023";
            string to = "26/04/2023";
            //arrange
            tripDataAccess.Setup(m => m.GetIncome(userId.ToString(), driverId, from, to)).ReturnsAsync(50000);
            //act
            JObject keyValuePairs = new JObject();
            keyValuePairs.Add("driverId", driverId.ToString());
            keyValuePairs.Add("from", from);
            keyValuePairs.Add("to", to);
            var result = await tripController.GetIncome(keyValuePairs);
            //assert
            Assert.True(result.status);
            Assert.Equal(50000, result.data);
            Assert.Equal("Get income successfully", result.message);
        }

        [Fact]
        public async void GetDriverCompletedTripsSuccess()
        {
            Guid driverId = Guid.NewGuid();
            string from = "20/04/2023";
            string to = "26/04/2023";
            List<Trip> trips = new List<Trip>()
            {
                new Trip(){TripId = Guid.NewGuid()},
                new Trip(){TripId = Guid.NewGuid()},
                new Trip(){TripId = Guid.NewGuid()},
            };
            //arrange
            tripDataAccess.Setup(m => m.GetCompletedTrips(userId.ToString(), driverId, from, to)).ReturnsAsync(trips);
            //act
            JObject keyValuePairs = new JObject();
            keyValuePairs.Add("driverId", driverId.ToString());
            keyValuePairs.Add("from", from);
            keyValuePairs.Add("to", to);
            var result = await tripController.GetCompletedTrips(keyValuePairs);
            //assert
            Assert.True(result.status);
            Assert.Equal(trips, result.data);
            Assert.Equal("Get completed trips successfully", result.message);
        }

        [Fact]
        public async void GetDriverTripsTotalPagesSuccess()
        {
            Guid driverId = Guid.NewGuid();
            int pageSize = 15;
            //arrange
            tripDataAccess.Setup(m => m.CalcNumOfPagesForDriver(userId.ToString(), driverId, pageSize)).ReturnsAsync(pageSize);
            //act
            JObject keyValuePairs = new JObject();
            keyValuePairs.Add("driverId", driverId.ToString());
            keyValuePairs.Add("pageSize", pageSize);

            var result = await tripController.GetDriverTripTotalPages(keyValuePairs);
            //assert
            Assert.True(result.status);
            Assert.Equal(pageSize, result.data);
            Assert.Equal("Get num of pages successfully", result.message);
        }

        [Fact]
        public async void GetPassengerTripsTotalPagesSuccess()
        {
            Guid passengerId = Guid.NewGuid();
            int pageSize = 15;
            //arrange
            tripDataAccess.Setup(m => m.CalcNumOfPagesForPassenger(userId.ToString(), passengerId, pageSize)).ReturnsAsync(pageSize);
            //act
            JObject keyValuePairs = new JObject();
            keyValuePairs.Add("passengerId", passengerId.ToString());
            keyValuePairs.Add("pageSize", pageSize);

            var result = await tripController.GetPassengersTripTotalPages(keyValuePairs);
            //assert
            Assert.True(result.status);
            Assert.Equal(pageSize, result.data);
            Assert.Equal("Get num of pages successfully", result.message);
        }

        [Fact]
        public async void GetPassengerTripsPagingSuccess()
        {
            //arrange
            List<Trip> trips = new List<Trip>()
            {
                new Trip(){TripId = Guid.NewGuid()},
                new Trip(){TripId = Guid.NewGuid()},
                new Trip(){TripId = Guid.NewGuid()},
            };
            Guid passengerId = Guid.NewGuid(); 
            int pageSize = 15;
            int pageNum = 1;
            tripDataAccess.Setup(m => m.CalcNumOfPagesForPassenger(userId.ToString(), passengerId, pageSize)).ReturnsAsync(1);
            tripDataAccess.Setup(m => m.GetPassengerTripsPaging(userId.ToString(), passengerId, pageSize, pageNum)).ReturnsAsync(trips);
            //act
            JObject keyValuePairs = new JObject();
            keyValuePairs.Add("passengerId", passengerId.ToString());
            keyValuePairs.Add("pageSize", pageSize);
            keyValuePairs.Add("pageNum", pageNum);
            var result = await tripController.GetPassengersTripPaging(keyValuePairs);
            //assert
            Assert.True(result.status);
            Assert.Equal(trips, result.data);
            Assert.Equal("Get num of pages successfully", result.message);
        }

        [Fact]
        public async void GetPassengerTripsPagingDefaultSuccess()
        {
            //arrange
            List<Trip> trips = new List<Trip>()
            {
                new Trip(){TripId = Guid.NewGuid()},
                new Trip(){TripId = Guid.NewGuid()},
                new Trip(){TripId = Guid.NewGuid()},
            };
            Guid passengerId = Guid.NewGuid();
            tripDataAccess.Setup(m => m.CalcNumOfPagesForPassenger(userId.ToString(), passengerId, 15)).ReturnsAsync(1);
            tripDataAccess.Setup(m => m.GetPassengerTripsPaging(userId.ToString(), passengerId, 15, 1)).ReturnsAsync(trips);
            //act
            JObject keyValuePairs = new JObject();
            keyValuePairs.Add("passengerId", passengerId.ToString());
            var result = await tripController.GetPassengersTripPaging(keyValuePairs);
            //assert
            Assert.True(result.status);
            Assert.Equal(trips, result.data);
            Assert.Equal("Get num of pages successfully", result.message);
        }

        [Fact]
        public async void GetPassengerTripsPagingFailed()
        {
            //arrange
            List<Trip> trips = new List<Trip>()
            {
                new Trip(){TripId = Guid.NewGuid()},
                new Trip(){TripId = Guid.NewGuid()},
                new Trip(){TripId = Guid.NewGuid()},
            };
            Guid passengerId = Guid.NewGuid();
            int pageSize = 15;
            int pageNum = 2;
            tripDataAccess.Setup(m => m.CalcNumOfPagesForPassenger(userId.ToString(), passengerId, pageSize)).ReturnsAsync(1);
            tripDataAccess.Setup(m => m.GetPassengerTripsPaging(userId.ToString(), passengerId, pageSize, pageNum)).ReturnsAsync(trips);
            //act
            JObject keyValuePairs = new JObject();
            keyValuePairs.Add("passengerId", passengerId.ToString());
            keyValuePairs.Add("pageSize", pageSize);
            keyValuePairs.Add("pageNum", pageNum);
            var result = await tripController.GetPassengersTripPaging(keyValuePairs);
            //assert
            Assert.False(result.status);
            Assert.Null(result.data);
            Assert.Equal("The pageNum you input is greater than the max number of pages, try another pageSize or smaller pageNum", result.message);
        }

        [Fact]
        public async void GetDriverTripsPagingSuccess()
        {
            //arrange
            List<Trip> trips = new List<Trip>()
            {
                new Trip(){TripId = Guid.NewGuid()},
                new Trip(){TripId = Guid.NewGuid()},
                new Trip(){TripId = Guid.NewGuid()},
            };
            Guid driverId = Guid.NewGuid();
            int pageSize = 15;
            int pageNum = 1;
            tripDataAccess.Setup(m => m.CalcNumOfPagesForDriver(userId.ToString(), driverId, pageSize)).ReturnsAsync(1);
            tripDataAccess.Setup(m => m.GetDriverTripsPaging(userId.ToString(), driverId, pageSize, pageNum)).ReturnsAsync(trips);
            //act
            JObject keyValuePairs = new JObject();
            keyValuePairs.Add("driverId", driverId.ToString());
            keyValuePairs.Add("pageSize", pageSize);
            keyValuePairs.Add("pageNum", pageNum);
            var result = await tripController.GetDriverTripPageing(keyValuePairs);
            //assert
            Assert.True(result.status);
            Assert.Equal(trips, result.data);
            Assert.Equal("Get num of pages successfully", result.message);
        }

        [Fact]
        public async void GetDriverTripsPagingDefaultSuccess()
        {
            //arrange
            List<Trip> trips = new List<Trip>()
            {
                new Trip(){TripId = Guid.NewGuid()},
                new Trip(){TripId = Guid.NewGuid()},
                new Trip(){TripId = Guid.NewGuid()},
            };
            Guid driverId = Guid.NewGuid();
            tripDataAccess.Setup(m => m.CalcNumOfPagesForDriver(userId.ToString(), driverId, 15)).ReturnsAsync(1);
            tripDataAccess.Setup(m => m.GetDriverTripsPaging(userId.ToString(), driverId, 15, 1)).ReturnsAsync(trips);
            //act
            JObject keyValuePairs = new JObject();
            keyValuePairs.Add("driverId", driverId.ToString());
            var result = await tripController.GetDriverTripPageing(keyValuePairs);
            //assert
            Assert.True(result.status);
            Assert.Equal(trips, result.data);
            Assert.Equal("Get num of pages successfully", result.message);
        }

        [Fact]
        public async void GetDriverTripsPagingFailed()
        {
            //arrange
            List<Trip> trips = new List<Trip>()
            {
                new Trip(){TripId = Guid.NewGuid()},
                new Trip(){TripId = Guid.NewGuid()},
                new Trip(){TripId = Guid.NewGuid()},
            };
            Guid driverId = Guid.NewGuid();
            int pageSize = 15;
            int pageNum = 2;
            tripDataAccess.Setup(m => m.CalcNumOfPagesForDriver(userId.ToString(), driverId, pageSize)).ReturnsAsync(1);
            tripDataAccess.Setup(m => m.GetDriverTripsPaging(userId.ToString(), driverId, pageSize, pageNum)).ReturnsAsync(trips);
            //act
            JObject keyValuePairs = new JObject();
            keyValuePairs.Add("driverId", driverId.ToString());
            keyValuePairs.Add("pageSize", pageSize);
            keyValuePairs.Add("pageNum", pageNum);
            var result = await tripController.GetDriverTripPageing(keyValuePairs);
            //assert
            Assert.False(result.status);
            Assert.Null(result.data);
            Assert.Equal("The pageNum you input is greater than the max number of pages, try another pageSize or smaller pageNum", result.message);
        }

        [Theory]
        [InlineData("286474fc-21e2-4086-aa8f-fa5aaa999219", 3.3, "OKe")]
        [InlineData("090895a0-04d7-4a3a-909d-15b141ef3657", 4.0, "OKe")]
        [InlineData("525bc067-69a4-46ee-b55f-7d2e85a796ee", 0, "OKe")]
        [InlineData("48e38866-682d-4d9c-8bae-46a9d55fb93c", 4.0, "OKe")]
        [InlineData("c2433fd2-bde0-4bc3-b611-ddbfaabef922", 5.0, "OKe")]
        public async void RateTripSuccess(Guid tripId, double rate, string description)
        {
            
            RateTripDTO rateTripDTO = new RateTripDTO()
            {
                TripId = tripId.ToString(), 
                Rate = rate,
                Description = description
            };
            //arrange
            feedbackDataAccess.Setup(m => m.RateTrip(userId.ToString(), tripId, rateTripDTO.Description, rateTripDTO.Rate)).ReturnsAsync(1);
            //act
            var result = await tripFeedbackController.RateTrip(rateTripDTO);
            //assert
            Assert.True(result.status);
            Assert.Equal("Rate trip feedback successfully", result.message);
        }

        [Fact]
        public async void RateTripFailed()
        {
            Guid tripId = Guid.NewGuid();
            RateTripDTO rateTripDTO = new RateTripDTO()
            {
                TripId = tripId.ToString(),
                Rate = 4.5,
                Description = "Ok"
            };
            //arrange
            feedbackDataAccess.Setup(m => m.RateTrip(userId.ToString(), tripId, rateTripDTO.Description, rateTripDTO.Rate)).ReturnsAsync(0);
            //act
            var result = await tripFeedbackController.RateTrip(rateTripDTO);
            //assert
            Assert.False(result.status);
            Assert.Equal("Failed to rate trip", result.message);
        }

        [Fact]
        public async void GetFeedBackSuccess()
        {
            Guid tripId = Guid.NewGuid();
            TripFeedback tripFeedback = new TripFeedback()
            {
                TripId = tripId,
                Rate = 4.5,
                Note = "Ok"
            };
            //arrange
            feedbackDataAccess.Setup(m => m.GetTripFeedback(userId.ToString(), tripId)).ReturnsAsync(tripFeedback);
            //act
            JObject keyValuePairs = new JObject();
            keyValuePairs.Add("tripId", tripId.ToString());
            var result = await tripFeedbackController.GetTripFeedBack(keyValuePairs);
            //assert
            Assert.True(result.status);
            Assert.Equal(result.data, tripFeedback);
            Assert.Equal("Get trip successfully", result.message);
        }

        [Fact]
        public async void GetFeedBackFailed()
        {
            Guid tripId = Guid.NewGuid();
           
            //arrange
            feedbackDataAccess.Setup(m => m.GetTripFeedback(userId.ToString(), tripId)).ReturnsAsync(new TripFeedback());
            //act
            JObject keyValuePairs = new JObject();
            keyValuePairs.Add("tripId", tripId.ToString());
            var result = await tripFeedbackController.GetTripFeedBack(keyValuePairs);
            //assert
            TripFeedback resultData = result.data as TripFeedback;
            Assert.Equal(resultData.TripId, new TripFeedback().TripId);
        }
    }
}