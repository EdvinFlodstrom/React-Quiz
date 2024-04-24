using Backend.Dtos;
using Backend.Handlers.Questions;
using Backend.Models;
using Backend.Models.QuestionTypes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuizController(IMediator mediator, ILogger<QuizController> logger) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<QuizController> _logger = logger;

    private const string BadRequestMessageTemplate = "Invalid request data";
    private const string WarningMessageTemplate = "Error: ";
    private const string ErrorMessageTemplate = "An unexpected error occured: ";

    [HttpPost("create/{questionType}")]
    public async Task<ActionResult<FourOptionQuestionDto>> CreateQuestion(string questionType, [FromBody] JsonElement fourOptionQuestionJson)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid request data: {ModelStateErrors}", ModelState.Values.SelectMany(v => v.Errors));
            return BadRequest(BadRequestMessageTemplate);
        }

        try
        {
            FourOptionQuestion fourOptionQuestion = DeserializeAndReturnQuestion(questionType, fourOptionQuestionJson);

            CreateQuestionCommand command = new()
            {
                FourOptionQuestion = fourOptionQuestion,
            };

            CreateQuestionCommandResponse response = await _mediator.Send(command);

            return VerifySuccessOrLogError(response.Success, response.Error)
                ? Ok(response.Question)
                : BadRequest(response.Error is not null ? response.Error.Message : ErrorMessageTemplate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessageTemplate + ex.Message);
            return StatusCode(500, ErrorMessageTemplate + ex.Message);
        }
    }

    private static FourOptionQuestion DeserializeAndReturnQuestion(string questionType, JsonElement fourOptionQuestionJson)
    {
        return questionType.ToLower() switch
        {
            "chemistry" => JsonSerializer.Deserialize<ChemistryQuestion>(fourOptionQuestionJson)
                                    ?? throw new NullReferenceException(nameof(fourOptionQuestionJson)),

            "food" => JsonSerializer.Deserialize<FoodQuestion>(fourOptionQuestionJson)
                                    ?? throw new NullReferenceException(nameof(fourOptionQuestionJson)),

            "game" => JsonSerializer.Deserialize<GameQuestion>(fourOptionQuestionJson)
                                    ?? throw new NullReferenceException(nameof(fourOptionQuestionJson)),

            "geography" => JsonSerializer.Deserialize<GeographyQuestion>(fourOptionQuestionJson)
                                    ?? throw new NullReferenceException(nameof(fourOptionQuestionJson)),

            "history" => JsonSerializer.Deserialize<HistoryQuestion>(fourOptionQuestionJson)
                                    ?? throw new NullReferenceException(nameof(fourOptionQuestionJson)),

            "literature" => JsonSerializer.Deserialize<LiteratureQuestion>(fourOptionQuestionJson)
                                    ?? throw new NullReferenceException(nameof(fourOptionQuestionJson)),

            "math" => JsonSerializer.Deserialize<MathQuestion>(fourOptionQuestionJson)
                                    ?? throw new NullReferenceException(nameof(fourOptionQuestionJson)),

            "music" => JsonSerializer.Deserialize<MusicQuestion>(fourOptionQuestionJson)
                                    ?? throw new NullReferenceException(nameof(fourOptionQuestionJson)),

            "sports" => JsonSerializer.Deserialize<SportsQuestion>(fourOptionQuestionJson)
                                    ?? throw new NullReferenceException(nameof(fourOptionQuestionJson)),

            "technology" => JsonSerializer.Deserialize<TechnologyQuestion>(fourOptionQuestionJson)
                                    ?? throw new NullReferenceException(nameof(fourOptionQuestionJson)),

            _ => throw new ArgumentException("Please verify that you chose a valid question type."),
        };
    }

    private bool VerifySuccessOrLogError(bool success, Exception? exception)
    {
        if (success)
            return true;

        _logger.LogWarning(WarningMessageTemplate + (exception is not null ? exception.Message : ""));

        return false;
    }
}
