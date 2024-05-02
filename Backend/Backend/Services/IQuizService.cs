using Backend.Handlers.Questions;
using Backend.Handlers.Quiz;
using Backend.Infrastructure.Models.Entities;
using Backend.Infrastructure.Models.Requests;

namespace Backend.Services;

public interface IQuizService
{
    Task<CheckAnswerCommandResponse> CheckAnswer(string playerName, int questionAnswer);

    GetQuestionCommandResponse GetQuestion(string playerName);

    Task<InitializeQuizCommandResponse> InitializeQuiz(string playerName, int amountOfQuestions, string? questionType);

    GetManyQuestionsCommandResponse GetManyQuestions(int numberOfQuestions, string? questionType);

    Task<CreateQuestionCommandResponse> CreateQuestion(FourOptionQuestion fourOptionQuestion);

    Task<PatchQuestionCommandResponse> PatchQuestion(int questionId, PatchQuestionRequest request);

    Task<DeleteQuestionCommandResponse> DeleteQuestion(int questionId);
}
