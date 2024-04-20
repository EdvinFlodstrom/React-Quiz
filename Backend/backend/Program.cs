using Backend.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}

builder.Services.AddDbContext<QuizDbContext>(options =>
{
    options.UseSqlServer(Environment.GetEnvironmentVariable("React-Quiz"));
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
