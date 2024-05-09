using System.ComponentModel.DataAnnotations;

namespace Backend.Infrastructure.Models.Requests;

public class CheckAnswerRequest : BaseRequest
{
    [Range(0, 4, ErrorMessage = "Question answer must be in the range 0-4. 0 always returns incorrect.")]
    public required int QuestionAnswer { get; set; }
}