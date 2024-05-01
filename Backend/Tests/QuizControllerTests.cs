using Backend.Controllers;
using Backend.Handlers.Questions;
using Backend.Infrastructure.Models.Dtos;
using Backend.Infrastructure.Models.Entities;
using Backend.Infrastructure.Models.Entities.QuestionTypes;
using Backend.Infrastructure.Models.Requests;
using Backend.Infrastructure.Validation.ValidatorFactory;
using Backend.Infrastructure.Validation.Validators;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;

namespace Tests;

[TestClass]
public class QuizControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly JsonSerializerOptions options;
    private readonly Mock<IQuestionValidatorFactory> _validatorFactoryMock;
    private readonly Mock<ILogger<QuizController>> _loggerMock;
    private readonly QuizController _controller;

    public QuizControllerTests()
    {
        _mediatorMock = new();
        options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        _validatorFactoryMock = new();
        _loggerMock = new();
        _controller = new(_mediatorMock.Object, options, _validatorFactoryMock.Object, _loggerMock.Object);
    }

    [TestMethod]
    public async Task CreateQuestion_Should_Return_FourOptionQuestionDto()
    {
        // Arrange
        string questionType = "Geography";
        MathQuestion questionRequest = new()
        {
            Question = "What is Eyjafjallajökull?",
            Option1 = "A glacier in Norway",
            Option2 = "A volcano on Iceland",
            Option3 = "A crater in China",
            Option4 = "A city on Greenland",
            CorrectOptionNumber = 2,
        };

        CreateQuestionCommandResponse commandResponse = new()
        {
            Question = new()
            {
                Question = questionRequest.Question,
                Option1 = questionRequest.Option1,
                Option2 = questionRequest.Option2,
                Option3 = questionRequest.Option3,
                Option4 = questionRequest.Option4,
            },
            Success = true,
            Error = null,
        };

        string jsonString = JsonSerializer.Serialize(questionRequest, options);
        JsonDocument jsonDocument = JsonDocument.Parse(jsonString);
        JsonElement fourOptionQuestionJson = jsonDocument.RootElement;

        IValidator<FourOptionQuestion> validator = new FourOptionQuestionValidator();

        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateQuestionCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(commandResponse);

        _validatorFactoryMock.Setup(v => v.GetValidator<FourOptionQuestion>())
            .Returns(validator);

        // Act
        var response = await _controller.CreateQuestion(questionType, fourOptionQuestionJson);
        ResetAllMocks();

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().NotBeNull();

        ObjectResult objectResult = (ObjectResult)response.Result!;
        objectResult.StatusCode.Should().Be(200);
        objectResult.Value.Should().NotBeNull();

        FourOptionQuestionDto question = (FourOptionQuestionDto)objectResult.Value!;
        question.Question.Should().Be("What is Eyjafjallajökull?");
        question.Option1.Should().Be("A glacier in Norway");
        question.Option2.Should().Be("A volcano on Iceland");
        question.Option3.Should().Be("A crater in China");
        question.Option4.Should().Be("A city on Greenland");
    }

    [TestMethod]
    public async Task CreateQuestion_QuestionNull_Should_Return_BadRequest()
    {
        // Arrange
        string questionType = "Geography";
        MathQuestion? questionRequest = null;

        string jsonString = JsonSerializer.Serialize(questionRequest, options);
        JsonDocument jsonDocument = JsonDocument.Parse(jsonString);
        JsonElement fourOptionQuestionJson = jsonDocument.RootElement;

        IValidator<FourOptionQuestion> validator = new FourOptionQuestionValidator();

        _validatorFactoryMock.Setup(v => v.GetValidator<FourOptionQuestion>())
            .Returns(validator);

        // Act
        var response = await _controller.CreateQuestion(questionType, fourOptionQuestionJson);
        ResetAllMocks();

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().NotBeNull();
        response.Result.Should().BeOfType<BadRequestObjectResult>();
        VerifyLog(LogLevel.Warning);
    }

    [TestMethod]
    public async Task CreateQuestion_QuestionInvalid_Should_Return_BadRequest()
    {
        // Arrange
        string questionType = "Geography";
        MathQuestion questionRequest = new()
        {
            Question = "",
            Option1 = "A glacier in Norway",
            Option2 = "",
            Option3 = "A crater in China",
            Option4 = "A city on Greenland",
            CorrectOptionNumber = 1337,
        };

        string jsonString = JsonSerializer.Serialize(questionRequest, options);
        JsonDocument jsonDocument = JsonDocument.Parse(jsonString);
        JsonElement fourOptionQuestionJson = jsonDocument.RootElement;

        IValidator<FourOptionQuestion> validator = new FourOptionQuestionValidator();

        _validatorFactoryMock.Setup(v => v.GetValidator<FourOptionQuestion>())
            .Returns(validator);

        // Act
        var response = await _controller.CreateQuestion(questionType, fourOptionQuestionJson);
        ResetAllMocks();

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().NotBeNull();
        response.Result.Should().BeOfType<BadRequestObjectResult>();
        VerifyLog(LogLevel.Warning);
    }

    [TestMethod]
    public async Task CreateQuestion_QuestionTypeNull_Should_Return_BadRequest()
    {
        // Arrange
        string? questionType = null;
        MathQuestion questionRequest = new()
        {
            Question = "What is Eyjafjallajökull?",
            Option1 = "A glacier in Norway",
            Option2 = "A volcano on Iceland",
            Option3 = "A crater in China",
            Option4 = "A city on Greenland",
            CorrectOptionNumber = 2
        };
        string jsonString = JsonSerializer.Serialize(questionRequest);
        JsonDocument jsonDocument = JsonDocument.Parse(jsonString);
        JsonElement fourOptionQuestionJson = jsonDocument.RootElement;

        // Act
#pragma warning disable CS8604 // Possible null reference argument.
        var response = await _controller.CreateQuestion(questionType, fourOptionQuestionJson);
#pragma warning restore CS8604 // Possible null reference argument.

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().NotBeNull();
        response.Result.Should().BeOfType<BadRequestObjectResult>();
        VerifyLog(LogLevel.Warning);
    }

    [TestMethod]
    public async Task CreateQuestion_QuestionTypeEmpty_Should_Return_BadRequest()
    {
        // Arrange
        string? questionType = "";
        MathQuestion questionRequest = new()
        {
            Question = "What is Eyjafjallajökull?",
            Option1 = "A glacier in Norway",
            Option2 = "A volcano on Iceland",
            Option3 = "A crater in China",
            Option4 = "A city on Greenland",
            CorrectOptionNumber = 2
        };
        string jsonString = JsonSerializer.Serialize(questionRequest);
        JsonDocument jsonDocument = JsonDocument.Parse(jsonString);
        JsonElement fourOptionQuestionJson = jsonDocument.RootElement;

        // Act
        var response = await _controller.CreateQuestion(questionType, fourOptionQuestionJson);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().NotBeNull();
        response.Result.Should().BeOfType<BadRequestObjectResult>();
        VerifyLog(LogLevel.Warning);
    }

    [TestMethod]
    public async Task CreateQuestion_QuestionTypeInvalid_Should_Return_BadRequest()
    {
        // Arrange
        string? questionType = "My special question type";
        MathQuestion questionRequest = new()
        {
            Question = "What is Eyjafjallajökull?",
            Option1 = "A glacier in Norway",
            Option2 = "A volcano on Iceland",
            Option3 = "A crater in China",
            Option4 = "A city on Greenland",
            CorrectOptionNumber = 2
        };
        string jsonString = JsonSerializer.Serialize(questionRequest);
        JsonDocument jsonDocument = JsonDocument.Parse(jsonString);
        JsonElement fourOptionQuestionJson = jsonDocument.RootElement;

        // Act
        var response = await _controller.CreateQuestion(questionType, fourOptionQuestionJson);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().NotBeNull();
        response.Result.Should().BeOfType<BadRequestObjectResult>();
        VerifyLog(LogLevel.Warning);
    }

    [TestMethod]
    public async Task PatchQuestion_Should_Return_FourOptionQuestion()
    {
        // Arrange 
        int questionId = 5;
        PatchQuestionRequest patchQuestionRequest = new()
        {
            Question = "My patched question!",
            Option1 = "My patched option 1!",
            Option2 = "My patched option 2!",
            Option3 = "My patched option 3!",
            Option4 = "My patched option 4!",
            CorrectOptionNumber = 4,
        };

        PatchQuestionCommandResponse commandResponse = new()
        {
            Question = new MathQuestion()
            {
                Question = patchQuestionRequest.Question,
                Option1 = patchQuestionRequest.Option1,
                Option2 = patchQuestionRequest.Option2,
                Option3 = patchQuestionRequest.Option3,
                Option4 = patchQuestionRequest.Option4,
                CorrectOptionNumber = Convert.ToInt32(patchQuestionRequest.CorrectOptionNumber), // Int? to Int
            },
            Success = true,
            Error = null,
        };

        IValidator<PatchQuestionRequest> validator = new PatchQuestionRequestValidator();

        _mediatorMock.Setup(m => m.Send(It.IsAny<PatchQuestionCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(commandResponse);

        _validatorFactoryMock.Setup(v => v.GetValidator<PatchQuestionRequest>())
            .Returns(validator);

        // Act
        var response = await _controller.PatchQuestion(questionId, patchQuestionRequest);
        ResetAllMocks();

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().NotBeNull();

        ObjectResult objectResult = (ObjectResult)response.Result!;
        objectResult.StatusCode.Should().Be(200);
        objectResult.Value.Should().NotBeNull();

        FourOptionQuestion question = (FourOptionQuestion)objectResult.Value!;
        question.Question.Should().Be("My patched question!");
        question.Option1.Should().Be("My patched option 1!");
        question.Option2.Should().Be("My patched option 2!");
        question.Option3.Should().Be("My patched option 3!");
        question.Option4.Should().Be("My patched option 4!");
        question.CorrectOptionNumber.Should().Be(4);
    }

    [TestMethod]
    public async Task PatchQuestion_PatchRequestInvalid_Should_Return_BadRequest()
    {
        // Arrange
        int questionId = 1;
        PatchQuestionRequest patchQuestionRequest = new()
        {
            Question = "",
        };

        IValidator<PatchQuestionRequest> validator = new PatchQuestionRequestValidator();

        _validatorFactoryMock.Setup(v => v.GetValidator<PatchQuestionRequest>())
            .Returns(validator);

        // Act
        var response = await _controller.PatchQuestion(questionId, patchQuestionRequest);
        ResetAllMocks();

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().NotBeNull();
        response.Result.Should().BeOfType<BadRequestObjectResult>();
        VerifyLog(LogLevel.Warning);
    }

    [TestMethod]
    public async Task PatchQuestion_PatchRequestNull_Should_Return_BadRequest()
    {
        // Arrange
        int questionId = 1;
        PatchQuestionRequest? patchQuestionRequest = null;

        // Act
#pragma warning disable CS8604 // Possible null reference argument.
        var response = await _controller.PatchQuestion(questionId, patchQuestionRequest);
#pragma warning restore CS8604 // Possible null reference argument.

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().NotBeNull();
        response.Result.Should().BeOfType<BadRequestObjectResult>();
        VerifyLog(LogLevel.Warning);
    }

    [TestMethod]
    public async Task PatchQuestion_InvalidQuestionId_Should_Return_BadRequest()
    {
        // Arrange
        int questionId = -1337;
        PatchQuestionRequest patchQuestionRequest = new()
        {
            Question = "Patched question!",
        };

        PatchQuestionCommandResponse commandResponse = new()
        {
            Question = null,
            Success = false,
            Error = new ArgumentException("A question with the provided ID does not exist in the database."),
        };

        IValidator<PatchQuestionRequest> validator = new PatchQuestionRequestValidator();

        _mediatorMock.Setup(m => m.Send(It.IsAny<PatchQuestionCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(commandResponse);

        _validatorFactoryMock.Setup(v => v.GetValidator<PatchQuestionRequest>())
            .Returns(validator);

        // Act
        var response = await _controller.PatchQuestion(questionId, patchQuestionRequest);
        ResetAllMocks();

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().NotBeNull();
        response.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    private void VerifyLog(LogLevel level)
    {
        _loggerMock.Verify(l => l.Log(
            It.Is<LogLevel>(loglevel => loglevel == level),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    private void ResetAllMocks()
    {
        _mediatorMock.Reset();
        _validatorFactoryMock.Reset();
    }
}