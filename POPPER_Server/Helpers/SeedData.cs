using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Minio;
using Minio.DataModel.Args;
using POPPER_Server.Dtos;
using POPPER_Server.Models;

namespace POPPER_Server.Helpers;

public static class SeedData
{
    public static async Task<IApplicationBuilder> CreateBucketsAsync(this IApplicationBuilder app)
    {
        //TODO check if actually works
        ArgumentNullException.ThrowIfNull(app, nameof(app));
        using var scope = app.ApplicationServices.CreateScope();
        IServiceProvider services = scope.ServiceProvider;
        IMinioClient _minioClient = services.GetRequiredService<IMinioClient>();

        var bucketNames = new string[] { "posts", "profile-pictures" };
        foreach (string BucketName in bucketNames)
        {
            BucketExistsArgs? bukerArgs = new BucketExistsArgs().WithBucket(BucketName);
            bool bucketExists = await _minioClient
                .BucketExistsAsync(bukerArgs)
                .ConfigureAwait(false);
            if (!bucketExists)
            {
                MakeBucketArgs newBucket = new MakeBucketArgs().WithBucket(BucketName);
                await _minioClient.MakeBucketAsync(newBucket).ConfigureAwait(true);
            }
        }
        return app;
    }

    public static IApplicationBuilder SeedUsers(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(app));
        using var scope = app.ApplicationServices.CreateScope();
        IServiceProvider services = scope.ServiceProvider;
        PopperdbContext context = services.GetRequiredService<PopperdbContext>();
        IMapper mapper = services.GetRequiredService<IMapper>();
        IPasswordHasher<User> passwordHasher = services.GetRequiredService<IPasswordHasher<User>>();

        if (context.Users.Any())
            return app;

        List<NewUserDto> usersDtos = new List<NewUserDto>()
        {
            new NewUserDto()
            {
                Username = "Gordan",
                FirstName = "Gordan",
                LastName = "Ramzi",
                Password = "Pa$$w0rd",
                DateOfBirth = "1966/11/8",
                Email = "gramsi@kuhinja.hr"
            },
            new NewUserDto()
            {
                Username = "VelkiTomica",
                FirstName = "Tomislav",
                LastName = "Špiček",
                Password = "321Kuhaj!!",
                DateOfBirth = "1975/6/6",
                Email = "tspicek@kuhinja.hr"
            },
            new NewUserDto()
            {
                Username = "User1",
                FirstName = "First1",
                LastName = "Last1",
                Password = "Password1",
                DateOfBirth = "1990/01/01",
                Email = "user1@example.com"
            },
            new NewUserDto()
            {
                Username = "User2",
                FirstName = "First2",
                LastName = "Last2",
                Password = "Password2",
                DateOfBirth = "1990/02/02",
                Email = "user2@example.com"
            },
            new NewUserDto()
            {
                Username = "User3",
                FirstName = "First3",
                LastName = "Last3",
                Password = "Password3",
                DateOfBirth = "1990/03/03",
                Email = "user3@example.com"
            }
        };

        List<User> users = mapper.Map<List<User>>(usersDtos);
        for (int i = 0; i < usersDtos.Count; i++)
        {
            users[i].Password = passwordHasher.HashPassword(users[i], usersDtos[i].Password);
        }

        context.AddRange(users);
        context.SaveChanges();
        return app;
    }
}
