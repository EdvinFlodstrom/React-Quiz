using Backend.Handlers.Questions;
using Backend.Infrastructure.Models.Dtos;
using Backend.Infrastructure.Models.Entities;
using Backend.Infrastructure.Models.Entities.QuestionTypes;
using Backend.Services;
using FluentAssertions;
using Moq;

namespace Tests;

[TestClass]
public class HandlerTests
{
    private readonly Mock<IQuizService> _service;

    public HandlerTests()
    {
        _service = new Mock<IQuizService>();
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

        _service.Setup(s => s.CreateQuestion(It.IsAny<FourOptionQuestion>()))
            .ReturnsAsync(createQuestionCommandResponse);

        CreateQuestionCommandHandler handler = new(_service.Object);

        // Act
        var response = await handler.Handle(createQuestionCommand, CancellationToken.None);
        _service.Reset();

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
}