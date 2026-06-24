using EventosVivos.API.BL;
using EventosVivos.API.BL.Interfaces;
using EventosVivos.API.DAO;
using EventosVivos.API.DAO.Interfaces;
using EventosVivos.API.Data;
using EventosVivos.API.Middlewares;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("EventosVivosDB"));

// DAOs
builder.Services.AddScoped<IVenueDAO, VenueDAO>();
builder.Services.AddScoped<IEventDAO, EventDAO>();
builder.Services.AddScoped<IReservationDAO, ReservationDAO>();

// BLs
builder.Services.AddScoped<IEventBL, EventBL>();
builder.Services.AddScoped<IReservationBL, ReservationBL>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngular");
app.UseAuthorization();
app.MapControllers();

app.Run();