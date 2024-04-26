using Backend.Data;
using Backend.Services;
using Backend;
using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddDbContext<QuizDbContext>(options =>
{
    options.UseSqlite(Environment.GetEnvironmentVariable("SQLite-React-Quiz"));
});

builder.Services.AddSingleton(new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
});

builder.Services.AddScoped<QuizService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
            .WithOrigins("http://localhost:5500", "http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

// Add other service configurations
var app = builder.Build();

// Configure middleware
app.UseCors("CorsPolicy");
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

// Other middleware options

// Configure endpoints
app.MapControllers();

// Other endpoint mappings


app.Run();