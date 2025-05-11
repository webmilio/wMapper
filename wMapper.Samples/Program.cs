using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.Design;
using System.Text.Json;

namespace wMapper.Samples;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        var rng = new Random();
        
        var services = new ServiceContainer();
        var mapper = 
            new MapperBuilder(services)
                .Register(new UserMapper())
                .Build();

        var userDbo = new UserDbo()
        {
            Id = rng.Next(),

            Username = "jdoe",
            Email = "john.doe@example.com",

            Password = "superultrasecretpassword",

            CreatedOn = DateTime.Now
        };

        var userDto = mapper.Adapt<UserDto>(userDbo);

        Console.WriteLine(JsonSerializer.Serialize(userDto, new JsonSerializerOptions()
        {
            WriteIndented = true
        }));
    }
}
