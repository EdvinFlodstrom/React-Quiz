namespace Backend.Models;

public class PlayerStatistics
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int CorrectAnswers { get; set; }

    public int CurrentQuestionId { get; set; }

    public int ListOfQuestions { get; set; }
}
