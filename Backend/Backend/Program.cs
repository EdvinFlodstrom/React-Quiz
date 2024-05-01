using Backend;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Models.Entities;
using Backend.Infrastructure.Models.Requests;
using Backend.Infrastructure.Validation.ValidatorFactory;
using Backend.Infrastructure.Validation.Validators;
using Backend.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json;

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

builder.Services.AddScoped<Backend.Infrastructure.Validation.ValidatorFactory.IQuestionValidatorFactory, QuestionValidatorFactory>();
builder.Services.AddScoped<IValidator<FourOptionQuestion>, FourOptionQuestionValidator>();
builder.Services.AddScoped<IValidator<PatchQuestionRequest>, PatchQuestionRequestValidator>();

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