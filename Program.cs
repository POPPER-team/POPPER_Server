using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Minio;
using MongoDB.Driver;
using POPPER_Server.Services;
using POPPER_Server.Helpers;
using POPPER_Server.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string? secureKey = builder.Configuration["JWT:SecureKey"];
string[] minioCS = builder.Configuration.GetConnectionString("Minio").Split(';');

//Services
builder.Services.AddAutoMapper(typeof(MapperProfile));
builder.Services.AddScoped<IMongoDatabase>(_ =>
    new MongoClient(builder.Configuration.GetConnectionString("MongoDb")).GetDatabase("Popper_session"));
builder.Services.AddDbContext<PopperdbContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("MySqlDb")));
builder.Services.AddMinio(options => options
    .WithEndpoint(minioCS[0])
    .WithCredentials(minioCS[1], minioCS[2])
    .Build()
);
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

//User added services
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IMinioService, MinioService>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<IFollowService, FollowService>();

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

IServiceProvider services = app.Services.CreateScope().ServiceProvider;
TokenHelper.ProvideService(services);
UserHelper.ProvideService(services);
//TODO test apis with new config for services
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