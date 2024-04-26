using Backend.Data;
using Backend.Services;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json;

namespace Backend;

public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        // Add services to the container.

        services.AddSingleton(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });

        services.AddControllers();

        services.AddEndpointsApiExplorer();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddAutoMapper(typeof(MappingProfile));

        services.AddScoped<QuizService>();

        services.AddDbContext<QuizDbContext>(options =>
        {
            options.UseSqlServer(Environment.GetEnvironmentVariable("React-Quiz"));
        });

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder
                    .WithOrigins("http://localhost:5500", "http://localhost:3000")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
        });
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseCors("CorsPolicy");

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapFallbackToFile("/index.html");
        });
    }
}
