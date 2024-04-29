using Backend.Controllers;
using Backend.Handlers.Questions;
using Backend.Models.Entities.QuestionTypes;
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
    private readonly Mock<ILogger<QuizController>> _loggerMock;
    private readonly QuizController _controller;

    public QuizControllerTests()
    {
        _mediatorMock = new();
        options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        _loggerMock = new();
        _controller = new(_mediatorMock.Object, options, _loggerMock.Object);
    }

    [TestMethod]
    public async Task CreateQuestion_Should_Return_FourOptionQuestionDto()
    {
        //Arrange
        string questionType = "Math";
        MathQuestion question = new()
        {
            Question = "What is Eyjafjallajökull?",
            Option1 = "A glacier in Norway",
            Option2 = "A volcano on Iceland",
            Option3 = "A crater in China",
            Option4 = "A city on Greenland",
            CorrectOptionNumber = 2
        };

        CreateQuestionCommandResponse questionResponse = new()
        {
            Question = new()
            {
                Question = question.Question,
                Option1 = question.Option1,
                Option2 = question.Option2,
                Option3 = question.Option3,
                Option4 = question.Option4,
            },
            Success = true,
            Error = null,
        };

        string jsonString = JsonSerializer.Serialize(question);
        JsonDocument jsonDocument = JsonDocument.Parse(jsonString);
        JsonElement fourOptionQuestionJson = jsonDocument.RootElement;

        CreateQuestionCommand command = new()
        { FourOptionQuestion = question };

        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateQuestionCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(questionResponse);

        //Act
        var response = await _controller.CreateQuestion(questionType, fourOptionQuestionJson);

        //Assert
        response.Should().NotBeNull();
        response.Result.Should().NotBeNull();
        response.Result.Should().BeOfType<ObjectResult>();

        // Add additional assertions, related to response.Result
    }
}