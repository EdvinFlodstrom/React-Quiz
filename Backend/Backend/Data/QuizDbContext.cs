using Backend.Models;
using Backend.Models.QuestionTypes;
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
            .ToTable(nameof(FourOptionQuestions))
            .HasDiscriminator<string>("QuestionType")
            .HasValue<ChemistryQuestion>("Chemistry")
            .HasValue<FoodQuestion>("Food")
            .HasValue<GameQuestion>("Game")
            .HasValue<GeographyQuestion>("Geography")
            .HasValue<HistoryQuestion>("History")
            .HasValue<LiteratureQuestion>("Literature")
            .HasValue<MathQuestion>("Math")
            .HasValue<MusicQuestion>("Music")
            .HasValue<SportsQuestion>("Sports")
            .HasValue<TechnologyQuestion>("Technology");

        modelBuilder.Entity<PlayerStatistics>()
            .ToTable(nameof(PlayerStatistics));

        modelBuilder.Entity<FloatingIds>()
            .ToTable(nameof(FloatingIds));

        base.OnModelCreating(modelBuilder);
    }
}
