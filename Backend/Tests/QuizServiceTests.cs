using AutoMapper;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Models.Entities.QuestionTypes;
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
    private IQuizService _service;

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
    public async Task CreateQuestion_Should_Return_CreateQuestionCommandResponse()
    {
        // Arrange



        // Act



        // Assert

    }



    // _context.X.Add(new() { });
    // await _context.SaveChangesAsync();
    // No need for 'using'

    private void VerifyLog(LogLevel level)
    {
        _loggerMock.Verify(l => l.Log(
            It.Is<LogLevel>(logLevel => logLevel == level),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }
    // Create
    // Patch
    // Delete
    // GetMany
    // Initialize
    // GetSingle
    // CheckAnswer
}