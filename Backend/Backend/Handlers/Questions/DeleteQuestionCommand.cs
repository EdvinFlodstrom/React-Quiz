﻿using Backend.Infrastructure.Models.Entities;
using Backend.Services;
using MediatR;

namespace Backend.Handlers.Questions;

public class DeleteQuestionCommand : IRequest<DeleteQuestionCommandResponse>
{
    public required int QuestionId { get; set; }
}

public class DeleteQuestionCommandHandler(IQuizService quizService) : IRequestHandler<DeleteQuestionCommand, DeleteQuestionCommandResponse>
{
    private readonly IQuizService _quizService = quizService;

    public async Task<DeleteQuestionCommandResponse> Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
    {
        DeleteQuestionCommandResponse response = new();

        try
        {
            response = await _quizService.DeleteQuestion(request.QuestionId);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex;
        }

        return response;
    }
}

public class DeleteQuestionCommandResponse
{
    public FourOptionQuestion? Question { get; set; }

    public bool Success { get; set; }

    public Exception? Error { get; set; }
}
