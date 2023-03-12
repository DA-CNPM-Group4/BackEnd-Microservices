using AuthenticationService;
using AuthenticationService.RabbitMQServices;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<IMessageProducer, RabbitmqProducer>();
builder.Services.AddControllers();

//var dbHost = ".\\SQLEXPRESS";
var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
//var dbName = "dms_customer";
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
//var dbPassword = "vuhiep2k1072001";
var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");
var connectionString = $"Data Source={dbHost}; Initial Catalog = {dbName}; User ID = sa; Password = {dbPassword};TrustServerCertificate=True;";
builder.Services.AddDbContext<AuthDbContext>(p => p.UseSqlServer(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
