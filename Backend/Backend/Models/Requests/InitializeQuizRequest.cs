using System.ComponentModel.DataAnnotations;

namespace Backend.Models.Requests;

public class InitializeQuizRequest : BaseRequest
{
    [Range(1,30, ErrorMessage = "Please choose an amount of questions in the range 1-30.")]
    public required int AmountOfQuestions { get; set; }

    public string? QuestionType { get; set; }
}