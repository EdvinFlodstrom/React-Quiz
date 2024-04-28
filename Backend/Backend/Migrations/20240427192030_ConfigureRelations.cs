using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentQuestionId",
                table: "PlayerStatistics");

            migrationBuilder.RenameColumn(
                name: "ListOfQuestions",
                table: "PlayerStatistics",
                newName: "TotalAmountOfQuestions");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FourOptionQuestionPlayerStatistics");

            migrationBuilder.RenameColumn(
                name: "TotalAmountOfQuestions",
                table: "PlayerStatistics",
                newName: "ListOfQuestions");

            migrationBuilder.AddColumn<int>(
                name: "CurrentQuestionId",
                table: "PlayerStatistics",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
