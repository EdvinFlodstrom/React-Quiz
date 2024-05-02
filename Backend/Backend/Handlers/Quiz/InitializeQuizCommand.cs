using Backend.Services;
using MediatR;

namespace Backend.Handlers.Quiz;

public class InitializeQuizCommand : IRequest<InitializeQuizCommandResponse>
{
    public required string PlayerName { get; set; }

    public required int AmountOfQuestions { get; set; }

    public string? QuestionType { get; set; }
}

public class InitializeQuizCommandHandler(IQuizService quizService) : IRequestHandler<InitializeQuizCommand, InitializeQuizCommandResponse>
{
    private readonly IQuizService _quizService = quizService;

    public async Task<InitializeQuizCommandResponse> Handle(InitializeQuizCommand request, CancellationToken cancellationToken)
    {
        InitializeQuizCommandResponse response = new();

        try
        {
            response = await _quizService.InitializeQuiz(request.PlayerName, request.AmountOfQuestions, request.QuestionType);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex;
        }

        return response;
    }
}

public class InitializeQuizCommandResponse
{
    public string? Details { get; set; }

    public bool Success { get; set; }

    public Exception? Error { get; set; }
}
