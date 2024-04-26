using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class UseSqlite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FloatingIds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FloatingIds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FourOptionQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Question = table.Column<string>(type: "TEXT", nullable: false),
                    Option1 = table.Column<string>(type: "TEXT", nullable: false),
                    Option2 = table.Column<string>(type: "TEXT", nullable: false),
                    Option3 = table.Column<string>(type: "TEXT", nullable: false),
                    Option4 = table.Column<string>(type: "TEXT", nullable: false),
                    CorrectOptionNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionType = table.Column<string>(type: "TEXT", maxLength: 21, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FourOptionQuestions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerStatistics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CorrectAnswers = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentQuestionId = table.Column<int>(type: "INTEGER", nullable: false),
                    ListOfQuestions = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerStatistics", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FloatingIds");

            migrationBuilder.DropTable(
                name: "FourOptionQuestions");

            migrationBuilder.DropTable(
                name: "PlayerStatistics");
        }
    }
}
