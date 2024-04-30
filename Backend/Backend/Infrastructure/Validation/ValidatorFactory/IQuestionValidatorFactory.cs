using FluentValidation;

namespace Backend.Infrastructure.Validation.ValidatorFactory;

public interface IQuestionValidatorFactory
{
    IValidator<T> GetValidator<T>();
}
