using Backend.Infrastructure.Models.Entities;
using Backend.Infrastructure.Models.Requests;
using Backend.Services;
using MediatR;

namespace Backend.Handlers.Questions;

public class PatchQuestionCommand : IRequest<PatchQuestionCommandResponse>
{
    public required int QuestionId { get; set; }

    public required PatchQuestionRequest Request { get; set; }
}

public class PatchQuestionCommandHandler(QuizService quizService) : IRequestHandler<PatchQuestionCommand, PatchQuestionCommandResponse>
{
    private readonly QuizService _quizService = quizService;

    public async Task<PatchQuestionCommandResponse> Handle(PatchQuestionCommand request, CancellationToken cancellationToken)
    {
        PatchQuestionCommandResponse response = new();

        try
        {
            response = await _quizService.PatchQuestion(request.QuestionId, request.Request);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex;
        }

        return response;
    }
}

public class PatchQuestionCommandResponse
{
    public FourOptionQuestion? Question { get; set; }

    public bool Success { get; set; }

    public Exception? Error { get; set; }
}
