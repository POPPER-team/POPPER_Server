using Minio;
using MongoDB.Driver;
using POPPER_Server.Services;
using MySql.Data.MySqlClient;
using POPPER_Server.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//DATABASE services
MongoClient mongoClient = new(builder.Configuration.GetConnectionString("MongoDb"));
builder.Services.AddSingleton(mongoClient.GetDatabase("test"));

builder.Services.AddScoped<MySqlConnection>(_
    => new MySqlConnection(builder.Configuration.GetConnectionString("MySqlDb")));
builder.Services.AddScoped<TestContext>(_
    => new TestContext(builder.Configuration.GetConnectionString("MySqlDb")));

string[] minioCS = builder.Configuration.GetConnectionString("Minio").Split(';');
IMinioClient minioClient = new MinioClient()
    .WithEndpoint(minioCS[0])
    .WithCredentials(minioCS[1], minioCS[2])
    .Build();
builder.Services.AddSingleton(minioClient);
//OTHER SERVICES
builder.Services.AddScoped<IMinioService, MinioService>();

builder.Services.AddScoped<IUserServices, UserServices>();
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