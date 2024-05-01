using Backend.Infrastructure.Models.Requests;
using FluentValidation;

namespace Backend.Infrastructure.Validation.Validators;

public class PatchQuestionRequestValidator : AbstractValidator<PatchQuestionRequest>
{
    public PatchQuestionRequestValidator()
    {
        RuleFor(x => x.Question)
            .NotEmpty()
            .WithMessage("Question may not be empty.")
            .Unless(x => x.Question is null);

        RuleFor(x => x)
            .Custom((model, context) =>
            {
                if (model.Option1 is not null && string.IsNullOrEmpty(model.Option1))
                    context.AddFailure("Option1 may not be empty.");
                if (model.Option2 is not null && string.IsNullOrEmpty(model.Option2))
                    context.AddFailure("Option2 may not be empty.");
                if (model.Option3 is not null && string.IsNullOrEmpty(model.Option3))
                    context.AddFailure("Option3 may not be empty.");
                if (model.Option4 is not null && string.IsNullOrEmpty(model.Option4))
                    context.AddFailure("Option4 may not be empty.");
            });
    }
}
