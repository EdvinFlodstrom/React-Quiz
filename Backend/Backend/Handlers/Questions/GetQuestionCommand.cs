using Backend.Infrastructure.Models.Dtos;
using Backend.Services;
using MediatR;

namespace Backend.Handlers.Questions;

public class GetQuestionCommand : IRequest<GetQuestionCommandResponse>
{
    public required string PlayerName { get; set; }
}

public class GetQuestionCommandHandler(IQuizService quizService) : IRequestHandler<GetQuestionCommand, GetQuestionCommandResponse>
{
    private readonly IQuizService _quizService = quizService;

    public Task<GetQuestionCommandResponse> Handle(GetQuestionCommand request, CancellationToken cancellationToken)
    {
        GetQuestionCommandResponse response = new();

        try
        {
            response = _quizService.GetQuestion(request.PlayerName);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex;
        }

        return Task.FromResult(response);
    }
}

public class GetQuestionCommandResponse
{
    public FourOptionQuestionDto? Question { get; set; } // 200 (OK) and not null? Quiz is not over.

    public string? Details { get; set; } // 200 (OK) and not null? Quiz is over.

    public bool Success { get; set; }

    public Exception? Error { get; set; }
}