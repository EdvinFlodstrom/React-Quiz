using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuizController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
}
