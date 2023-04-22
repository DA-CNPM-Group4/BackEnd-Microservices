using AuthenticationService.Models;
using Helper.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationService.Repositories
{
    public class AuthenticationDbContext:DbContext
    {
        public DbSet<AuthenticationInfo> AuthenticationInfo { get; set; }
        public DbSet<EmailSender> EmailSender { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.HasDefaultSchema("public");
        //    base.OnModelCreating(modelBuilder);
        //}
            
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
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
            builder.UseNpgsql(connectionString);
        }
    }
}
