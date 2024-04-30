using FluentValidation;

namespace Backend.Infrastructure.Validation.ValidatorFactory;

public interface IValidatorFactory
{
    IValidator<T> GetValidator<T>();
}
