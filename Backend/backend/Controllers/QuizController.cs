using Backend.Dtos;
using Backend.Handlers.Questions;
using Backend.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuizController(IMediator mediator, ILogger<QuizController> logger) : ControllerBase
{
    private const string BadRequestMessageTemplate = "Invalid request data";
    private const string WarningMessageTemplate = "Error: ";
    private const string ErrorMessageTemplate = "An unexpected error occured: ";
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<QuizController> _logger = logger;

    [HttpPost("create")]
    public async Task<ActionResult<FourOptionQuestionDto>> CreateQuestion([FromBody] FourOptionQuestion fourOptionQuestion)
    {
        if (!ModelState.IsValid || fourOptionQuestion is null)
        {
            _logger.LogWarning("Invalid request data: {ModelStateErrors}", ModelState.Values.SelectMany(v => v.Errors));
            return BadRequest(BadRequestMessageTemplate);
        }

        try
        {
            CreateQuestionCommand command = new()
            {
                FourOptionQuestion = fourOptionQuestion,
            };

            CreateQuestionCommandResponse response = await _mediator.Send(command);

            return VerifySuccessOrLogError(response.Success, response.Error)
                ? Ok(response)
                : BadRequest(response.Error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessageTemplate + ex.Message);
            return StatusCode(500, ErrorMessageTemplate + ex.Message);
        }
    }

    private bool VerifySuccessOrLogError(bool success, Exception? exception)
    {
        if (success)
            return true;

        _logger.LogWarning(WarningMessageTemplate + (exception is not null ? exception.Message : ""));

        return false;
    }
}
