using Backend.Handlers.Questions;
using Backend.Handlers.Quiz;
using Backend.Models.Dtos;
using Backend.Models.Entities;
using Backend.Models.Entities.QuestionTypes;
using Backend.Models.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuizController(IMediator mediator, JsonSerializerOptions jsonSerializerOptions, ILogger<QuizController> logger) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly JsonSerializerOptions _serializerOptions = jsonSerializerOptions;
    private readonly ILogger<QuizController> _logger = logger;

    private const string BadRequestMessageTemplate = "Invalid request data";
    private const string WarningMessageTemplate = "Error: ";
    private const string ErrorMessageTemplate = "An unexpected error occured: ";

    [HttpPost("get")]
    public async Task<ActionResult<FourOptionQuestionDto>> GetQuestion([FromBody] GetQuestionRequest request)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid request data: {ModelStateErrors}", ModelState.Values.SelectMany(v => v.Errors));
            return BadRequest(BadRequestMessageTemplate);
        }

        try
        {
            GetQuestionCommand command = new()
            {
                PlayerName = request.PlayerName,
            };

            var response = await _mediator.Send(command);

            return response.Success == true
                ? response.Question is not null
                ? Ok(response.Question)
                : Ok(response.Details)
                : BadRequest(response.Error is not null ? response.Error.Message : ErrorMessageTemplate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessageTemplate + ex.Message);
            return StatusCode(500, ex + ErrorMessageTemplate + ex.Message);
        }
    }

    [HttpPost("initialize")]
    public async Task<ActionResult<string>> InitializeQuiz([FromBody] InitializeQuizRequest request)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid request data: {ModelStateErrors}", ModelState.Values.SelectMany(v => v.Errors));
            return BadRequest(BadRequestMessageTemplate);
        }

        try
        {
            InitializeQuizCommand command = new()
            {
                PlayerName = request.PlayerName,
                AmountOfQuestions = request.AmountOfQuestions,
                QuestionType = request.QuestionType,
            };

            var response = await _mediator.Send(command);

            return response.Success
                ? Ok(response.Details)
                : BadRequest(response.Error is not null ? response.Error.Message : ErrorMessageTemplate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessageTemplate + ex.Message);
            return StatusCode(500, ErrorMessageTemplate + ex.Message);
        }
    }

    [HttpGet("getMany/{numberOfQuestions:int}")]
    public async Task<ActionResult<List<FourOptionQuestion>>> GetManyQuestions(int numberOfQuestions, [FromQuery] string? questionType)
    {
        if (numberOfQuestions <= 0)
            return BadRequest(BadRequestMessageTemplate);

        try
        {
            GetManyQuestionsCommand command = new()
            {
                NumberOfQuestions = numberOfQuestions,
                QuestionType = questionType,
            };

            var response = await _mediator.Send(command);

            return VerifySuccessOrLogError(response.Success, response.Error)
                ? Ok(response.Questions)
                : BadRequest(response.Error is not null ? response.Error.Message : ErrorMessageTemplate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessageTemplate + ex.Message);
            return StatusCode(500, ErrorMessageTemplate + ex.Message);
        }
    }

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

            var response = await _mediator.Send(command);

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

    [HttpDelete("delete/{questionId:int}")]
    public async Task<ActionResult<FourOptionQuestion>> DeleteQuestion(int questionId)
    {
        try
        {
            DeleteQuestionCommand request = new()
            {
                QuestionId = questionId,
            };

            var response = await _mediator.Send(request);

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

    private FourOptionQuestion DeserializeAndReturnQuestion(string questionType, JsonElement fourOptionQuestionJson)
    {
        return questionType.ToLower() switch
        {
            "chemistry" => JsonSerializer.Deserialize<ChemistryQuestion>(fourOptionQuestionJson, _serializerOptions)
                                    ?? throw new NullReferenceException(nameof(fourOptionQuestionJson)),

            "food" => JsonSerializer.Deserialize<FoodQuestion>(fourOptionQuestionJson, _serializerOptions)
                                    ?? throw new NullReferenceException(nameof(fourOptionQuestionJson)),

            "game" => JsonSerializer.Deserialize<GameQuestion>(fourOptionQuestionJson, _serializerOptions)
                                    ?? throw new NullReferenceException(nameof(fourOptionQuestionJson)),

            "geography" => JsonSerializer.Deserialize<GeographyQuestion>(fourOptionQuestionJson, _serializerOptions)
                                    ?? throw new NullReferenceException(nameof(fourOptionQuestionJson)),

            "history" => JsonSerializer.Deserialize<HistoryQuestion>(fourOptionQuestionJson, _serializerOptions)
                                    ?? throw new NullReferenceException(nameof(fourOptionQuestionJson)),

            "literature" => JsonSerializer.Deserialize<LiteratureQuestion>(fourOptionQuestionJson, _serializerOptions)
                                    ?? throw new NullReferenceException(nameof(fourOptionQuestionJson)),

            "math" => JsonSerializer.Deserialize<MathQuestion>(fourOptionQuestionJson, _serializerOptions)
                                    ?? throw new NullReferenceException(nameof(fourOptionQuestionJson)),

            "music" => JsonSerializer.Deserialize<MusicQuestion>(fourOptionQuestionJson, _serializerOptions)
                                    ?? throw new NullReferenceException(nameof(fourOptionQuestionJson)),

            "sports" => JsonSerializer.Deserialize<SportsQuestion>(fourOptionQuestionJson, _serializerOptions)
                                    ?? throw new NullReferenceException(nameof(fourOptionQuestionJson)),

            "technology" => JsonSerializer.Deserialize<TechnologyQuestion>(fourOptionQuestionJson, _serializerOptions)
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
