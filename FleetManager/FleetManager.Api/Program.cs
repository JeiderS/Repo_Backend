using FleetManager.Application;
using FleetManager.Infrastructure.Persistence.Mysql;
using FleetManager.Infrastructure.Persistence.Mysql.Context;
using Microsoft.EntityFrameworkCore;




var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddApplication();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddApplication()
    .AddPersistence();

// configure Conexion MySQL
var connectionString = builder.Configuration.GetConnectionString("MysqlConnection");

builder.Services.AddDbContext<DataBaseContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
