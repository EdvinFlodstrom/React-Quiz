using Backend.Handlers.Questions;
using Backend.Handlers.Quiz;
using Backend.Infrastructure.Models.Dtos;
using Backend.Infrastructure.Models.Entities;
using Backend.Infrastructure.Models.Entities.QuestionTypes;
using Backend.Infrastructure.Models.Requests;
using Backend.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Tests;

[TestClass]
public class HandlerTests
{
    private readonly Mock<IQuizService> _serviceMock;

    public HandlerTests()
    {
        _serviceMock = new Mock<IQuizService>();
    }

    [TestMethod]
    public async Task CreateQuestionCommand_Should_Return_Response()
    {
        // Arrange
        CreateQuestionCommand createQuestionCommand = new()
        {
            FourOptionQuestion = new GeographyQuestion()
            {
                Question = "What is Eyjafjallajökull?",
                Option1 = "A glacier in Norway",
                Option2 = "A volcano on Iceland",
                Option3 = "A crater in China",
                Option4 = "A city in Greenland",
                CorrectOptionNumber = 2,
            },
        };

        CreateQuestionCommandResponse createQuestionCommandResponse = new()
        {
            Question = new()
            {
                Question = "What is Eyjafjallajökull?",
                Option1 = "A glacier in Norway",
                Option2 = "A volcano on Iceland",
                Option3 = "A crater in China",
                Option4 = "A city in Greenland",
            },
            Success = true,
            Error = null,
        };

        _serviceMock.Setup(s => s.CreateQuestion(It.IsAny<FourOptionQuestion>()))
            .ReturnsAsync(createQuestionCommandResponse);

        CreateQuestionCommandHandler handler = new(_serviceMock.Object);

        // Act
        var response = await handler.Handle(createQuestionCommand, CancellationToken.None);
        _serviceMock.Reset();

        // Assert
        response.Should().NotBeNull();
        response.Question.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.Error.Should().BeNull();

        FourOptionQuestionDto question = response.Question!;
        question.Question.Should().Be("What is Eyjafjallajökull?");
        question.Option1.Should().Be("A glacier in Norway");
        question.Option2.Should().Be("A volcano on Iceland");
        question.Option3.Should().Be("A crater in China");
        question.Option4.Should().Be("A city in Greenland");
    }

    [TestMethod]
    public async Task CreateQuestionCommand_Error_Should_Return_ResponseFalse()
    {
        // Arrange
        CreateQuestionCommand createQuestionCommand = new()
        {
            FourOptionQuestion = new GeographyQuestion()
            {
                Question = "What is Eyjafjallajökull?",
                Option1 = "A glacier in Norway",
                Option2 = "A volcano on Iceland",
                Option3 = "A crater in China",
                Option4 = "A city in Greenland",
                CorrectOptionNumber = 2,
            },
        };

        CreateQuestionCommandResponse createQuestionCommandResponse = new()
        {
            Question = null,
            Success = false,
            Error = new DbUpdateException("Database error."),
        };

        _serviceMock.Setup(s => s.CreateQuestion(It.IsAny<FourOptionQuestion>()))
            .ReturnsAsync(createQuestionCommandResponse);

        CreateQuestionCommandHandler handler = new(_serviceMock.Object);

        // Act
        var response = await handler.Handle(createQuestionCommand, CancellationToken.None);
        _serviceMock.Reset();

        // Assert
        response.Should().NotBeNull();
        response.Question.Should().BeNull();
        response.Success.Should().BeFalse();
        response.Error.Should().NotBeNull();

        Exception error = response.Error!;
        error.Message.Should().Be("Database error.");
    }

    [TestMethod]
    public async Task PatchQuestionCommand_Should_Return_Response()
    {
        // Arrange
        PatchQuestionCommand patchQuestionCommand = new()
        {
            QuestionId = 1,
            Request = new()
            {
                Question = "Patched question!",
                Option1 = "Patched option 1!",
                Option2 = "Patched option 2!",
                Option3 = "Patched option 3!",
                Option4 = "Patched option 4!",
                CorrectOptionNumber = 4,
            },
        };

        PatchQuestionCommandResponse patchQuestionCommandResponse = new()
        {
            Question = new GeographyQuestion()
            {
                Question = "Patched question!",
                Option1 = "Patched option 1!",
                Option2 = "Patched option 2!",
                Option3 = "Patched option 3!",
                Option4 = "Patched option 4!",
                CorrectOptionNumber = 4,
            },
            Success = true,
            Error = null,
        };

        _serviceMock.Setup(s => s.PatchQuestion(It.IsAny<int>(), It.IsAny<PatchQuestionRequest>()))
            .ReturnsAsync(patchQuestionCommandResponse);

        PatchQuestionCommandHandler handler = new(_serviceMock.Object);

        // Act
        var response = await handler.Handle(patchQuestionCommand, CancellationToken.None);
        _serviceMock.Reset();

        // Assert
        response.Should().NotBeNull();
        response.Question.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.Error.Should().BeNull();

        FourOptionQuestion question = response.Question!;
        question.Should().BeOfType<GeographyQuestion>();
        question.Question.Should().Be("Patched question!");
        question.Option1.Should().Be("Patched option 1!");
        question.Option2.Should().Be("Patched option 2!");
        question.Option3.Should().Be("Patched option 3!");
        question.Option4.Should().Be("Patched option 4!");
        question.CorrectOptionNumber.Should().Be(4);
    }

    [TestMethod]
    public async Task PatchQuestionCommand_Error_Should_Return_ResponseFalse()
    {
        // Arrange
        PatchQuestionCommand patchQuestionCommand = new()
        {
            QuestionId = 1,
            Request = new()
            {
                Question = "Patched question!",
            },
        };

        PatchQuestionCommandResponse patchQuestionCommandResponse = new()
        {
            Question = null,
            Success = false,
            Error = new DbUpdateException("Database error."),
        };

        _serviceMock.Setup(s => s.PatchQuestion(It.IsAny<int>(), It.IsAny<PatchQuestionRequest>()))
            .ReturnsAsync(patchQuestionCommandResponse);

        PatchQuestionCommandHandler handler = new(_serviceMock.Object);

        // Act
        var response = await handler.Handle(patchQuestionCommand, CancellationToken.None);
        _serviceMock.Reset();

        // Assert
        response.Should().NotBeNull();
        response.Question.Should().BeNull();
        response.Success.Should().BeFalse();
        response.Error.Should().NotBeNull();

        Exception error = response.Error!;
        error.Message.Should().Be("Database error.");
    }

    [TestMethod]
    public async Task DeleteQuestionCommand_Should_Return_Response()
    {
        // Arrange
        DeleteQuestionCommand deleteQuestionCommand = new()
        {
            QuestionId = 1,
        };

        DeleteQuestionCommandResponse deleteQuestionCommandResponse = new()
        {
            Question = new GeographyQuestion()
            {
                Question = "What is Eyjafjallajökull?",
                Option1 = "A glacier in Norway",
                Option2 = "A volcano on Iceland",
                Option3 = "A crater in China",
                Option4 = "A city in Greenland",
                CorrectOptionNumber = 2,
            },
            Success = true,
            Error = null,
        };

        _serviceMock.Setup(s => s.DeleteQuestion(It.IsAny<int>()))
            .ReturnsAsync(deleteQuestionCommandResponse);

        DeleteQuestionCommandHandler handler = new(_serviceMock.Object);

        // Act
        var response = await handler.Handle(deleteQuestionCommand, CancellationToken.None);
        _serviceMock.Reset();

        // Assert
        response.Should().NotBeNull();
        response.Question.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.Error.Should().BeNull();

        FourOptionQuestion question = response.Question!;
        question.Question.Should().Be("What is Eyjafjallajökull?");
        question.Option1.Should().Be("A glacier in Norway");
        question.Option2.Should().Be("A volcano on Iceland");
        question.Option3.Should().Be("A crater in China");
        question.Option4.Should().Be("A city in Greenland");
        question.CorrectOptionNumber.Should().Be(2);
    }

    [TestMethod]
    public async Task DeleteQuestionCommand_Error_Should_Return_ResponseFalse()
    {
        // Arrange
        DeleteQuestionCommand deleteQuestionCommand = new()
        {
            QuestionId = 1,
        };

        DeleteQuestionCommandResponse deleteQuestionCommandResponse = new()
        {
            Question = null,
            Success = false,
            Error = new DbUpdateException("Database error."),
        };

        _serviceMock.Setup(s => s.DeleteQuestion(It.IsAny<int>()))
            .ReturnsAsync(deleteQuestionCommandResponse);

        DeleteQuestionCommandHandler handler = new(_serviceMock.Object);

        // Act
        var response = await handler.Handle(deleteQuestionCommand, CancellationToken.None);
        _serviceMock.Reset();

        // Assert
        response.Should().NotBeNull();
        response.Question.Should().BeNull();
        response.Success.Should().BeFalse();
        response.Error.Should().NotBeNull();

        Exception error = response.Error!;
        error.Message.Should().Be("Database error.");
    }

    [TestMethod]
    public async Task GetManyQuestionsCommand_Should_Return_Response()
    {
        // Arrange
        GetManyQuestionsCommand getManyQuestionsCommand = new()
        {
            NumberOfQuestions = 1,
            QuestionType = "Geography",
        };

        GetManyQuestionsCommandResponse getManyQuestionsCommandResponse = new()
        {
            Questions =
        [
            new GeographyQuestion()
            {
                Question = "What is Eyjafjallajökull?",
                Option1 = "A glacier in Norway",
                Option2 = "A volcano on Iceland",
                Option3 = "A crater in China",
                Option4 = "A city in Greenland",
                CorrectOptionNumber = 2,
            },
        ],
            Success = true,
            Error = null,
        };

        _serviceMock.Setup(s => s.GetManyQuestions(It.IsAny<int>(), It.IsAny<string>()))
            .Returns(getManyQuestionsCommandResponse);

        GetManyQuestionsCommandHandler handler = new(_serviceMock.Object);

        // Act
        var response = await handler.Handle(getManyQuestionsCommand, CancellationToken.None);
        _serviceMock.Reset();

        // Assert
        response.Questions.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.Error.Should().BeNull();

        List<FourOptionQuestion> questions = response.Questions!;
        questions.First().Question.Should().Be("What is Eyjafjallajökull?");
        questions.First().Option1.Should().Be("A glacier in Norway");
        questions.First().Option2.Should().Be("A volcano on Iceland");
        questions.First().Option3.Should().Be("A crater in China");
        questions.First().Option4.Should().Be("A city in Greenland");
        questions.First().CorrectOptionNumber.Should().Be(2);
    }

    [TestMethod]
    public async Task GetManyQuestionsCommand_Error_Should_Return_ResponseFalse()
    {
        // Arrange
        GetManyQuestionsCommand getManyQuestionsCommand = new()
        {
            NumberOfQuestions = 1,
            QuestionType = "Geography",
        };

        GetManyQuestionsCommandResponse getManyQuestionsCommandResponse = new()
        {
            Questions = null,
            Success = false,
            Error = new DbUpdateException("Database error."),
        };

        _serviceMock.Setup(s => s.GetManyQuestions(It.IsAny<int>(), It.IsAny<string>()))
            .Returns(getManyQuestionsCommandResponse);

        GetManyQuestionsCommandHandler handler = new(_serviceMock.Object);

        // Act
        var response = await handler.Handle(getManyQuestionsCommand, CancellationToken.None);
        _serviceMock.Reset();

        // Assert
        response.Should().NotBeNull();
        response.Questions.Should().BeNull();
        response.Success.Should().BeFalse();
        response.Error.Should().NotBeNull();

        Exception error = response.Error!;
        error.Message.Should().Be("Database error.");
    }

    [TestMethod]
    public async Task InitializeQuizCommand_Should_Return_Response()
    {
        // Arrange
        InitializeQuizCommand initializeQuizCommand = new()
        {
            PlayerName = "Åke",
            AmountOfQuestions = 1,
            QuestionType = "Geography",
        };

        InitializeQuizCommandResponse initializeQuizCommandResponse = new()
        {
            Details = "Quiz has been initialized successfully for player Åke",
            Success = true,
            Error = null,
        };

        _serviceMock.Setup(s => s.InitializeQuiz(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(initializeQuizCommandResponse);

        InitializeQuizCommandHandler handler = new(_serviceMock.Object);

        // Act
        var response = await handler.Handle(initializeQuizCommand, CancellationToken.None);
        _serviceMock.Reset();

        // Assert
        response.Should().NotBeNull();
        response.Details.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.Error.Should().BeNull();

        string details = response.Details!;
        details.Should().Be("Quiz has been initialized successfully for player Åke");
    }

    [TestMethod]
    public async Task InitializeQuizCommand_Error_Should_Return_ResponseFalse()
    {
        // Arrange
        InitializeQuizCommand initializeQuizCommand = new()
        {
            PlayerName = "Åke",
            AmountOfQuestions = 1,
            QuestionType = "Geography",
        };

        InitializeQuizCommandResponse initializeQuizCommandResponse = new()
        {
            Details = null,
            Success = false,
            Error = new DbUpdateException("Database error."),
        };

        _serviceMock.Setup(s => s.InitializeQuiz(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())) // string or string? See above.
            .ReturnsAsync(initializeQuizCommandResponse);

        InitializeQuizCommandHandler handler = new(_serviceMock.Object);

        // Act
        var response = await handler.Handle(initializeQuizCommand, CancellationToken.None);
        _serviceMock.Reset();

        // Assert
        response.Should().NotBeNull();
        response.Details.Should().BeNull();
        response.Success.Should().BeFalse();
        response.Error.Should().NotBeNull();

        Exception error = response.Error!;
        error.Message.Should().Be("Database error.");
    }

    [TestMethod]
    public async Task GetQuestionCommand_QuestionsLeft_Should_Return_Response()
    {
        // Arrange
        GetQuestionCommand getQuestionCommand = new()
        {
            PlayerName = "Åke",
        };

        GetQuestionCommandResponse getQuestionCommandResponse = new()
        {
            Question = new FourOptionQuestionDto()
            {
                Question = "What is Eyjafjallajökull?",
                Option1 = "A glacier in Norway",
                Option2 = "A volcano on Iceland",
                Option3 = "A crater in China",
                Option4 = "A city in Greenland",
            },
            Details = null,
            Success = true,
            Error = null,
        };

        _serviceMock.Setup(s => s.GetQuestion(It.IsAny<string>()))
            .Returns(getQuestionCommandResponse);

        GetQuestionCommandHandler handler = new(_serviceMock.Object);

        // Act
        var response = await handler.Handle(getQuestionCommand, CancellationToken.None);
        _serviceMock.Reset();

        // Assert
        response.Should().NotBeNull();
        response.Question.Should().NotBeNull();
        response.Details.Should().BeNull();
        response.Success.Should().BeTrue();
        response.Error.Should().BeNull();

        FourOptionQuestionDto question = response.Question!;
        question.Question.Should().Be("What is Eyjafjallajökull?");
        question.Option1.Should().Be("A glacier in Norway");
        question.Option2.Should().Be("A volcano on Iceland");
        question.Option3.Should().Be("A crater in China");
        question.Option4.Should().Be("A city in Greenland");
    }

    [TestMethod]
    public async Task GetQuestionCommand_NoQuestionsLeft_Should_Return_Response()
    {
        // Arrange
        GetQuestionCommand getQuestionCommand = new()
        {
            PlayerName = "Åke",
        };

        GetQuestionCommandResponse getQuestionCommandResponse = new()
        {
            Question = null,
            Details = "That's all the questions. Thanks for playing!",
            Success = true,
            Error = null,
        };

        _serviceMock.Setup(s => s.GetQuestion(It.IsAny<string>()))
            .Returns(getQuestionCommandResponse);

        GetQuestionCommandHandler handler = new(_serviceMock.Object);

        // Act
        var response = await handler.Handle(getQuestionCommand, CancellationToken.None);
        _serviceMock.Reset();

        // Assert
        response.Should().NotBeNull();
        response.Question.Should().BeNull();
        response.Details.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.Error.Should().BeNull();

        string details = response.Details!;
        details.Should().Be("That's all the questions. Thanks for playing!");
    }

    [TestMethod]
    public async Task GetQuestionCommand_Error_Should_Return_ResponseFalse()
    {
        // Arrange
        GetQuestionCommand getQuestionCommand = new()
        {
            PlayerName = "Åke",
        };

        GetQuestionCommandResponse getQuestionCommandResponse = new()
        {
            Question = null,
            Details = null,
            Success = false,
            Error = new DbUpdateException("Database error."),
        };

        _serviceMock.Setup(s => s.GetQuestion(It.IsAny<string>()))
            .Returns(getQuestionCommandResponse);

        GetQuestionCommandHandler handler = new(_serviceMock.Object);

        // Act
        var response = await handler.Handle(getQuestionCommand, CancellationToken.None);
        _serviceMock.Reset();

        // Assert
        response.Should().NotBeNull();
        response.Question.Should().BeNull();
        response.Details.Should().BeNull();
        response.Success.Should().BeFalse();
        response.Error.Should().NotBeNull();

        Exception error = response.Error!;
        error.Message.Should().Be("Database error.");
    }

    [TestMethod]
    public async Task GetQuestionById_Should_Return_Response()
    {
        // Arrange
        GetQuestionByIdCommand getQuestionByIdCommand = new()
        {
            QuestionId = 4,
        };

        GetQuestionByIdCommandResponse getQuestionByIdCommandResponse = new()
        {
            Question = new()
            {
                QuestionType = "Geography",
                Question = "What is Eyjafjallajökull?",
                Option1 = "A glacier in Norway",
                Option2 = "A volcano on Iceland",
                Option3 = "A crater in China",
                Option4 = "A city in Greenland",
                CorrectOptionNumber = 2,
            },
            Success = true,
            Error = null,
        };

        _serviceMock.Setup(s => s.GetQuestionById(It.IsAny<int>()))
            .ReturnsAsync(getQuestionByIdCommandResponse);

        GetQuestionByIdCommandHandler handler = new(_serviceMock.Object);

        // Act
        var response = await handler.Handle(getQuestionByIdCommand, CancellationToken.None);
        _serviceMock.Reset();

        // Assert
        response.Should().NotBeNull();
        response.Question.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.Error.Should().BeNull();

        FourOptionQuestionByIdDto question = response.Question!;
        question.Question.Should().Be("What is Eyjafjallajökull?");
        question.Option1.Should().Be("A glacier in Norway");
        question.Option2.Should().Be("A volcano on Iceland");
        question.Option3.Should().Be("A crater in China");
        question.Option4.Should().Be("A city in Greenland");
        question.CorrectOptionNumber.Should().Be(2);
    }

    [TestMethod]
    public async Task GetQuestionById_QuestionIdNotFound_Should_Return_ResponseFalse()
    {
        // Arrange
        GetQuestionByIdCommand getQuestionByIdCommand = new()
        {
            QuestionId = 1337,
        };

        GetQuestionByIdCommandResponse getQuestionByIdCommandResponse = new()
        {
            Question = null,
            Success = false,
            Error = new ArgumentException("No question with the requested ID exists in the database."),
        };

        _serviceMock.Setup(s => s.GetQuestionById(It.IsAny<int>()))
            .ReturnsAsync(getQuestionByIdCommandResponse);

        GetQuestionByIdCommandHandler handler = new(_serviceMock.Object);

        // Act
        var response = await handler.Handle(getQuestionByIdCommand, CancellationToken.None);
        _serviceMock.Reset();

        // Assert
        response.Should().NotBeNull();
        response.Question.Should().BeNull();
        response.Success.Should().BeFalse();
        response.Error.Should().NotBeNull();

        Exception error = response.Error!;
        error.Message.Should().Be("No question with the requested ID exists in the database.");
    }

    [TestMethod]
    public async Task CheckAnswerCommand_QuestionsLeft_Should_Return_Response()
    {
        // Arrange
        CheckAnswerCommand checkAnswerCommand = new()
        {
            PlayerName = "Åke",
            QuestionAnswer = 4,
        };

        CheckAnswerCommandResponse checkAnswerCommandResponse = new()
        {
            Message = "Correct!",
            Success = true,
            Error = null,
        };

        _serviceMock.Setup(s => s.CheckAnswer(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(checkAnswerCommandResponse);

        CheckAnswerCommandHandler handler = new(_serviceMock.Object);

        // Act
        var response = await handler.Handle(checkAnswerCommand, CancellationToken.None);
        _serviceMock.Reset();

        // Assert
        response.Should().NotBeNull();
        response.Message.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.Error.Should().BeNull();

        string message = response.Message!;
        message.Should().Be("Correct!");
    }

    [TestMethod]
    public async Task CheckAnswerCommand_NoQuestionsLeft_Should_Return_ResponseFalse()
    {
        // Arrange
        CheckAnswerCommand checkAnswerCommand = new()
        {
            PlayerName = "Åke",
            QuestionAnswer = 4,
        };

        CheckAnswerCommandResponse checkAnswerCommandResponse = new()
        {
            Message = null,
            Success = false,
            Error = new ArgumentException("You've already answered all your questions."),
        };

        _serviceMock.Setup(s => s.CheckAnswer(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(checkAnswerCommandResponse);

        CheckAnswerCommandHandler handler = new(_serviceMock.Object);

        // Act
        var response = await handler.Handle(checkAnswerCommand, CancellationToken.None);
        _serviceMock.Reset();

        // Assert
        response.Should().NotBeNull();
        response.Message.Should().BeNull();
        response.Success.Should().BeFalse();
        response.Error.Should().NotBeNull();

        Exception error = response.Error!;
        error.Message.Should().Be("You've already answered all your questions.");
    }

    [TestMethod]
    public async Task CheckAnswerCommandError_Should_Return_ResponseFalse()
    {
        // Arrange
        CheckAnswerCommand checkAnswerCommand = new()
        {
            PlayerName = "Åke",
            QuestionAnswer = 4,
        };

        CheckAnswerCommandResponse checkAnswerCommandResponse = new()
        {
            Message = null,
            Success = false,
            Error = new DbUpdateException("Database error."),
        };

        _serviceMock.Setup(s => s.CheckAnswer(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(checkAnswerCommandResponse);

        CheckAnswerCommandHandler handler = new(_serviceMock.Object);

        // Act
        var response = await handler.Handle(checkAnswerCommand, CancellationToken.None);
        _serviceMock.Reset();

        // Assert
        response.Should().NotBeNull();
        response.Message.Should().BeNull();
        response.Success.Should().BeFalse();
        response.Error.Should().NotBeNull();

        Exception error = response.Error!;
        error.Message.Should().Be("Database error.");
    }
}