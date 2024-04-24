using AutoMapper;
using Backend.Data;
using Backend.Dtos;
using Backend.Handlers.Questions;
using Backend.Models;
using Backend.Models.QuestionTypes;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

namespace Backend.Services;

public class QuizService(QuizDbContext quizDbContext, IMapper mapper, ILogger<QuizService> logger)
{
    private readonly QuizDbContext _quizDbContext = quizDbContext;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<QuizService> _logger = logger;

    private const string logDatabaseWarningStringTemplate = "An error occured during database interaction: ";
    private const string logRegularWarningStringTemplate = "An unexpected error occured: ";

    public async Task<CreateQuestionCommandResponse> CreateQuestion(FourOptionQuestion fourOptionQuestion)
    {
        try
        {
            var floatingId = _quizDbContext.FloatingIds.FirstOrDefault();

            // If no IDs of deleted questions are present, count the number of questions and assign that + 1 as the ID.
            fourOptionQuestion.Id = floatingId is not null ? floatingId.Id : await _quizDbContext.FourOptionQuestions.CountAsync() + 1;

            _quizDbContext.FourOptionQuestions.Add(fourOptionQuestion);
            await _quizDbContext.SaveChangesAsync();

            return new CreateQuestionCommandResponse
            {
                Question = _mapper.Map<FourOptionQuestionDto>(fourOptionQuestion),
                Success = true,
                Error = null,
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex is DbUpdateException 
                ? logDatabaseWarningStringTemplate 
                : logRegularWarningStringTemplate + ex.Message
                );

            return new CreateQuestionCommandResponse
            {
                Question = null,
                Success = false,
                Error = ex,
            };
        }
    }
}
