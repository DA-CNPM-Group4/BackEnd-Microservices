using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using TripService.Models;

namespace TripService.FireBaseServices
{
    public class FirebaseService
    {
        private readonly FirebaseClient firebaseClient;
        public FirebaseService()
        {
            string firebaseSecret = "XyQNVZKtYFACTKqiepk4UZpxER3PhXEX7K1LyqLU";
            firebaseClient = new FirebaseClient(
                "https://doancnpmnhom4-6bc5e-default-rtdb.firebaseio.com",
                new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(firebaseSecret)
                }
            );

            //firebaseClient = FirebaseApp.Create(new AppOptions()
            //{
            //    Credential = GoogleCredential.FromFile("doancnpmnhom4-6bc5e-firebase-adminsdk-lt7t0-41cdf5fd48.json"),
            //    ProjectId = "doancnpmnhom4-6bc5e",
            //});
            //Console.WriteLine("init successfully");
        }

        public async void AddNewRequest(TripRequest request)
        {
            await firebaseClient.Child("requests").Child(request.RequestId.ToString).PutAsync(request);
        }

        public async void AddNewTrip(Trip newTrip)
        {
            await firebaseClient.Child("trips").Child(newTrip.TripId.ToString).PutAsync(newTrip);
        }

        public async void RemoveRequest(Guid requestId)
        {
            await firebaseClient.Child("requests").Child(requestId.ToString()).DeleteAsync();
        }

        public async void RemoveTrip(Guid tripId)
        {
            await firebaseClient.Child("trips").Child(tripId.ToString()).DeleteAsync(); 
        }
        public async void GetCurrentRequests(Guid requestId)
        {
            var requests = await firebaseClient.Child("currentRequest")
                .OrderByKey()

                .OnceAsync<TripRequest>();
            foreach (var request in requests)
            {
                Console.WriteLine(request.Object.RequestId + " " + request.Object.RequestStatus);
            }

        }

        public void RealtimeTracking()
        {
            var observable = firebaseClient.Child("currentRequest").Child("abc").Child("status").AsObservable<object>().Subscribe(x => Console.WriteLine("Change: " + x.Object));
        }
    }
}
