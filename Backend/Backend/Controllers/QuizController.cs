using Backend.Handlers.Questions;
using Backend.Handlers.Quiz;
using Backend.Infrastructure.Models.Dtos;
using Backend.Infrastructure.Models.Entities;
using Backend.Infrastructure.Models.Entities.QuestionTypes;
using Backend.Infrastructure.Models.Requests;
using Backend.Infrastructure.Models.Responses;
using Backend.Infrastructure.Validation.ValidatorFactory;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuizController(IMediator mediator, JsonSerializerOptions jsonSerializerOptions, IQuizValidatorFactory validatorFactory, ILogger<QuizController> logger) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly JsonSerializerOptions _serializerOptions = jsonSerializerOptions;
    private readonly IQuizValidatorFactory _validatorFactory = validatorFactory;
    private readonly ILogger<QuizController> _logger = logger;

    private const string BadRequestMessageTemplate = "Invalid request data: ";
    private const string ErrorMessageTemplate = "An unexpected error occured: ";
    private const string LogErrorMessageTemplate = "An unexpected error occured: ";

    [HttpPost("answer")]
    public async Task<ActionResult<CheckAnswerResponse>> CheckAnswer([FromBody] CheckAnswerRequest request)
    {
        if (!ModelState.IsValid || request is null)
        {
            _logger.LogWarning(BadRequestMessageTemplate + "{ModelStateErrors}", ModelState.Values.SelectMany(v => v.Errors));
            return BadRequest(BadRequestMessageTemplate);
        }

        try
        {
            var (success, validationMessage) = ValidateRequestAndLogErrors<BaseRequest>(request);
            if (!success)
                return BadRequest(validationMessage);

            CheckAnswerCommand command = new()
            {
                PlayerName = request.PlayerName,
                QuestionAnswer = request.QuestionAnswer,
            };

            var response = await _mediator.Send(command);

            return response.Success == true
                ? Ok(new CheckAnswerResponse
                {
                    Message = response.Message!,
                    Correct = response.Correct,
                    CorrectOption = Convert.ToInt32(response.CorrectOption),
                })
                : BadRequest(response.Error is not null ? response.Error.Message : ErrorMessageTemplate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, LogErrorMessageTemplate + "{Error}", ex.Message);
            return StatusCode(500, ErrorMessageTemplate + ex.Message);
        }
    }

    [HttpPost("get")]
    public async Task<ActionResult<GetQuestionResponse>> GetQuestion([FromBody] GetQuestionRequest request)
    {
        if (!ModelState.IsValid || request is null)
        {
            _logger.LogWarning(BadRequestMessageTemplate + "{ModelStateErrors}", ModelState.Values.SelectMany(v => v.Errors));
            return BadRequest(BadRequestMessageTemplate);
        }

        try
        {
            (bool success, string? validationMessage) = ValidateRequestAndLogErrors<BaseRequest>(request);
            if (!success)
                return BadRequest(validationMessage);

            GetQuestionCommand command = new()
            {
                PlayerName = request.PlayerName,
            };

            var response = await _mediator.Send(command);

            return response.Success == true
                ? Ok(new GetQuestionResponse
                {
                    FourOptionQuestion = response.Question,
                    Details = response.Details
                })
                : BadRequest(response.Error is not null ? response.Error.Message : ErrorMessageTemplate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, LogErrorMessageTemplate + "{Error}", ex.Message);
            return StatusCode(500, ex + ErrorMessageTemplate + ex.Message);
        }
    }

    [HttpGet("get/{questionId:int}")]
    public async Task<ActionResult<FourOptionQuestionByIdDto>> GetQuestionById(int questionId)
    {
        if (questionId <= 0)
            return BadRequest(BadRequestMessageTemplate);

        try
        {
            GetQuestionByIdCommand command = new()
            {
                QuestionId = questionId,
            };

            var response = await _mediator.Send(command);

            if (response.Success == true)
                return Ok(response.Question);

            if (response.Error is not null)
            {
                if (response.Error is ArgumentException)
                    return NotFound(response.Error.Message);
                return BadRequest(response.Error.Message);
            }
            return BadRequest(ErrorMessageTemplate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, LogErrorMessageTemplate + "{Error}", ex.Message);
            return StatusCode(500, ErrorMessageTemplate + ex.Message);
        }
    }

    [HttpPost("initialize")]
    public async Task<ActionResult<string>> InitializeQuiz([FromBody] InitializeQuizRequest request)
    {
        if (!ModelState.IsValid || request is null)
        {
            _logger.LogWarning(BadRequestMessageTemplate + "{ModelStateErrors}", ModelState.Values.SelectMany(v => v.Errors));
            return BadRequest(BadRequestMessageTemplate);
        }

        try
        {
            var (success, validationMessage) = ValidateRequestAndLogErrors<BaseRequest>(request);
            if (!success)
                return BadRequest(validationMessage);

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
            (FourOptionQuestion? fourOptionQuestion, string? message) = DeserializeAndReturnQuestion(questionType, fourOptionQuestionJson);

            if (fourOptionQuestion is null)
            {
                _logger.LogWarning(BadRequestMessageTemplate + "{RequestDataErrors}", message);
                return BadRequest(message);
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
        if (!ModelState.IsValid || request is null)
        {
            _logger.LogWarning(BadRequestMessageTemplate + "{ModelStateErrors}", ModelState.Values.SelectMany(v => v.Errors));
            return BadRequest(BadRequestMessageTemplate);
        }

        try
        {
            var (success, validationMessage) = ValidateRequestAndLogErrors<PatchQuestionRequest>(request);
            if (!success)
                return BadRequest(validationMessage);

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

    private (FourOptionQuestion? fourOptionQuestion, string? message) DeserializeAndReturnQuestion(string questionType, JsonElement fourOptionQuestionJson)
    {
        FourOptionQuestion? question = null;

        try
        {
            question = questionType.ToLower() switch
            {
                "chemistry" => JsonSerializer.Deserialize<ChemistryQuestion>(fourOptionQuestionJson, _serializerOptions),

                "food" => JsonSerializer.Deserialize<FoodQuestion>(fourOptionQuestionJson, _serializerOptions),

                "game" => JsonSerializer.Deserialize<GameQuestion>(fourOptionQuestionJson, _serializerOptions),

                "geography" => JsonSerializer.Deserialize<GeographyQuestion>(fourOptionQuestionJson, _serializerOptions),

                "history" => JsonSerializer.Deserialize<HistoryQuestion>(fourOptionQuestionJson, _serializerOptions),

                "literature" => JsonSerializer.Deserialize<LiteratureQuestion>(fourOptionQuestionJson, _serializerOptions),

                "math" => JsonSerializer.Deserialize<MathQuestion>(fourOptionQuestionJson, _serializerOptions),

                "music" => JsonSerializer.Deserialize<MusicQuestion>(fourOptionQuestionJson, _serializerOptions),

                "sports" => JsonSerializer.Deserialize<SportsQuestion>(fourOptionQuestionJson, _serializerOptions),

                "technology" => JsonSerializer.Deserialize<TechnologyQuestion>(fourOptionQuestionJson, _serializerOptions),

                _ => new InvalidQuestionType // Use only to differentiate from JSON deserialization failures.
                {
                    Question = "",
                    Option1 = "",
                    Option2 = "",
                    Option3 = "",
                    Option4 = "",
                    CorrectOptionNumber = 1,
                },
            };
        }
        catch (JsonException ex)
        {
            _logger.LogWarning("JSON deserialization failed: {JsonException}", ex.Message);
            return (null, "Deserialization failed: " + ex.Message);
        }

        if (question is null)
            return (null, "Deserialization failed. Please verify your request body.");
        else if (question is InvalidQuestionType)
            return (null, "Invalid question type. Please verify that you chose a valid question type.");
        else
            return (question, null);
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
