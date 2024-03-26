using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

MongoClient mongoClient = new(builder.Configuration.GetConnectionString("MongoDb"));
builder.Services.AddSingleton(mongoClient.GetDatabase("test"));

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