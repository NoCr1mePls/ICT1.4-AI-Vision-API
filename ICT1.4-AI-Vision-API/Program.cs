using HomeTry.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using HomeTry.Data;
using HomeTry.Interfaces;

// Add services to the container.

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
		options.JsonSerializerOptions.WriteIndented = true;
	});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var sqlConnectionString = builder.Configuration.GetValue<string>("SqlConnectionString");

if (string.IsNullOrWhiteSpace(sqlConnectionString))
    throw new InvalidProgramException("Configuration variable SqlConnectionString not found");

builder.Services.AddDbContext<LitterDbContext>(options =>
	options.UseSqlServer(sqlConnectionString));

builder.Services.AddHttpContextAccessor(); 
builder.Services.AddScoped<ILitterRepository, LitterRepository>();
builder.Services.AddHttpClient();
builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => $"The API is up . Connection string found: {((!string.IsNullOrEmpty(sqlConnectionString)) ? "y" : "n")}");

app.Run();
