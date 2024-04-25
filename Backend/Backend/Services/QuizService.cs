using AutoMapper;
using Backend.Data;
using Backend.Dtos;
using Backend.Handlers.Questions;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class QuizService(QuizDbContext quizDbContext, IMapper mapper, ILogger<QuizService> logger)
{
    private readonly QuizDbContext _quizDbContext = quizDbContext;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<QuizService> _logger = logger;

    private const string logDatabaseWarningStringTemplate = "An error occured during database interaction: ";
    private const string logRegularWarningStringTemplate = "An unexpected error occured: ";

    public GetManyQuestionsCommandResponse GetManyQuestions(int numberOfQuestions)
    {
        try
        {
            var allQuestions = _quizDbContext.FourOptionQuestions.ToList();

            var myQuestions = _mapper.Map<List<FourOptionQuestion>, List<FourOptionQuestionDto>>(allQuestions);

            if (numberOfQuestions >= allQuestions.Count)
            {
                return new GetManyQuestionsCommandResponse
                {
                    Questions = _mapper.Map<List<FourOptionQuestion>, List<FourOptionQuestionDto>>(
                        SelectRandomQuestions(allQuestions, allQuestions.Count)),
                    Success = true,
                    Error = null,
                };
            }

            var questions = SelectRandomQuestions(allQuestions, numberOfQuestions);

            return new GetManyQuestionsCommandResponse
            {
                Questions = _mapper.Map<List<FourOptionQuestion>, List<FourOptionQuestionDto>>(questions),
                Success = true,
                Error = null,
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex is DbUpdateException
                ? logDatabaseWarningStringTemplate
                : logRegularWarningStringTemplate
                + ex.Message);

            return new GetManyQuestionsCommandResponse
            {
                Questions = null,
                Success = false,
                Error = ex,
            };
        }
    }

    public async Task<CreateQuestionCommandResponse> CreateQuestion(FourOptionQuestion fourOptionQuestion)
    {
        try
        {
            var floatingId = _quizDbContext.FloatingIds.FirstOrDefault();

            // If no IDs of deleted questions are present, count the number of questions and assign that + 1 as the ID.
            fourOptionQuestion.Id = floatingId is not null ? floatingId.Id : await _quizDbContext.FourOptionQuestions.CountAsync() + 1;

            _quizDbContext.FourOptionQuestions.Add(fourOptionQuestion);

            if (floatingId is not null)
                _quizDbContext.FloatingIds.Remove(floatingId);

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
                : logRegularWarningStringTemplate 
                + ex.Message
                );

            return new CreateQuestionCommandResponse
            {
                Question = null,
                Success = false,
                Error = ex,
            };
        }
    }

    public async Task<DeleteQuestionCommandResponse> DeleteQuestion(int questionId)
    {
        try
        {
            var questionToRemove = _quizDbContext.FourOptionQuestions.Find(questionId);

            if (questionToRemove is null)
                return new DeleteQuestionCommandResponse
                {
                    Question = null,
                    Success = false,
                    Error = new ArgumentException("The provided ID does not exist in the database."),
                };

            _quizDbContext.FourOptionQuestions.Remove(questionToRemove);
            _quizDbContext.FloatingIds.Add(new FloatingIds { Id = questionId });

            await _quizDbContext.SaveChangesAsync();

            return new DeleteQuestionCommandResponse
            {
                Question = questionToRemove,
                Success = true,
                Error = null,
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex is DbUpdateException
                ? logDatabaseWarningStringTemplate
                : logRegularWarningStringTemplate
                + ex.Message
                );

            return new DeleteQuestionCommandResponse
            {
                Question = null,
                Success = false,
                Error = ex,
            };
        }
    }

    private static List<FourOptionQuestion> SelectRandomQuestions(List<FourOptionQuestion> questions, int numberOfQuestions)
    {
        Random rnd = new();

        var selectedQuestions = new List<FourOptionQuestion>(numberOfQuestions);
        var remainingQuestions = new List<FourOptionQuestion>(questions);

        for (int i = 0; i < numberOfQuestions; i++)
        {
            int randomIndex = rnd.Next(0, remainingQuestions.Count);
            selectedQuestions.Add(remainingQuestions[randomIndex]);
            remainingQuestions.RemoveAt(randomIndex);
        }

        return selectedQuestions;
    }
}
