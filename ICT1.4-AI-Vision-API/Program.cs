using SensoringApi.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using SensoringApi.Data;
using SensoringApi.Interfaces;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using System;

// Add services to the container.

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var sqlConnectionString = builder.Configuration.GetValue<string>("SqlConnectionString");

builder.Services.AddDbContext<LitterDbContext>(options =>
    options.UseSqlServer(sqlConnectionString));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ILitterRepository, LitterRepository>();
builder.Services.AddHttpClient();

var app = builder.Build();

// Global error handling
app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (contextFeature != null)
        {
            var errorResponse = new
            {
                Message = "Er is een fout opgetreden tijdens de verwerking van het verzoek.",
                Detail = contextFeature.Error.Message
            };

            var errorJson = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(errorJson);
        }
    });
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseHsts();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => $"The API is up. Connection string found: {((!string.IsNullOrEmpty(sqlConnectionString)) ? "y" : "n")}");

app.Run();
