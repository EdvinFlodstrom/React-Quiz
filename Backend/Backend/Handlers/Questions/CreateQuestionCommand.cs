﻿using Backend.Infrastructure.Models.Dtos;
using Backend.Infrastructure.Models.Entities;
using Backend.Services;
using MediatR;

namespace Backend.Handlers.Questions;

public class CreateQuestionCommand : IRequest<CreateQuestionCommandResponse>
{
    public required FourOptionQuestion FourOptionQuestion { get; set; }
}

public class CreateQuestionCommandHandler(IQuizService quizService) : IRequestHandler<CreateQuestionCommand, CreateQuestionCommandResponse>
{
    private readonly IQuizService _quizService = quizService;

    public async Task<CreateQuestionCommandResponse> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        CreateQuestionCommandResponse response = new();

        try
        {
            response = await _quizService.CreateQuestion(request.FourOptionQuestion);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex;
        }

        return response;
    }
}

public class CreateQuestionCommandResponse
{
    public FourOptionQuestionDto? Question { get; set; }

    public bool Success { get; set; }

    public Exception? Error { get; set; }
}