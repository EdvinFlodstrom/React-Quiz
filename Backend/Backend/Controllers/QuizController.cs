using Backend.Handlers.Questions;
using Backend.Handlers.Quiz;
using Backend.Infrastructure.Models.Dtos;
using Backend.Infrastructure.Models.Entities;
using Backend.Infrastructure.Models.Entities.QuestionTypes;
using Backend.Infrastructure.Models.Requests;
using Backend.Infrastructure.Validation.ValidatorFactory;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuizController(IMediator mediator, JsonSerializerOptions jsonSerializerOptions, IQuestionValidatorFactory validatorFactory, ILogger<QuizController> logger) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly JsonSerializerOptions _serializerOptions = jsonSerializerOptions;
    private readonly IQuestionValidatorFactory _validatorFactory = validatorFactory;
    private readonly ILogger<QuizController> _logger = logger;

    private const string BadRequestMessageTemplate = "Invalid request data: ";
    private const string LogWarningMessageTemplate = "Error: ";
    private const string ErrorMessageTemplate = "An unexpected error occured: ";
    private const string LogErrorMessageTemplate = "An unexpected error occured: ";

    [HttpPost("answer")]
    public async Task<ActionResult<string>> CheckAnswer([FromBody] CheckAnswerRequest request)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning(BadRequestMessageTemplate + "{ModelStateErrors}", ModelState.Values.SelectMany(v => v.Errors));
            return BadRequest(BadRequestMessageTemplate);
        }

        try
        {
            CheckAnswerCommand command = new()
            {
                PlayerName = request.PlayerName,
                QuestionAnswer = request.QuestionAnswer,
            };

            var response = await _mediator.Send(command);

            return response.Success == true
                ? Ok(response.Message)
                : BadRequest(response.Error is not null ? response.Error.Message : ErrorMessageTemplate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, LogErrorMessageTemplate + "{Error}", ex.Message);
            return StatusCode(500, ErrorMessageTemplate + ex.Message);
        }
    }

    [HttpPost("get")]
    public async Task<ActionResult<FourOptionQuestionDto>> GetQuestion([FromBody] GetQuestionRequest request)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning(BadRequestMessageTemplate + "{ModelStateErrors}", ModelState.Values.SelectMany(v => v.Errors));
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
            _logger.LogError(ex, LogErrorMessageTemplate + "{Error}", ex.Message);
            return StatusCode(500, ex + ErrorMessageTemplate + ex.Message);
        }
    }

    [HttpPost("initialize")]
    public async Task<ActionResult<string>> InitializeQuiz([FromBody] InitializeQuizRequest request)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning(BadRequestMessageTemplate + "{ModelStateErrors}", ModelState.Values.SelectMany(v => v.Errors));
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
            _logger.LogError(ex, LogErrorMessageTemplate + "{Error}", ex.Message);
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
            _logger.LogError(ex, LogErrorMessageTemplate + "{Error}", ex.Message);
            return StatusCode(500, ErrorMessageTemplate + ex.Message);
        }
    }

    [HttpPost("create/{questionType}")]
    public async Task<ActionResult<FourOptionQuestionDto>> CreateQuestion(string questionType, [FromBody] JsonElement fourOptionQuestionJson)
    {
        if (!ModelState.IsValid || string.IsNullOrEmpty(questionType))
        {
            _logger.LogWarning(BadRequestMessageTemplate + "{ModelStateErrors}", ModelState.Values.SelectMany(v => v.Errors));
            return BadRequest(BadRequestMessageTemplate);
        }

        try
        {
            FourOptionQuestion? fourOptionQuestion = DeserializeAndReturnQuestion(questionType, fourOptionQuestionJson);

            if (fourOptionQuestion is null)
            {
                _logger.LogWarning(BadRequestMessageTemplate + "{RequestDataErrors}", "Invalid question type.");
                return BadRequest("Please verify that you chose a valid question type.");
            }

            var (success, validationMessage) = ValidateRequestAndLogErrors<FourOptionQuestion>(fourOptionQuestion);
            if (!success)
                return BadRequest(validationMessage);

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
            _logger.LogError(ex, LogErrorMessageTemplate + "{Error}", ex.Message);
            return StatusCode(500, ErrorMessageTemplate + ex.Message);
        }
    }

    [HttpPatch("patch/{questionId}")]
    public async Task<ActionResult<FourOptionQuestion>> PatchQuestion(int questionId, [FromBody] PatchQuestionRequest request)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning(BadRequestMessageTemplate + "{ModelStateErrors}", ModelState.Values.SelectMany(v => v.Errors));
            return BadRequest(BadRequestMessageTemplate);
        }

        try
        {
            PatchQuestionCommand command = new()
            {
                Request = request,
                QuestionId = questionId,
            };

            var response = await _mediator.Send(command);

            return response.Success == true
                ? Ok(response.Question)
                : BadRequest(response.Error is not null ? response.Error.Message : ErrorMessageTemplate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, LogErrorMessageTemplate + "{Error}", ex.Message);
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
            _logger.LogError(ex, LogErrorMessageTemplate + "{Error}", ex.Message);
            return StatusCode(500, ErrorMessageTemplate + ex.Message);
        }
    }

    private FourOptionQuestion? DeserializeAndReturnQuestion(string questionType, JsonElement fourOptionQuestionJson)
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

            _ => null, // Check if response is null when using method. Return BadRequest() if such is the case.
        };
    }

    private (bool success, string? validationMessage) ValidateRequestAndLogErrors<T>(T instance)
    {
        var validator = _validatorFactory.GetValidator<T>();
        var validationResult = validator.Validate(instance);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed: {ValidationResult}", validationResult.ToString());
            return (false, $"Validation failed: {validationResult}");
        }

        return (true, null);
    }

    private bool VerifySuccessOrLogError(bool success, Exception? exception)
    {
        if (success)
            return true;

        _logger.LogWarning(BadRequestMessageTemplate + "{Error}", (exception is not null ? exception.Message : ""));

        return false;
    }
}
