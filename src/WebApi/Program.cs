using Application;
using Infrastructure;
using WebApi.Middlewares;

namespace WebApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        
        builder.Services.AddInfrastructureServices(builder.Configuration);

        builder.Services.AddJwtAuthentication(builder.Services.GetJwtSettings(builder.Configuration));

        builder.Services.AddApplicationServices();

        var app = builder.Build();

        // Database Seeder
        await app.Services.AddDatabaseInitializerAsync();

        app.UseHttpsRedirection();

        app.UseInfrastructure();

        app.UseMiddleware<ErrorHandlingMiddleware>();

        app.MapControllers();

        app.Run();
    }
}
