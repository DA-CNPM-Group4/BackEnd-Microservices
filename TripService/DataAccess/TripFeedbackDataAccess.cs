﻿using Helper;
using System.Security.Cryptography;
using System.Text;
using TripService.Models;

namespace TripService.DataAccess
{
    public class TripFeedbackDataAccess
    {
        private readonly List<string> _connectionStrings = new List<string>();
        public TripFeedbackDataAccess() 
        {
            var connectionStrings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _connectionStrings.Add(connectionStrings.GetConnectionString("connectionString1"));
            _connectionStrings.Add(connectionStrings.GetConnectionString("connectionString2"));
            _connectionStrings.Add(connectionStrings.GetConnectionString("connectionString3"));
        }

        private string GetConnectionString(string userId)
        {
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.ASCII.GetBytes(userId));
            var x = BitConverter.ToUInt16(hash, 0) % _connectionStrings.Count;
            return _connectionStrings[x];
        }

        public async Task<int> RateTrip(string userId, Guid tripId, string description, double rate)
        {
            using var context = new TripServiceContext(GetConnectionString(userId));
            TripFeedback tripFeedback = context.TripFeedback.FirstOrDefault(t => t.TripId == tripId);
            if (tripFeedback != null)
            {
                return 0;
            }
            TripFeedback feedback = new TripFeedback()
            {
                TripId = tripId,
                Note = description,
                Rate = rate
            };
            Trip trip = await context.Trip.FindAsync(tripId);
            if (trip == null)
            {
                return 0;
            }
            if (trip.TripStatus != Catalouge.Trip.Done)
            {
                return 0;
            }
            await context.TripFeedback.AddAsync(feedback);
            return await context.SaveChangesAsync();
        }

        public async Task<TripFeedback> GetTripFeedback(string userId, Guid tripId)
        {
            using var context = new TripServiceContext(GetConnectionString(userId));
            TripFeedback feedBack = await context.TripFeedback.FindAsync(tripId);
            return feedBack;
        }

        public async Task<int> CalcNumOfPages(string userId, int pageSize)
        {
            using var context = new TripServiceContext(GetConnectionString(userId));
            int totalRecords = context.TripFeedback.Count();
            return (int)Math.Ceiling((double)totalRecords / pageSize);
        }

        public async Task<int> ClearTable()
        {
            foreach (var connectionString in _connectionStrings)
            {
                using var dbContext = new TripServiceContext(connectionString);
                dbContext.RemoveRange(dbContext.TripFeedback);
                await dbContext.SaveChangesAsync();
            }
            return 1;
        }
    }
}
