using Backend.Infrastructure.Models.Dtos;
using Backend.Services;
using MediatR;

namespace Backend.Handlers.Questions;

public class GetQuestionByIdCommand : IRequest<GetQuestionByIdCommandResponse>
{
    public required int QuestionId { get; set; }
}

public class GetQuestionByIdCommandHandler(IQuizService quizService) : IRequestHandler<GetQuestionByIdCommand, GetQuestionByIdCommandResponse>
{
    private readonly IQuizService _quizService = quizService;

    public async Task<GetQuestionByIdCommandResponse> Handle(GetQuestionByIdCommand request, CancellationToken cancellationToken)
    {
        GetQuestionByIdCommandResponse response = new();

        try
        {
            response = await _quizService.GetQuestionById(request.QuestionId);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex;
        }

        return response;
    }
}

public class GetQuestionByIdCommandResponse
{
    public FourOptionQuestionByIdDto? Question { get; set; }

    public bool Success { get; set; }

    public Exception? Error { get; set; }
}