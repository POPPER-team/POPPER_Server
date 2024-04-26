using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Minio;
using MongoDB.Driver;
using POPPER_Server.Services;
using POPPER_Server.Helpers;
using POPPER_Server.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

MongoClient mongoClient = new(builder.Configuration.GetConnectionString("MongoDb"));
builder.Services.AddSingleton(mongoClient.GetDatabase("test"));

builder.Services.AddSingleton<PopperdbContext>(_
    => new PopperdbContext(builder.Configuration.GetConnectionString("MySqlDb")));

string[] minioCS = builder.Configuration.GetConnectionString("Minio").Split(';');
IMinioClient minioClient = new MinioClient()
    .WithEndpoint(minioCS[0])
    .WithCredentials(minioCS[1], minioCS[2])
    .Build();
builder.Services.AddSingleton(minioClient);

//Services
builder.Services.AddAutoMapper(typeof(MapperProfile));

// Configure JWT security services
string? secureKey = builder.Configuration["JWT:SecureKey"];
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        byte[] Key = Encoding.UTF8.GetBytes(secureKey);
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(Key)
        };
    });

//Scoped services
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IMinioService, MinioService>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddTransient<ISessionService, SessionService>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "POPPER_Server", Version = "v1" });
    option.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter valid JWT",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });

    option.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new List<string>()
            }
        });
});


WebApplication? app = builder.Build();

TokenHelper.ProvideService(app.Services);
UserHelper.ProvideService(app.Services);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.SeedUsers();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
