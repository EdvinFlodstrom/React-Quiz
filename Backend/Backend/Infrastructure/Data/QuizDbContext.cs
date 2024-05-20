using Backend.Infrastructure.Models.Entities;
using Backend.Infrastructure.Models.Entities.QuestionTypes;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Data;

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

        modelBuilder.Entity<FourOptionQuestion>()
            .HasIndex(q => q.Question)
            .IsUnique();

        modelBuilder.Entity<PlayerStatistics>()
            .ToTable(nameof(PlayerStatistics));

        modelBuilder.Entity<FloatingIds>()
            .ToTable(nameof(FloatingIds));

        modelBuilder.Entity<PlayerStatisticsFourOptionQuestion>()
            .HasKey(psq => new { psq.PlayerStatisticsId, psq.QuestionId });

        modelBuilder.Entity<PlayerStatisticsFourOptionQuestion>()
            .HasOne(psq => psq.PlayerStatistics)
            .WithMany(ps => ps.PlayerStatisticsFourOptionQuestions)
            .HasForeignKey(psq => psq.PlayerStatisticsId);

        modelBuilder.Entity<PlayerStatisticsFourOptionQuestion>()
            .HasOne(psq => psq.Question)
            .WithMany(q => q.PlayerStatisticsFourOptionQuestion)
            .HasForeignKey(psq => psq.QuestionId);

        base.OnModelCreating(modelBuilder);
    }
}
