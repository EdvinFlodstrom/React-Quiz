using System.ComponentModel.DataAnnotations;

namespace Backend.Infrastructure.Models.Requests;

public class CheckAnswerRequest : BaseRequest
{
    [Range(1, 4, ErrorMessage = "Question answer must be in the range 1-4.")]
    public required int QuestionAnswer { get; set; }
}