﻿// <auto-generated />
using Backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Backend.Migrations
{
    [DbContext(typeof(QuizDbContext))]
    [Migration("20240428091628_ConfigureManyToMany")]
    partial class ConfigureManyToMany
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.4");

            modelBuilder.Entity("Backend.Models.Entities.FloatingIds", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("FloatingIds", (string)null);
                });

            modelBuilder.Entity("Backend.Models.Entities.FourOptionQuestion", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CorrectOptionNumber")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Option1")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Option2")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Option3")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Option4")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Question")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("QuestionType")
                        .IsRequired()
                        .HasMaxLength(21)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("FourOptionQuestions", (string)null);

                    b.HasDiscriminator<string>("QuestionType").HasValue("FourOptionQuestion");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Backend.Models.Entities.PlayerStatistics", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CorrectAnswers")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("TotalAmountOfQuestions")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("PlayerStatistics", (string)null);
                });

            modelBuilder.Entity("Backend.Models.Entities.PlayerStatisticsFourOptionQuestion", b =>
                {
                    b.Property<int>("PlayerStatisticsId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("QuestionId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Order")
                        .HasColumnType("INTEGER");

                    b.HasKey("PlayerStatisticsId", "QuestionId");

                    b.HasIndex("QuestionId");

                    b.ToTable("PlayerStatisticsFourOptionQuestion");
                });

            modelBuilder.Entity("Backend.Models.Entities.QuestionTypes.ChemistryQuestion", b =>
                {
                    b.HasBaseType("Backend.Models.Entities.FourOptionQuestion");

                    b.HasDiscriminator().HasValue("Chemistry");
                });

            modelBuilder.Entity("Backend.Models.Entities.QuestionTypes.FoodQuestion", b =>
                {
                    b.HasBaseType("Backend.Models.Entities.FourOptionQuestion");

                    b.HasDiscriminator().HasValue("Food");
                });

            modelBuilder.Entity("Backend.Models.Entities.QuestionTypes.GameQuestion", b =>
                {
                    b.HasBaseType("Backend.Models.Entities.FourOptionQuestion");

                    b.HasDiscriminator().HasValue("Game");
                });

            modelBuilder.Entity("Backend.Models.Entities.QuestionTypes.GeographyQuestion", b =>
                {
                    b.HasBaseType("Backend.Models.Entities.FourOptionQuestion");

                    b.HasDiscriminator().HasValue("Geography");
                });

            modelBuilder.Entity("Backend.Models.Entities.QuestionTypes.HistoryQuestion", b =>
                {
                    b.HasBaseType("Backend.Models.Entities.FourOptionQuestion");

                    b.HasDiscriminator().HasValue("History");
                });

            modelBuilder.Entity("Backend.Models.Entities.QuestionTypes.LiteratureQuestion", b =>
                {
                    b.HasBaseType("Backend.Models.Entities.FourOptionQuestion");

                    b.HasDiscriminator().HasValue("Literature");
                });

            modelBuilder.Entity("Backend.Models.Entities.QuestionTypes.MathQuestion", b =>
                {
                    b.HasBaseType("Backend.Models.Entities.FourOptionQuestion");

                    b.HasDiscriminator().HasValue("Math");
                });

            modelBuilder.Entity("Backend.Models.Entities.QuestionTypes.MusicQuestion", b =>
                {
                    b.HasBaseType("Backend.Models.Entities.FourOptionQuestion");

                    b.HasDiscriminator().HasValue("Music");
                });

            modelBuilder.Entity("Backend.Models.Entities.QuestionTypes.SportsQuestion", b =>
                {
                    b.HasBaseType("Backend.Models.Entities.FourOptionQuestion");

                    b.HasDiscriminator().HasValue("Sports");
                });

            modelBuilder.Entity("Backend.Models.Entities.QuestionTypes.TechnologyQuestion", b =>
                {
                    b.HasBaseType("Backend.Models.Entities.FourOptionQuestion");

                    b.HasDiscriminator().HasValue("Technology");
                });

            modelBuilder.Entity("Backend.Models.Entities.PlayerStatisticsFourOptionQuestion", b =>
                {
                    b.HasOne("Backend.Models.Entities.PlayerStatistics", "PlayerStatistics")
                        .WithMany("PlayerStatisticsFourOptionQuestions")
                        .HasForeignKey("PlayerStatisticsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Backend.Models.Entities.FourOptionQuestion", "Question")
                        .WithMany("PlayerStatisticsFourOptionQuestion")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PlayerStatistics");

                    b.Navigation("Question");
                });

            modelBuilder.Entity("Backend.Models.Entities.FourOptionQuestion", b =>
                {
                    b.Navigation("PlayerStatisticsFourOptionQuestion");
                });

            modelBuilder.Entity("Backend.Models.Entities.PlayerStatistics", b =>
                {
                    b.Navigation("PlayerStatisticsFourOptionQuestions");
                });
#pragma warning restore 612, 618
        }
    }
}
