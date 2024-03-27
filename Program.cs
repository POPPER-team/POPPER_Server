using System.Reflection;
using Microsoft.OpenApi.Models;
using Minio;
using MongoDB.Driver;
using POPPER_Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

MongoClient mongoClient = new(builder.Configuration.GetConnectionString("MongoDb"));
builder.Services.AddSingleton(mongoClient.GetDatabase("test"));

string[] minioCS = builder.Configuration.GetConnectionString("Minio").Split(';');
IMinioClient minioClient = new MinioClient()
    .WithEndpoint(minioCS[0])
    .WithCredentials(minioCS[1], minioCS[2])
    .Build();
builder.Services.AddSingleton<IMinioClient>(minioClient);
builder.Services.AddScoped<IMinioService, MinioService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.Run();