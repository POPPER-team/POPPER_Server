using AutoMapper;
using Microsoft.AspNetCore.Identity;
using POPPER_Server.Dtos;
using POPPER_Server.Models;
using POPPER_Server.Services;

namespace POPPER_Server.Helpers;

public static class SeedData
{
    public static IApplicationBuilder SeedUsers(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(app));
        using var scope = app.ApplicationServices.CreateScope();
        IServiceProvider services = scope.ServiceProvider;
        PopperdbContext context = services.GetRequiredService<PopperdbContext>();
        IMapper mapper = services.GetRequiredService<IMapper>();
        IPasswordHasher<User> passwordHasher = services.GetRequiredService<IPasswordHasher<User>>();
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