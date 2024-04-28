namespace Backend.Models.Entities;

public class PlayerStatistics
{
    public int Id { get; private set; }

    public required string Name { get; set; }

    public required int CorrectAnswers { get; set; }

    public required int TotalAmountOfQuestions { get; set; }

    public required ICollection<PlayerStatisticsFourOptionQuestion> PlayerStatisticsFourOptionQuestions { get; set; }
}
