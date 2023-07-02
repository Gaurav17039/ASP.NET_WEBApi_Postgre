using CRUD_Migration_Logging_XunitTesting.Models;
using Microsoft.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CRUD_Migration_Logging_XunitTesting;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using CRUD_Migration_Logging_XunitTesting.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ShoppingContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("MyShoppingCartDB")));


builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//////////////////////

var key = "lectureTest1234$$$";

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddSingleton<JwtAuthenticationManager>(new JwtAuthenticationManager(key));
builder.Services.AddLogging();
builder.Services.AddTransient<GlobalExceptionHandling>();

//////////////////////

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<GlobalExceptionHandling>();

app.MapControllers();

app.Run();


public partial class Program
{
    public static void Main(string[] args)
    {
        CreateWebHostBuilder(args).Build().Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
        .ConfigureLogging(logBuilder =>
        {
            logBuilder.ClearProviders(); // removes all providers from LoggerFactory
            logBuilder.AddConsole();
            logBuilder.AddTraceSource("Information, ActivityTracing"); // Add Trace listener provider
        });
}
