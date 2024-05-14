using AutoMapper;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Models.Dtos;
using Backend.Infrastructure.Models.Entities;
using Backend.Infrastructure.Models.Entities.QuestionTypes;
using Backend.Infrastructure.Models.Requests;
using Backend.Services;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests;

[TestClass]
public class QuizServiceTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<QuizService>> _loggerMock;
    private QuizDbContext _context;
    private QuizService _service;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public QuizServiceTests()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new();
    }

    [TestInitialize]
    public void TestInitialize()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<QuizDbContext>()
            .UseSqlite(connection)
            .Options;

        _context = new QuizDbContext(options);
        _context.Database.EnsureCreated();

        _service = new QuizService(_context, _mapperMock.Object, _loggerMock.Object);
    }

    [TestMethod]
    public async Task CreateQuestion_With_FloatingId_Should_Return_CreateQuestionCommandResponse()
    {
        // Arrange
        _context.FloatingIds.Add(new()
        {
            Id = 4,
        });
        await _context.SaveChangesAsync();

        GeographyQuestion geographyQuestion = new()
        {
            Question = "What is Eyjafjallajökull?",
            Option1 = "A mountain range in Norway",
            Option2 = "A glacier in Iceland",
            Option3 = "A crater in China",
            Option4 = "A city in Greenland",
            CorrectOptionNumber = 2,
        };

        FourOptionQuestionDto fourOptionQuestionDto = new()
        {
            Question = "What is Eyjafjallajökull?",
            Option1 = "A mountain range in Norway",
            Option2 = "A glacier in Iceland",
            Option3 = "A crater in China",
            Option4 = "A city in Greenland",
        };

        _mapperMock.Setup(m => m.Map<FourOptionQuestionDto>(It.IsAny<FourOptionQuestion>()))
            .Returns(fourOptionQuestionDto);

        // Act
        var response = await _service.CreateQuestion(geographyQuestion);
        _mapperMock.Reset();

        // Assert
        response.Should().NotBeNull();
        response.Question.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.Error.Should().BeNull();

        FourOptionQuestionDto question = response.Question!;
        question.Question.Should().Be("What is Eyjafjallajökull?");
        question.Option1.Should().Be("A mountain range in Norway");
        question.Option2.Should().Be("A glacier in Iceland");
        question.Option3.Should().Be("A crater in China");
        question.Option4.Should().Be("A city in Greenland");
        _context.FourOptionQuestions.Should().NotBeEmpty();

        FourOptionQuestion dbQuestion = _context.FourOptionQuestions.FirstOrDefault()!;
        dbQuestion.Id.Should().Be(4);
        dbQuestion.Question.Should().Be("What is Eyjafjallajökull?");
        dbQuestion.Option1.Should().Be("A mountain range in Norway");
        dbQuestion.Option2.Should().Be("A glacier in Iceland");
        dbQuestion.Option3.Should().Be("A crater in China");
        dbQuestion.Option4.Should().Be("A city in Greenland");
        dbQuestion.CorrectOptionNumber.Should().Be(2);

        _context.FloatingIds.Should().BeEmpty();
    }

    [TestMethod]
    public async Task CreateQuestion_Without_FloatingId_Should_Return_CreateQuestionCommandResponse()
    {
        // Arrange
        GeographyQuestion geographyQuestion = new()
        {
            Question = "What is Eyjafjallajökull?",
            Option1 = "A mountain range in Norway",
            Option2 = "A glacier in Iceland",
            Option3 = "A crater in China",
            Option4 = "A city in Greenland",
            CorrectOptionNumber = 2,
        };

        FourOptionQuestionDto fourOptionQuestionDto = new()
        {
            Question = "What is Eyjafjallajökull?",
            Option1 = "A mountain range in Norway",
            Option2 = "A glacier in Iceland",
            Option3 = "A crater in China",
            Option4 = "A city in Greenland",
        };

        _mapperMock.Setup(m => m.Map<FourOptionQuestionDto>(It.IsAny<FourOptionQuestion>()))
            .Returns(fourOptionQuestionDto);

        // Act
        var response = await _service.CreateQuestion(geographyQuestion);
        _mapperMock.Reset();

        // Assert
        response.Should().NotBeNull();
        response.Question.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.Error.Should().BeNull();

        FourOptionQuestionDto question = response.Question!;
        question.Question.Should().Be("What is Eyjafjallajökull?");
        question.Option1.Should().Be("A mountain range in Norway");
        question.Option2.Should().Be("A glacier in Iceland");
        question.Option3.Should().Be("A crater in China");
        question.Option4.Should().Be("A city in Greenland");
        _context.FourOptionQuestions.Should().NotBeEmpty();

        FourOptionQuestion dbQuestion = _context.FourOptionQuestions.FirstOrDefault()!;
        dbQuestion.Id.Should().Be(await _context.FourOptionQuestions.CountAsync());
        dbQuestion.Question.Should().Be("What is Eyjafjallajökull?");
        dbQuestion.Option1.Should().Be("A mountain range in Norway");
        dbQuestion.Option2.Should().Be("A glacier in Iceland");
        dbQuestion.Option3.Should().Be("A crater in China");
        dbQuestion.Option4.Should().Be("A city in Greenland");
        dbQuestion.CorrectOptionNumber.Should().Be(2);

        _context.FloatingIds.Should().BeEmpty();
    }

    [TestMethod]
    public async Task CreateQuestion_DuplicateQuestion_Should_Return_CreateQuestionCommandResponseFalse()
    {
        // Arrange
        _context.FourOptionQuestions.Add(new GeographyQuestion
        {
            Id = 1,
            Question = "What is What is Eyjafjallajökull?",
            Option1 = "A mountain range in Norway",
            Option2 = "A glacier in Iceland",
            Option3 = "A crater in China",
            Option4 = "A city in Greenland",
            CorrectOptionNumber = 2,
        });

        await _context.SaveChangesAsync();

        GeographyQuestion geographyQuestion = new()
        {
            Question = "What is What is Eyjafjallajökull",
            Option1 = "A mountain range in Norway",
            Option2 = "A glacier in Iceland",
            Option3 = "A crater in China",
            Option4 = "A city in Greenland",
            CorrectOptionNumber = 2,
        };

        // Act
        var response = await _service.CreateQuestion(geographyQuestion);

        // Assert
        response.Should().NotBeNull();
        response.Question.Should().BeNull();
        response.Success.Should().BeFalse();
        response.Error.Should().NotBeNull();

        Exception error = response.Error!;
        error.Message.Should().Be("The same question already exists in the database.");

        _context.FourOptionQuestions.Count().Should().Be(1);
    }

    [TestMethod]
    public async Task CreateQuestion_Error_Should_Return_CreateQuestionCommandResponseFalse()
    {
        // Arrange
        _context.FloatingIds.Add(new()
        {
            Id = 1,
        });

        _context.FourOptionQuestions.Add(new GeographyQuestion()
        {
            Id = 1,
            Question = "What is Eyjafjallajökull?",
            Option1 = "A mountain range in Norway",
            Option2 = "A glacier in Iceland",
            Option3 = "A crater in China",
            Option4 = "A city in Greenland",
            CorrectOptionNumber = 2,
        });

        await _context.SaveChangesAsync();

        MathQuestion mathQuestion = new()
        {
            Id = 2,
            Question = "What is 1 + 1?",
            Option1 = "Probably a large number",
            Option2 = "3",
            Option3 = "11",
            Option4 = "1337",
            CorrectOptionNumber = 3,
        };

        // Act
        var response = await _service.CreateQuestion(mathQuestion);

        // Assert
        response.Should().NotBeNull();
        response.Question.Should().BeNull();
        response.Success.Should().BeFalse();
        response.Error.Should().NotBeNull();

        Exception error = response.Error!;
        error.Message.Should().NotBeNull();

        _context.FourOptionQuestions.Count().Should().Be(1);
        _context.FloatingIds.Should().NotBeEmpty();

        VerifyLog(LogLevel.Warning);
    }

    [TestMethod]
    public async Task PatchQuestion_Should_Return_PatchQuestionCommandResponse()
    {
        // Arrange
        _context.FourOptionQuestions.Add(new GeographyQuestion()
        {
            Id = 1,
            Question = "What is Eyjafjallajökull?",
            Option1 = "A mountain range in Norway",
            Option2 = "A glacier in Iceland",
            Option3 = "A crater in China",
            Option4 = "A city in Greenland",
            CorrectOptionNumber = 2,
        });

        await _context.SaveChangesAsync();

        int questionId = 1;
        PatchQuestionRequest patchQuestionRequest = new()
        {
            Question = "Patched question",
            Option1 = "Patched option 1!",
        };

        // Act
        var response = await _service.PatchQuestion(questionId, patchQuestionRequest);

        // Assert
        response.Should().NotBeNull();
        response.Question.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.Error.Should().BeNull();

        FourOptionQuestion question = response.Question!;
        question.Question.Should().Be("Patched question?");
        question.Option1.Should().Be("Patched option 1!");
        question.Option2.Should().Be("A glacier in Iceland");
        question.Option3.Should().Be("A crater in China");
        question.Option4.Should().Be("A city in Greenland");
        question.CorrectOptionNumber.Should().Be(2);
        _context.FourOptionQuestions.Should().NotBeEmpty();

        FourOptionQuestion dbQuestion = _context.FourOptionQuestions.FirstOrDefault()!;
        dbQuestion.Question.Should().Be("Patched question?");
        dbQuestion.Option1.Should().Be("Patched option 1!");
        dbQuestion.Option2.Should().Be("A glacier in Iceland");
        dbQuestion.Option3.Should().Be("A crater in China");
        dbQuestion.Option4.Should().Be("A city in Greenland");
        dbQuestion.CorrectOptionNumber.Should().Be(2);
    }

    [TestMethod]
    public async Task PatchQuestion_QuestionIdInvalid_Should_Return_PatchQuestionCommandResponseFalse()
    {
        // Arrange
        int questionId = 1337;
        PatchQuestionRequest patchQuestionRequest = new();

        // Act
        var response = await _service.PatchQuestion(questionId, patchQuestionRequest);

        // Assert
        response.Should().NotBeNull();
        response.Question.Should().BeNull();
        response.Success.Should().BeFalse();
        response.Error.Should().NotBeNull();

        Exception error = response.Error!;
        error.Message.Should().Be("A question with the provided ID does not exist in the database.");
        _context.FourOptionQuestions.Should().BeEmpty();
    }

    [TestMethod]
    public async Task DeleteQuestion_Should_Return_DeleteQuestionCommandResponse()
    {
        // Arrange
        _context.FourOptionQuestions.Add(new GeographyQuestion()
        {
            Id = 1,
            Question = "What is Eyjafjallajökull?",
            Option1 = "A mountain range in Norway",
            Option2 = "A glacier in Iceland",
            Option3 = "A crater in China",
            Option4 = "A city in Greenland",
            CorrectOptionNumber = 2,
        });

        await _context.SaveChangesAsync();

        int questionId = 1;

        // Act
        var response = await _service.DeleteQuestion(questionId);

        // Assert
        response.Should().NotBeNull();
        response.Question.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.Error.Should().BeNull();

        FourOptionQuestion question = response.Question!;
        question.Question.Should().Be("What is Eyjafjallajökull?");
        question.Option1.Should().Be("A mountain range in Norway");
        question.Option2.Should().Be("A glacier in Iceland");
        question.Option3.Should().Be("A crater in China");
        question.Option4.Should().Be("A city in Greenland");
        _context.FourOptionQuestions.Should().BeEmpty();
        _context.FloatingIds.Should().NotBeEmpty();

        FloatingIds floatingId = _context.FloatingIds.FirstOrDefault()!;
        floatingId.Id.Should().Be(1);
    }

    [TestMethod]
    public async Task DeleteQuestion_QuestionIdInvalid_Should_Return_DeleteQuestionCommandResponseFalse()
    {
        // Arrange
        int questionId = 1337;

        // Act
        var response = await _service.DeleteQuestion(questionId);

        // Assert
        response.Should().NotBeNull();
        response.Question.Should().BeNull();
        response.Success.Should().BeFalse();
        response.Error.Should().NotBeNull();

        Exception error = response.Error!;
        error.Message.Should().Be("A question with the provided ID does not exist in the database.");
        _context.FourOptionQuestions.Should().BeEmpty();
    }

    [TestMethod]
    public async Task GetManyQuestions_QuestionTypeNull_Should_Return_GetManyQuestionsCommandResponse()
    {
        // Arrange
        List<FourOptionQuestion> questionList =
        [
            new GeographyQuestion()
            {
                Id = 1,
                Question = "What is Eyjafjallajökull?",
                Option1 = "A mountain range in Norway",
                Option2 = "A glacier in Iceland",
                Option3 = "A crater in China",
                Option4 = "A city in Greenland",
                CorrectOptionNumber = 2,
            },
            new MathQuestion()
            {
                Id = 2,
                Question = "What is 1 + 1?",
                Option1 = "Probably a large number",
                Option2 = "3",
                Option3 = "11",
                Option4 = "1337",
                CorrectOptionNumber = 3,
            },
        ];

        _context.FourOptionQuestions.AddRange(questionList);

        await _context.SaveChangesAsync();

        int numberOfQuestions = 2;
        string? questionType = null;

        // Act
        var response = _service.GetManyQuestions(numberOfQuestions, questionType);

        // Assert
        response.Should().NotBeNull();
        response.Questions.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.Error.Should().BeNull();

        List<FourOptionQuestion> questions = response.Questions!;
        questions.Count.Should().Be(numberOfQuestions);
        questions[0].Should().NotBeNull();
        questions[1].Should().NotBeNull();

        FourOptionQuestion geographyQuestion = questions.FirstOrDefault(q => q.Id == 1)!;
        FourOptionQuestion mathQuestion = questions.FirstOrDefault(q => q.Id == 2)!;

        geographyQuestion.Id.Should().Be(1);
        geographyQuestion.Question.Should().Be("What is Eyjafjallajökull?");
        geographyQuestion.Option1.Should().Be("A mountain range in Norway");
        geographyQuestion.Option2.Should().Be("A glacier in Iceland");
        geographyQuestion.Option3.Should().Be("A crater in China");
        geographyQuestion.Option4.Should().Be("A city in Greenland");
        geographyQuestion.CorrectOptionNumber.Should().Be(2);

        mathQuestion.Id.Should().Be(2);
        mathQuestion.Question.Should().Be("What is 1 + 1?");
        mathQuestion.Option1.Should().Be("Probably a large number");
        mathQuestion.Option2.Should().Be("3");
        mathQuestion.Option3.Should().Be("11");
        mathQuestion.Option4.Should().Be("1337");
        mathQuestion.CorrectOptionNumber.Should().Be(3);
    }

    [TestMethod]
    public async Task GetManyQuestions_QuestionTypeNotNull_Should_Return_GetManyQuestionsCommandResponse()
    {
        // Arrange
        List<FourOptionQuestion> questionList =
        [
            new GeographyQuestion()
            {
                Id = 1,
                Question = "What is Eyjafjallajökull?",
                Option1 = "A mountain range in Norway",
                Option2 = "A glacier in Iceland",
                Option3 = "A crater in China",
                Option4 = "A city in Greenland",
                CorrectOptionNumber = 2,
            },
            new MathQuestion()
            {
                Id = 2,
                Question = "What is 1 + 1?",
                Option1 = "Probably a large number",
                Option2 = "3",
                Option3 = "11",
                Option4 = "1337",
                CorrectOptionNumber = 3,
            },
        ];

        _context.FourOptionQuestions.AddRange(questionList);

        await _context.SaveChangesAsync();

        // Request two questions, but only one matches the question type.
        int numberOfQuestions = 2;
        string questionType = "Geography";

        // Act
        var response = _service.GetManyQuestions(numberOfQuestions, questionType);

        // Assert
        response.Should().NotBeNull();
        response.Questions.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.Error.Should().BeNull();

        List<FourOptionQuestion> questions = response.Questions!;
        questions.Count.Should().Be(1); // See comment above
        questions[0].Should().NotBeNull();

        FourOptionQuestion geographyQuestion = questions.FirstOrDefault(q => q.Id == 1)!;

        geographyQuestion.Id.Should().Be(1);
        geographyQuestion.Question.Should().Be("What is Eyjafjallajökull?");
        geographyQuestion.Option1.Should().Be("A mountain range in Norway");
        geographyQuestion.Option2.Should().Be("A glacier in Iceland");
        geographyQuestion.Option3.Should().Be("A crater in China");
        geographyQuestion.Option4.Should().Be("A city in Greenland");
        geographyQuestion.CorrectOptionNumber.Should().Be(2);
    }

    [TestMethod]
    public void GetManyQuestions_QuestionTypeInvalid_Should_Return_GetManyQuestionsCommandResponseFalse()
    {
        // Arrange
        int numberOfQuestions = 1337;
        string questionType = "How about all of them?";

        // Act
        var response = _service.GetManyQuestions(numberOfQuestions, questionType);

        // Assert
        response.Should().NotBeNull();
        response.Questions.Should().BeNull();
        response.Success.Should().BeFalse();
        response.Error.Should().NotBeNull();

        Exception error = response.Error!;
        error.Message.Should().Be("The requested question type does not exist, or matches no questions.");
    }

    [TestMethod]
    public async Task InitializeQuiz_NewPlayer_Should_Return_InitializeQuizCommandResponse()
    {
        // Arrange
        _context.PlayerStatistics.Add(new()
        {
            Name = "Rally-Rolf",
            CorrectAnswers = 0,
            TotalAmountOfQuestions = 0,
            PlayerStatisticsFourOptionQuestions = [],
        });

        _context.FourOptionQuestions.Add(new GeographyQuestion()
        {
            Id = 1,
            Question = "What is Eyjafjallajökull?",
            Option1 = "A mountain range in Norway",
            Option2 = "A glacier in Iceland",
            Option3 = "A crater in China",
            Option4 = "A city in Greenland",
            CorrectOptionNumber = 2,
        });

        await _context.SaveChangesAsync();

        string playerName = "Åke Åkman";
        int amountOfQuestions = 1;
        string questionType = "Geography";

        // Act
        var response = await _service.InitializeQuiz(playerName, amountOfQuestions, questionType);

        // Assert
        response.Should().NotBeNull();
        response.Details.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.Error.Should().BeNull();

        string details = response.Details!;
        details.Should().Be("Quiz has been initialized successfully for player Åke Åkman");

        _context.PlayerStatistics.Should().NotBeEmpty();
        _context.PlayerStatistics.Count().Should().Be(2);
        PlayerStatistics player = _context.PlayerStatistics.FirstOrDefault(p => p.Name == "Åke Åkman")!;
        player.Should().NotBeNull();
        player.Id.Should().Be(2);
        player.Name.Should().Be("Åke Åkman");
        player.CorrectAnswers.Should().Be(0);
        player.TotalAmountOfQuestions.Should().Be(amountOfQuestions);
        player.PlayerStatisticsFourOptionQuestions.Should().NotBeNull();

        FourOptionQuestion question = player.PlayerStatisticsFourOptionQuestions.FirstOrDefault()!.Question;
        question.Should().NotBeNull();
        question.Id.Should().Be(1);
        question.Question.Should().Be("What is Eyjafjallajökull?");
        question.Option1.Should().Be("A mountain range in Norway");
        question.Option2.Should().Be("A glacier in Iceland");
        question.Option3.Should().Be("A crater in China");
        question.Option4.Should().Be("A city in Greenland");
        question.CorrectOptionNumber.Should().Be(2);
    }

    [TestMethod]
    public async Task InitializeQuiz_ExistingPlayer_Should_Return_InitializeQuizCommandResponse()
    {
        // Arrange
        _context.PlayerStatistics.Add(new()
        {
            Name = "Åke Åkman",
            CorrectAnswers = 0,
            TotalAmountOfQuestions = 0,
            PlayerStatisticsFourOptionQuestions = [],
        });

        _context.FourOptionQuestions.Add(new GeographyQuestion()
        {
            Id = 1,
            Question = "What is Eyjafjallajökull?",
            Option1 = "A mountain range in Norway",
            Option2 = "A glacier in Iceland",
            Option3 = "A crater in China",
            Option4 = "A city in Greenland",
            CorrectOptionNumber = 2,
        });

        await _context.SaveChangesAsync();

        string playerName = "åkE åKmAn";
        int amountOfQuestions = 1;
        string questionType = "Geography";

        // Act
        var response = await _service.InitializeQuiz(playerName, amountOfQuestions, questionType);

        // Assert
        response.Should().NotBeNull();
        response.Details.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.Error.Should().BeNull();

        string details = response.Details!;
        details.Should().Be("Quiz has been initialized successfully for player Åke Åkman");

        _context.PlayerStatistics.Should().NotBeEmpty();
        _context.PlayerStatistics.Count().Should().Be(1);
        PlayerStatistics player = _context.PlayerStatistics.FirstOrDefault(p => p.Name == "Åke Åkman")!;
        player.Should().NotBeNull();
        player.Id.Should().Be(1);
        player.Name.Should().Be("Åke Åkman");
        player.CorrectAnswers.Should().Be(0);
        player.TotalAmountOfQuestions.Should().Be(amountOfQuestions);
        player.PlayerStatisticsFourOptionQuestions.Should().NotBeNull();

        FourOptionQuestion question = player.PlayerStatisticsFourOptionQuestions.FirstOrDefault()!.Question;
        question.Should().NotBeNull();
        question.Id.Should().Be(1);
        question.Question.Should().Be("What is Eyjafjallajökull?");
        question.Option1.Should().Be("A mountain range in Norway");
        question.Option2.Should().Be("A glacier in Iceland");
        question.Option3.Should().Be("A crater in China");
        question.Option4.Should().Be("A city in Greenland");
        question.CorrectOptionNumber.Should().Be(2);
    }

    [TestMethod]
    public async Task InitializeQuiz_NewPlayer_QuestionTypeInvalid_Should_Return_InitializeQuizCommandResponseFalse()
    {
        // Arrange
        string playerName = "Åke Åkman";
        int amountOfQuestions = 1337;
        string questionType = "To be a question type or not to be";

        // Act
        var response = await _service.InitializeQuiz(playerName, amountOfQuestions, questionType);

        // Assert
        response.Should().NotBeNull();
        response.Details.Should().BeNull();
        response.Success.Should().BeFalse();
        response.Error.Should().NotBeNull();

        Exception error = response.Error!;
        error.Message.Should().Be("The requested question type does not exist, or matches no questions.");
    }

    [TestMethod]
    public async Task GetQuestion_ExistingPlayer_HasQuestionsLeft_Should_Return_GetQuestionCommandResponse()
    {
        // Arrange
        PlayerStatistics playerObject = new()
        {
            Name = "Åke Åkman",
            CorrectAnswers = 0,
            TotalAmountOfQuestions = 1,
            PlayerStatisticsFourOptionQuestions = [],
        };

        playerObject.PlayerStatisticsFourOptionQuestions.Add(new()
        {
            QuestionId = 1,
            Question = new GeographyQuestion()
            {
                Id = 1,
                Question = "What is Eyjafjallajökull?",
                Option1 = "A mountain range in Norway",
                Option2 = "A glacier in Iceland",
                Option3 = "A crater in China",
                Option4 = "A city in Greenland",
                CorrectOptionNumber = 2,
            },
            Order = 1,
            PlayerStatisticsId = 1,
            PlayerStatistics = playerObject,
        });

        _context.PlayerStatistics.Add(playerObject);

        await _context.SaveChangesAsync();

        string playerName = "åke ÅKMAN";

        FourOptionQuestionDto fourOptionQuestionDto = new()
        {
            Question = "What is Eyjafjallajökull?",
            Option1 = "A mountain range in Norway",
            Option2 = "A glacier in Iceland",
            Option3 = "A crater in China",
            Option4 = "A city in Greenland",
        };

        _mapperMock.Setup(m => m.Map<FourOptionQuestion, FourOptionQuestionDto>(It.IsAny<FourOptionQuestion>()))
            .Returns(fourOptionQuestionDto);

        // Act
        var response = _service.GetQuestion(playerName);
        _mapperMock.Reset();

        // Assert
        response.Should().NotBeNull();
        response.Question.Should().NotBeNull();
        response.Details.Should().BeNull();
        response.Success.Should().BeTrue();
        response.Error.Should().BeNull();

        FourOptionQuestionDto question = response.Question!;
        question.Question.Should().Be("What is Eyjafjallajökull?");
        question.Option1.Should().Be("A mountain range in Norway");
        question.Option2.Should().Be("A glacier in Iceland");
        question.Option3.Should().Be("A crater in China");
        question.Option4.Should().Be("A city in Greenland");
        _context.PlayerStatistics.Should().NotBeEmpty();
        _context.PlayerStatistics.Count().Should().Be(1);

        var player = _context.PlayerStatistics.FirstOrDefault()!;
        player.Id.Should().Be(1);
        player.Name.Should().Be("Åke Åkman");
        player.CorrectAnswers.Should().Be(0);
        player.TotalAmountOfQuestions.Should().Be(1);
        player.PlayerStatisticsFourOptionQuestions.Should().NotBeEmpty();
    }

    [TestMethod]
    public async Task GetQuestion_ExistingPlayer_HasNoQuestionsLeft_Should_Return_GetQuestionCommandResponse()
    {
        // Arrange
        _context.PlayerStatistics.Add(new()
        {
            Name = "Åke Åkman",
            CorrectAnswers = 1337,
            TotalAmountOfQuestions = 1337,
            PlayerStatisticsFourOptionQuestions = [],
        });

        await _context.SaveChangesAsync();

        string playerName = "ÅKE ÅKMAN";

        // Act
        var response = _service.GetQuestion(playerName);

        // Assert
        response.Should().NotBeNull();
        response.Question.Should().BeNull();
        response.Details.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.Error.Should().BeNull();

        string details = response.Details!;
        details.Should().NotBeEmpty();
    }

    [TestMethod]
    public void GetQuestion_UnexistingPlayer_Should_Return_GetQuestionCommandResponseFalse()
    {
        // Arrange
        string playerName = "Kool-Kalle";

        // Act
        var response = _service.GetQuestion(playerName);

        // Assert
        response.Should().NotBeNull();
        response.Question.Should().BeNull();
        response.Details.Should().BeNull();
        response.Success.Should().BeFalse();
        response.Error.Should().NotBeNull();

        Exception error = response.Error!;
        error.Message.Should().Be("No database entry matched the requested name.");

        _context.PlayerStatistics.Should().BeEmpty();
    }

    [TestMethod]
    public async Task GetQuestionById_Should_Return_GetQuestionByIdCommandResponse()
    {
        // Arrange
        _context.FourOptionQuestions.Add(new GeographyQuestion()
        {
            Id = 1,
            Question = "What is Eyjafjallajökull?",
            Option1 = "A mountain range in Norway",
            Option2 = "A glacier in Iceland",
            Option3 = "A crater in China",
            Option4 = "A city in Greenland",
            CorrectOptionNumber = 2,
        });

        await _context.SaveChangesAsync();

        FourOptionQuestionByIdDto fourOptionQuestionByIdDto = new()
        {
            QuestionType = "Geography",
            Question = "What is Eyjafjallajökull?",
            Option1 = "A mountain range in Norway",
            Option2 = "A glacier in Iceland",
            Option3 = "A crater in China",
            Option4 = "A city in Greenland",
            CorrectOptionNumber = 2,
        };

        int questionId = 1;

        _mapperMock.Setup(m => m.Map<FourOptionQuestion, FourOptionQuestionByIdDto>(It.IsAny<FourOptionQuestion>()))
            .Returns(fourOptionQuestionByIdDto);

        // Act
        var response = await _service.GetQuestionById(questionId);
        _mapperMock.Reset();

        // Assert
        response.Should().NotBeNull();
        response.Question.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.Error.Should().BeNull();

        FourOptionQuestionByIdDto question = response.Question!;
        question.QuestionType.Should().Be("Geography");
        question.Question.Should().Be("What is Eyjafjallajökull?");
        question.Option1.Should().Be("A mountain range in Norway");
        question.Option2.Should().Be("A glacier in Iceland");
        question.Option3.Should().Be("A crater in China");
        question.Option4.Should().Be("A city in Greenland");
        question.CorrectOptionNumber.Should().Be(2);
    }

    [TestMethod]
    public async Task GetQuestionById_QuestionNotFound_Should_Return_GetQuestionByIdCommandResponseFalse()
    {
        // Arrange
        int questionId = 1337;

        // Act
        var response = await _service.GetQuestionById(questionId);

        // Assert
        response.Should().NotBeNull();
        response.Question.Should().BeNull();
        response.Success.Should().BeFalse();
        response.Error.Should().NotBeNull();

        Exception error = response.Error!;
        error.Message.Should().Be("No question with the requested ID exists in the database.");
    }

    [TestMethod]
    public async Task CheckAnswer_ExistingPlayer_HasQuestionsLeft_CorrectAnswer_Should_Return_CheckAnswerCommandResponse()
    {
        // Arrange
        PlayerStatistics playerObject = new()
        {
            Name = "Åke Åkman",
            CorrectAnswers = 0,
            TotalAmountOfQuestions = 1,
            PlayerStatisticsFourOptionQuestions = [],
        };

        playerObject.PlayerStatisticsFourOptionQuestions.Add(new()
        {
            QuestionId = 1,
            Question = new GeographyQuestion()
            {
                Id = 1,
                Question = "What is Eyjafjallajökull?",
                Option1 = "A mountain range in Norway",
                Option2 = "A glacier in Iceland",
                Option3 = "A crater in China",
                Option4 = "A city in Greenland",
                CorrectOptionNumber = 2,
            },
            Order = 1,
            PlayerStatisticsId = 1,
            PlayerStatistics = playerObject,
        });

        _context.PlayerStatistics.Add(playerObject);

        await _context.SaveChangesAsync();

        string playerName = "Åke Åkman";
        int questionAnswer = 2;

        // Act
        var response = await _service.CheckAnswer(playerName, questionAnswer);

        // Assert
        response.Should().NotBeNull();
        response.Message.Should().NotBeNull();
        response.Correct.Should().BeTrue();
        response.CorrectOption.Should().Be(2);
        response.Success.Should().BeTrue();
        response.Error.Should().BeNull();

        string message = response.Message!;
        message.Should().Be("Correct!");
        _context.PlayerStatistics.Should().NotBeEmpty();

        PlayerStatistics player = _context.PlayerStatistics.FirstOrDefault(p => p.Name == playerName)!;
        player.Should().NotBeNull();
        player.PlayerStatisticsFourOptionQuestions.Count.Should().Be(0);
        player.TotalAmountOfQuestions.Should().Be(1);
        player.CorrectAnswers.Should().Be(1);
    }

    [TestMethod]
    public async Task CheckAnswer_ExistingPlayer_HasQuestionsLeft_IncorrectAnswer_Should_Return_CheckAnswerCommandResponse()
    {
        // Arrange
        PlayerStatistics playerObject = new()
        {
            Name = "Åke Åkman",
            CorrectAnswers = 0,
            TotalAmountOfQuestions = 1,
            PlayerStatisticsFourOptionQuestions = [],
        };

        playerObject.PlayerStatisticsFourOptionQuestions.Add(new()
        {
            QuestionId = 1,
            Question = new GeographyQuestion()
            {
                Id = 1,
                Question = "What is Eyjafjallajökull?",
                Option1 = "A mountain range in Norway",
                Option2 = "A glacier in Iceland",
                Option3 = "A crater in China",
                Option4 = "A city in Greenland",
                CorrectOptionNumber = 2,
            },
            Order = 1,
            PlayerStatisticsId = 1,
            PlayerStatistics = playerObject,
        });

        _context.PlayerStatistics.Add(playerObject);

        await _context.SaveChangesAsync();

        string playerName = "Åke Åkman";
        int questionAnswer = 4;

        // Act
        var response = await _service.CheckAnswer(playerName, questionAnswer);

        // Assert
        response.Should().NotBeNull();
        response.Message.Should().NotBeNull();
        response.Correct.Should().BeFalse();
        response.CorrectOption.Should().Be(2);
        response.Success.Should().BeTrue();
        response.Error.Should().BeNull();

        string message = response.Message!;
        message.Should().Be("Incorrect.");
        _context.PlayerStatistics.Should().NotBeEmpty();

        PlayerStatistics player = _context.PlayerStatistics.FirstOrDefault(p => p.Name == playerName)!;
        player.Should().NotBeNull();
        player.PlayerStatisticsFourOptionQuestions.Count.Should().Be(0);
        player.TotalAmountOfQuestions.Should().Be(1);
        player.CorrectAnswers.Should().Be(0);
    }

    [TestMethod]
    public async Task CheckAnswer_ExistingPlayer_HasNoQuestionsLeft_Should_Return_CheckAnswerCommandResponseFalse()
    {
        // Arrange
        _context.PlayerStatistics.Add(new()
        {
            Name = "Åke Åkman",
            CorrectAnswers = 1337,
            TotalAmountOfQuestions = 1337,
            PlayerStatisticsFourOptionQuestions = [],
        });

        await _context.SaveChangesAsync();

        string playerName = "Åke Åkman";
        int questionAnswer = 4;

        // Act
        var response = await _service.CheckAnswer(playerName, questionAnswer);

        // Assert
        response.Should().NotBeNull();
        response.Message.Should().BeNull();
        response.Correct.Should().BeFalse();
        response.CorrectOption.Should().BeNull();
        response.Success.Should().BeFalse();
        response.Error.Should().NotBeNull();

        Exception error = response.Error!;
        error.Message.Should().NotBeEmpty();
    }

    [TestMethod]
    public async Task CheckAnswer_UnexistingPlayer_Should_Return_CheckAnswerCommandResponseFalse()
    {
        // Arrange
        string playerName = "Kool-Kalle";
        int questionAnswer = 4;

        // Act
        var response = await _service.CheckAnswer(playerName, questionAnswer);

        // Assert
        response.Should().NotBeNull();
        response.Message.Should().BeNull();
        response.Correct.Should().BeFalse();
        response.CorrectOption.Should().BeNull();
        response.Success.Should().BeFalse();
        response.Error.Should().NotBeNull();

        Exception error = response.Error!;
        error.Message.Should().Be("No database entry matched the requested name.");
    }

    private void VerifyLog(LogLevel level)
    {
        _loggerMock.Verify(l => l.Log(
            It.Is<LogLevel>(logLevel => logLevel == level),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }
}