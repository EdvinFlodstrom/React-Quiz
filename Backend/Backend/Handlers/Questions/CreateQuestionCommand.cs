using Backend.Dtos;
using Backend.Models;
using Backend.Services;
using MediatR;

namespace Backend.Handlers.Questions;

public class CreateQuestionCommand : IRequest<CreateQuestionCommandResponse>
{
    public required FourOptionQuestion FourOptionQuestion { get; set; }
}

public class CreateQuestionCommandHandler(QuizService quizService) : IRequestHandler<CreateQuestionCommand, CreateQuestionCommandResponse>
{
    private readonly QuizService _quizService = quizService;
    public Task<CreateQuestionCommandResponse> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        CreateQuestionCommandResponse response = new();

        try
        {
            // Call QuizService and handle response...


        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex;
        }

        return Task.FromResult(response);
    }
}

public class CreateQuestionCommandResponse
{
    public bool Success { get; set; }

    public Exception? Error { get; set; }

    public FourOptionQuestionDto? Question { get; set; }
}