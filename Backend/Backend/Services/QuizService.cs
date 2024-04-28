﻿using AutoMapper;
using Backend.Data;
using Backend.Handlers.Questions;
using Backend.Handlers.Quiz;
using Backend.Models.Dtos;
using Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Backend.Services;

public class QuizService(QuizDbContext quizDbContext, IMapper mapper, ILogger<QuizService> logger)
{
    private readonly QuizDbContext _quizDbContext = quizDbContext;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<QuizService> _logger = logger;

    private const string LogDatabaseWarningStringTemplate = "An error occured during database interaction: ";
    private const string LogRegularWarningStringTemplate = "An unexpected error occured: ";

    public async Task<CheckAnswerCommandResponse> CheckAnswer(string playerName,  int questionAnswer)
    {
        try
        {
            playerName = FormatAndReturnPlayerName(playerName);

            var playerObject =
                _quizDbContext
                .PlayerStatistics
                .Include(ps => ps.PlayerStatisticsFourOptionQuestions)
                    .ThenInclude(psq => psq.Question)
                 .FirstOrDefault(ps => ps.Name == playerName);

            if (playerObject is null)
                return new CheckAnswerCommandResponse
                {
                    Message = null,
                    Success = false,
                    Error = new ArgumentException("No database entry matched the requested name."),
                };

            var playerStatisticsQuestionList = playerObject
                .PlayerStatisticsFourOptionQuestions
                .OrderBy(ps => ps.Order);

            if (!playerStatisticsQuestionList.Any())
                return new CheckAnswerCommandResponse
                {
                    Message = null,
                    Success = false,
                    Error = new ArgumentException("You've already answered all your questions. " +
                    "To restart, please initialize the quiz again."),
                };

            var playerStatisticsQuestion = playerStatisticsQuestionList.First();

            playerObject.PlayerStatisticsFourOptionQuestions.Remove(playerStatisticsQuestion);

            string message = "Incorrect.";
            if (questionAnswer == playerStatisticsQuestion.Question.CorrectOptionNumber)
            {
                playerObject.CorrectAnswers++;
                message = "Correct!";
            }

            await _quizDbContext.SaveChangesAsync();

            return new CheckAnswerCommandResponse
            {
                Message = message,
                Success = true,
                Error = null,
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex is DbUpdateException
                ? LogDatabaseWarningStringTemplate
                : LogRegularWarningStringTemplate
                + ex.Message);

            return new CheckAnswerCommandResponse
            {
                Message = null,
                Success = false,
                Error = ex
            };
        }
    }

    public GetQuestionCommandResponse GetQuestion(string playerName)
    {
        try
        {
            playerName = FormatAndReturnPlayerName(playerName);

            var playerObject =
                _quizDbContext
                .PlayerStatistics
                .Include(ps => ps.PlayerStatisticsFourOptionQuestions)
                    .ThenInclude(psq => psq.Question)
                 .FirstOrDefault(ps => ps.Name == playerName);

            if (playerObject is null)
                return new GetQuestionCommandResponse
                {
                    Question = null,
                    Details = null,
                    Success = false,
                    Error = new ArgumentException("No database entry matched the requested name."),
                };

            var questionList = playerObject
                .PlayerStatisticsFourOptionQuestions
                .OrderBy(q => q.Order)
                .Select(q => q.Question);

            if (!questionList.Any())
                return new GetQuestionCommandResponse
                {
                    Question = null,
                    Details = "That's all the questions. Thanks for playing! Your final score was: " +
                    playerObject.CorrectAnswers + "/" + playerObject.TotalAmountOfQuestions
                    + ". To play again, please initialize the quiz once more.",
                    Success = true,
                    Error = null,
                };

            var question = questionList.First();

            return new GetQuestionCommandResponse
            {
                Question = _mapper.Map<FourOptionQuestion, FourOptionQuestionDto>(question),
                Details = null,
                Success = true,
                Error = null,
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex is DbUpdateException
                ? LogDatabaseWarningStringTemplate
                : LogRegularWarningStringTemplate
                + ex.Message);

            return new GetQuestionCommandResponse
            {
                Question = null,
                Details = null,
                Success = false,
                Error = ex,
            };
        }
    }

    public async Task<InitializeQuizCommandResponse> InitializeQuiz(string playerName, int amountOfQuestions, string? questionType)
    {
        try
        {
            playerName = FormatAndReturnPlayerName(playerName);

            var playerObject = _quizDbContext
                .PlayerStatistics
                .Include(ps => ps.PlayerStatisticsFourOptionQuestions)
                .FirstOrDefault(ps => ps.Name == playerName);

            var questionsObject = GetManyQuestions(amountOfQuestions, questionType);

            if (questionsObject.Questions is null)
                return new InitializeQuizCommandResponse
                {
                    Details = null,
                    Success = false,
                    Error = questionsObject.Error,
                };

            if (playerObject is null)
            {
                playerObject = new PlayerStatistics
                {
                    Name = playerName,
                    CorrectAnswers = 0,
                    TotalAmountOfQuestions = questionsObject.Questions.Count,
                    PlayerStatisticsFourOptionQuestions = new List<PlayerStatisticsFourOptionQuestion>(),
                };
                _quizDbContext.PlayerStatistics.Add(playerObject);
            }
            else
            {
                playerObject.CorrectAnswers = 0;
                playerObject.TotalAmountOfQuestions = questionsObject.Questions.Count;
                playerObject.PlayerStatisticsFourOptionQuestions.Clear();
            }

            int order = 1;
            foreach (var question in questionsObject.Questions)
            {
                PlayerStatisticsFourOptionQuestion playerStatisticsFourOptionQuestion = new()
                {
                    QuestionId = question.Id,
                    Question = question,
                    PlayerStatisticsId = playerObject!.Id,
                    PlayerStatistics = playerObject,
                    Order = order++,
                };

                playerObject.PlayerStatisticsFourOptionQuestions.Add(playerStatisticsFourOptionQuestion);
            }

            await _quizDbContext.SaveChangesAsync();

            return new InitializeQuizCommandResponse
            {
                Details = $"Quiz has been initialized successfully for player {playerName}",
                Success = true,
                Error = null,
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex is DbUpdateException
                ? LogDatabaseWarningStringTemplate
                : LogRegularWarningStringTemplate
                + ex.Message);

            return new InitializeQuizCommandResponse
            {
                Details = null,
                Success = false,
                Error = ex,
            };
        }
    }

    public GetManyQuestionsCommandResponse GetManyQuestions(int numberOfQuestions, string? questionType)
    {
        try
        { // If any question type is chosen, take all questions of that type. Otherwise, take any.
            var questions = questionType is null
                ? _quizDbContext.FourOptionQuestions
                : _quizDbContext.FourOptionQuestions
                .Where(q => string.Equals(
                    EF.Property<string>(q, "QuestionType").ToLower(), questionType.ToLower())
                );

            if (questions.FirstOrDefault() is null)
                return new GetManyQuestionsCommandResponse
                {
                    Questions = null,
                    Success = false,
                    Error = new ArgumentException("The requested question type does not exist, or matches no questions."),
                };

            return new GetManyQuestionsCommandResponse
            { // Choose X amount of questions. Randomization happens when getting individual question.
                Questions = questions
                    .AsEnumerable()
                    .OrderBy(q => Guid.NewGuid())
                    .Take(numberOfQuestions)
                    .ToList(),
                Success = true,
                Error = null,
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex is DbUpdateException
                ? LogDatabaseWarningStringTemplate
                : LogRegularWarningStringTemplate
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
                ? LogDatabaseWarningStringTemplate 
                : LogRegularWarningStringTemplate 
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
                ? LogDatabaseWarningStringTemplate
                : LogRegularWarningStringTemplate
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

    private static string FormatAndReturnPlayerName(string playerName)
    {
        TextInfo textInfo = new CultureInfo("sv-SE", false).TextInfo;
        return textInfo.ToTitleCase(playerName);
    }
}
