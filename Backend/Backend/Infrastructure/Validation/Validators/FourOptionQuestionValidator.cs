using Backend.Infrastructure.Models.Entities;
using FluentValidation;

namespace Backend.Infrastructure.Validation.Validators;

public class FourOptionQuestionValidator : AbstractValidator<FourOptionQuestion>
{
    public FourOptionQuestionValidator()
    {
        RuleFor(x => x.Question)
            .NotEmpty()
            .WithMessage("Question may not be empty.");

        RuleFor(x => new { x.Option1, x.Option2, x.Option3, x.Option4 })
            .Must(options =>
                !string.IsNullOrEmpty(options.Option1) &&
                !string.IsNullOrEmpty(options.Option2) &&
                !string.IsNullOrEmpty(options.Option3) &&
                !string.IsNullOrEmpty(options.Option4))
            .WithMessage("Options may not be empty.");

        RuleFor(x => x.CorrectOptionNumber)
            .NotEmpty()
            .WithMessage("Correct option number may not be empty.")
            .InclusiveBetween(1, 4)
            .WithMessage("Correct option number must be in range 1-4.");
    }
}
