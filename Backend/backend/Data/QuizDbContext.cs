using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class QuizDbContext(DbContextOptions<QuizDbContext> options) : DbContext(options)
{
    public DbSet<FourOptionQuestion> FourOptionQuestions { get; set; }
    public DbSet<PlayerStatistics> PlayerStatistics { get; set; }
    public DbSet<FloatingIds> FloatingIds { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FourOptionQuestion>()
            .ToTable(nameof(FourOptionQuestions));

        modelBuilder.Entity<PlayerStatistics>()
            .ToTable(nameof(PlayerStatistics));

        modelBuilder.Entity<FloatingIds>()
            .ToTable(nameof(FloatingIds));

        base.OnModelCreating(modelBuilder);
    }
}
