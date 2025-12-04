using Microsoft.EntityFrameworkCore;
using ReservationSystem.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using ReservationSystem.Domain.Entities;
using ReservationSystem.Application.Interfaces;
using ReservationSystem.Persistence.Repositories;
using ReservationSystem.Application.Services;
using Newtonsoft.Json; 
using Microsoft.OpenApi.Models; 
using ReservationSystem.Infrastructure.Services; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();

builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<ReservationService>();

builder.Services.AddHostedService<ReservationCleanupService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
