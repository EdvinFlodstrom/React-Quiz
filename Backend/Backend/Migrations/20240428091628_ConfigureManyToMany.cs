using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FourOptionQuestionPlayerStatistics");

            migrationBuilder.CreateTable(
                name: "PlayerStatisticsFourOptionQuestion",
                columns: table => new
                {
                    PlayerStatisticsId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerStatisticsFourOptionQuestion", x => new { x.PlayerStatisticsId, x.QuestionId });
                    table.ForeignKey(
                        name: "FK_PlayerStatisticsFourOptionQuestion_FourOptionQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "FourOptionQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerStatisticsFourOptionQuestion_PlayerStatistics_PlayerStatisticsId",
                        column: x => x.PlayerStatisticsId,
                        principalTable: "PlayerStatistics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStatisticsFourOptionQuestion_QuestionId",
                table: "PlayerStatisticsFourOptionQuestion",
                column: "QuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerStatisticsFourOptionQuestion");

            migrationBuilder.CreateTable(
                name: "FourOptionQuestionPlayerStatistics",
                columns: table => new
                {
                    PlayerStatisticsId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FourOptionQuestionPlayerStatistics", x => new { x.PlayerStatisticsId, x.QuestionsId });
                    table.ForeignKey(
                        name: "FK_FourOptionQuestionPlayerStatistics_FourOptionQuestions_QuestionsId",
                        column: x => x.QuestionsId,
                        principalTable: "FourOptionQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FourOptionQuestionPlayerStatistics_PlayerStatistics_PlayerStatisticsId",
                        column: x => x.PlayerStatisticsId,
                        principalTable: "PlayerStatistics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FourOptionQuestionPlayerStatistics_QuestionsId",
                table: "FourOptionQuestionPlayerStatistics",
                column: "QuestionsId");
        }
    }
}
