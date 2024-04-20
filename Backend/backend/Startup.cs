using Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend;

public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        // Add services to the container.

        services.AddControllers();
        services.AddEndpointsApiExplorer();

        services.AddDbContext<QuizDbContext>(options =>
        {
            options.UseSqlServer(Environment.GetEnvironmentVariable("React-Quiz"));
        });
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseRouting();
    }
}
