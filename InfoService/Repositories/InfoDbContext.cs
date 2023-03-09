using InfoService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace InfoService.Repositories
{
    public class InfoDbContext: DbContext
    {

        public DbSet<Staff> Staff { get; set; }
        public DbSet<Driver> Driver { get; set; }
        public DbSet<Passenger> Passenger { get; set; }
        public DbSet<Vehicle> Vehicle { get; set; }

        //public InfoDbContext(DbContextOptions<InfoDbContext> dbContextOptions) : base(dbContextOptions)
        //{
        //    try
        //    {
        //        var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
        //        if (databaseCreator != null)
        //        {
        //            if (!databaseCreator.CanConnect()) databaseCreator.Create();
        //            if (!databaseCreator.HasTables()) databaseCreator.CreateTables();
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}
        //public InfoDbContext()
        //{
        //    try
        //    {
        //        var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
        //        if (databaseCreator != null)
        //        {
        //            if (!databaseCreator.CanConnect()) databaseCreator.Create();
        //            if (!databaseCreator.HasTables()) databaseCreator.CreateTables();
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {

            base.OnConfiguring(builder);

            //var dbHost = ".\\SQLEXPRESS";
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            //var dbName = "dms_info";
            var dbName = Environment.GetEnvironmentVariable("DB_NAME");
            //var dbPassword = "vuhiep2k1072001";
            var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");
            var connectionString = $"Data Source={dbHost}; Initial Catalog = {dbName}; User ID = sa; Password = {dbPassword};TrustServerCertificate=True;Integrated Security=False";

            //builder.UseSqlServer(connectionString, builder =>
            //{
            //    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            //});

            builder.UseSqlServer(connectionString);
        }
    }
}
