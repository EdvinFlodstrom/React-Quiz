using Backend.Services;
using MediatR;

namespace Backend.Handlers.Questions;

public class CheckAnswerCommand : IRequest<CheckAnswerCommandResponse>
{
    public required string PlayerName { get; set; }

    public required int QuestionAnswer { get; set; }
}

public class CheckAnswerCommandHandler(IQuizService quizService) : IRequestHandler<CheckAnswerCommand, CheckAnswerCommandResponse>
{
    private readonly IQuizService _quizService = quizService;

    public async Task<CheckAnswerCommandResponse> Handle(CheckAnswerCommand request, CancellationToken cancellationToken)
    {
        CheckAnswerCommandResponse response = new();

        try
        {
            response = await _quizService.CheckAnswer(request.PlayerName, request.QuestionAnswer);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex;
        }

        return response;
    }
}

public class CheckAnswerCommandResponse
{
    public string? Message { get; set; }

    public bool Success { get; set; }

    public Exception? Error { get; set; }
}
