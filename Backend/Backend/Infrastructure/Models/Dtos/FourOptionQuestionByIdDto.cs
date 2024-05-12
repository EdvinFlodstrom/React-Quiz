namespace Backend.Infrastructure.Models.Dtos;

public class FourOptionQuestionByIdDto
{
    public required string QuestionType { get; set; }

    public required string Question { get; set; }

    public required string Option1 { get; set; }

    public required string Option2 { get; set; }

    public required string Option3 { get; set; }

    public required string Option4 { get; set; }

    public required int CorrectOptionNumber { get; set; }
}
