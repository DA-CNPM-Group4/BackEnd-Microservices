using ChatService.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ChatService.Repositories
{
    public class ChatDbContext : DbContext
    {
        public DbSet<Chat> Chat { get; set; }
        public DbSet<ChatMessage> ChatMessage { get; set; }


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
            var connectionString = "workstation id=chatdb4.mssql.somee.com;packet size=4096;user id=vuhiep123_SQLLogin_1;pwd=1hvdo3zw8c;data source=chatdb4.mssql.somee.com;persist security info=False;initial catalog=chatdb4;TrustServerCertificate=True";
            builder.UseSqlServer(connectionString);
        }
    }
}
