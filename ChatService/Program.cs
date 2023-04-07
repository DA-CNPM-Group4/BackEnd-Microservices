using JwtTokenManager;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors();
builder.Services.AddJwtAuthExtension();


//builder.Services.AddCors(p => p.AddPolicy("corspolicy", build =>
//{
//    build.SetIsOriginAllowed(isOriginAllowed: _ => true).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
//}));
var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();
app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
    .WithExposedHeaders("Content-Disposition")
);
app.UseMiddleware<ApiKeyMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
