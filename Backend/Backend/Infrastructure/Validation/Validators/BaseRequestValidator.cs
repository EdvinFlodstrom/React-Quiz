using Backend.Infrastructure.Models.Requests;
using FluentValidation;

namespace Backend.Infrastructure.Validation.Validators;

public class BaseRequestValidator : AbstractValidator<BaseRequest>
{
    public BaseRequestValidator()
    {
        RuleFor(x => x.PlayerName)
            .NotEmpty()
            .WithMessage("Player name may not be empty.")
            .MinimumLength(2)
            .WithMessage("Player name must be at least 2 characters long");
    }
}
