﻿using Backend.Infrastructure.Models.Entities;
using Backend.Services;
using MediatR;

namespace Backend.Handlers.Questions;

public class GetManyQuestionsCommand : IRequest<GetManyQuestionsCommandResponse>
{
    public required int NumberOfQuestions { get; set; }

    public string? QuestionType { get; set; }
}

public class GetManyQuestionsCommandHandler(IQuizService quizService) : IRequestHandler<GetManyQuestionsCommand, GetManyQuestionsCommandResponse>
{
    private readonly IQuizService _quizService = quizService;

    public Task<GetManyQuestionsCommandResponse> Handle(GetManyQuestionsCommand request, CancellationToken cancellationToken)
    {
        GetManyQuestionsCommandResponse response = new();

        try
        {
            response = _quizService.GetManyQuestions(request.NumberOfQuestions, request.QuestionType);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex;
        }

        return Task.FromResult(response);
    }
}

public class GetManyQuestionsCommandResponse
{
    public List<FourOptionQuestion>? Questions { get; set; }

    public bool Success { get; set; }

    public Exception? Error { get; set; }
}
