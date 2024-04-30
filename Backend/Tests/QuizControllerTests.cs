using Backend.Controllers;
using Backend.Handlers.Questions;
using Backend.Infrastructure.Models.Dtos;
using Backend.Infrastructure.Models.Entities.QuestionTypes;
using Backend.Infrastructure.Validation.ValidatorFactory;
using FluentAssertions;
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
    private readonly Mock<IValidatorFactory> _validatorFactory;
    private readonly Mock<ILogger<QuizController>> _loggerMock;
    private readonly QuizController _controller;

    public QuizControllerTests()
    {
        _mediatorMock = new();
        options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        _validatorFactory = new();
        _loggerMock = new();
        _controller = new(_mediatorMock.Object, options, _validatorFactory.Object, _loggerMock.Object);
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

        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateQuestionCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(commandResponse);

        // Act
        var response = await _controller.CreateQuestion(questionType, fourOptionQuestionJson);
        _mediatorMock.Reset();

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().NotBeNull();
        response.Result.Should().BeOfType<OkObjectResult>();

        ObjectResult objectResult = (ObjectResult)response.Result!;
        objectResult.StatusCode.Should().Be(200);

        FourOptionQuestionDto question = (FourOptionQuestionDto)objectResult.Value!;
        question.Should().NotBeNull();
        question.Question.Should().Be("What is Eyjafjallajökull?");
        question.Option1.Should().Be("A glacier in Norway");
        question.Option2.Should().Be("A volcano on Iceland");
        question.Option3.Should().Be("A crater in China");
        question.Option4.Should().Be("A city on Greenland");
    }

    [TestMethod]
    public async Task CreateQuestion_QuestionInvalid_Should_Return_()
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
            CorrectOptionNumber = 1400,
        };

        string jsonString = JsonSerializer.Serialize(questionRequest, options);
        JsonDocument jsonDocument = JsonDocument.Parse(jsonString);
        JsonElement fourOptionQuestionJson = jsonDocument.RootElement;

        // Act
        var response = await _controller.CreateQuestion(questionType, fourOptionQuestionJson);

        // Assert
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

        ObjectResult objectResult = (ObjectResult)response.Result!;
        objectResult.StatusCode.Should().Be(400);
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

        ObjectResult objectResult = (ObjectResult)response.Result!;
        objectResult.StatusCode.Should().Be(400);
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

        ObjectResult objectResult = (ObjectResult)response.Result!;
        objectResult.StatusCode.Should().Be(400);
    }
}