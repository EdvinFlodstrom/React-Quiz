namespace Backend.Infrastructure.Models.Responses;

public class CheckAnswerResponse
{
    public required string Message { get; set; }

    public required bool Correct { get; set; }
}
