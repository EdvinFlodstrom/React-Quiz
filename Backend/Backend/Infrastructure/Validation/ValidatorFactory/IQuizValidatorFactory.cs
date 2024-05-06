using FluentValidation;

namespace Backend.Infrastructure.Validation.ValidatorFactory;

public interface IQuizValidatorFactory
{
    IValidator<T> GetValidator<T>();
}
