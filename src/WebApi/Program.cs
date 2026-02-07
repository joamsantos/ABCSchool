
using Infrastructure;

namespace WebApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        builder.Services.AddInfrastructureServices(builder.Configuration);

        var app = builder.Build();

        // Database Seeder
        await app.Services.AddDatabaseInitializerAsync();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseInfrastructure();

        app.MapControllers();

        app.Run();
    }
}
