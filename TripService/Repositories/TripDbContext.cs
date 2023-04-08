using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TripService.Models;

namespace TripService.Repositories
{
    public class TripDbContext : DbContext
    {
        public DbSet<Trip> Trip { get; set; }
        public DbSet<TripFeedback> TripFeedback { get; set; }
        public DbSet<TripRequest> TripRequest { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);

            ////var dbHost = ".\\SQLEXPRESS";
            //var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            ////var dbName = "dms_info";
            //var dbName = Environment.GetEnvironmentVariable("DB_NAME");
            ////var dbPassword = "vuhiep2k1072001";
            //var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");
            //var connectionString = $"Data Source={dbHost}; Initial Catalog = {dbName}; User ID = sa; Password = {dbPassword};TrustServerCertificate=True;Integrated Security=False";

            //builder.UseSqlServer(connectionString, builder =>
            //{
            //    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            //});

            var connectionString = "";
            builder.UseSqlServer(connectionString);
        }
    }
}
